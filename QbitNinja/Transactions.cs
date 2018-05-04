using System;
using System.Collections.Generic;
using System.IO;
using NBitcoin;
using QBitNinja.Client;
using QBitNinja.Client.Models;

public class Transactions
{
  public static void ShowTransaction()
  {
    // Create a client
    var client = new QBitNinjaClient(Network.Main);
    // Parse transaction id to NBitcoin.uint256 so the client can eat it
    var transactionId = uint256.Parse("f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94");
    // Query the transaction
    var transactionResponse = client.GetTransaction(transactionId).Result;

    // CreateFileWithTxInfo(transactionResponse);

    PrintCoins(transactionResponse.ReceivedCoins, "Received coins (outputs)");
    PrintCoins(transactionResponse.SpentCoins, "Spent coins (outputs)");
    SpentCoins(transactionResponse);

    Console.WriteLine(transactionResponse.Transaction.Version);
  }

  public static void PrintCoins(List<ICoin> coins, string title)
  {
    Console.WriteLine("*********{0}**********", title);
    foreach (var coin in coins)
    {
      Money amount = (Money)coin.Amount;

      Console.WriteLine("Amount: {0}", amount.ToDecimal(MoneyUnit.BTC));
      var paymentScript = coin.TxOut.ScriptPubKey;
      Console.WriteLine("scriptPubKey: {0}", paymentScript);  // It's the ScriptPubKey
      var address = paymentScript.GetDestinationAddress(Network.Main);
      Console.WriteLine("address: {0}", address);
      Console.WriteLine();
    }
    Console.WriteLine("*******************************************");
  }

  public static void SpentCoins(GetTransactionResponse transactionResponse)
  {
    Console.WriteLine("************Spent coins (outputs)**********");
    var inputs = transactionResponse.Transaction.Inputs;
    foreach (TxIn input in inputs)
    {
      OutPoint previousOutpoint = input.PrevOut;
      Console.WriteLine(previousOutpoint.Hash); // hash of prev tx
      Console.WriteLine(previousOutpoint.N); // idx of out from prev tx, that has been spent in the current tx      
      Console.WriteLine();
    }
    Console.WriteLine("*******************************************");
  }

  public static void CreateFileWithTxInfo(GetTransactionResponse transactionResponse)
  {
    using (var logFile = System.IO.File.Create(@"C:\Users\Usuario\Desktop\Tx.txt"))
    using (var logWriter = new System.IO.StreamWriter(logFile))
    {
      logWriter.WriteLine(transactionResponse.Transaction.ToString());
    }
  }
}