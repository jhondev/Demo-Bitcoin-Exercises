using System;
using NBitcoin;

namespace Demo_Bitcoin_Exercises
{
  class Program
  {
    static void Main(string[] args)
    {
      // Keys();
      ScriptPubKeys();
    }

    static void ScriptPubKeys()
    {
      var publicKeyHash1 = new Key().ScriptPubKey;
      Console.WriteLine(publicKeyHash1);
      Console.WriteLine("public key hash: " + publicKeyHash1.GetDestination());
      Console.WriteLine("address from above hash: " + publicKeyHash1.GetDestinationAddress(Network.Main));

      var publicKeyHash = new KeyId("14836dbe7f38c5ac3d49e8d790af808a4ee9edcf");

      var testNetAddress = publicKeyHash.GetAddress(Network.TestNet);
      var mainNetAddress = publicKeyHash.GetAddress(Network.Main);
      Console.WriteLine("mainNetAddress " + mainNetAddress);

      Console.WriteLine(publicKeyHash.ScriptPubKey);
      Console.WriteLine(mainNetAddress.ScriptPubKey); // OP_DUP OP_HASH160 14836dbe7f38c5ac3d49e8d790af808a4ee9edcf OP_EQUALVERIFY OP_CHECKSIG
      Console.WriteLine(testNetAddress.ScriptPubKey); // OP_DUP OP_HASH160 14836dbe7f38c5ac3d49e8d790af808a4ee9edcf OP_EQUALVERIFY OP_CHECKSIG

      var paymentScript = publicKeyHash.ScriptPubKey;
      var sameMainNetAddress = paymentScript.GetDestinationAddress(Network.Main);
      Console.WriteLine("Main? " + (mainNetAddress == sameMainNetAddress)); // True
      Console.WriteLine("sameMainNetAddress " + sameMainNetAddress);

      var samePublicKeyHash = (KeyId)paymentScript.GetDestination();
      Console.WriteLine("samePublicKeyHash: " + samePublicKeyHash); // True
      var sameMainNetAddress2 = new BitcoinPubKeyAddress(samePublicKeyHash, Network.Main);
      Console.WriteLine("sameMainNetAddress " + sameMainNetAddress2);
    }

    static void Keys()
    {
      Console.WriteLine("***********Keys*****************");
      var privateKey = new Key();
      Console.WriteLine("Private key: " + privateKey.GetWif(Network.Main));

      var publicKey = privateKey.PubKey;
      Console.WriteLine("Public key: " + publicKey);

      var addressMainNet = publicKey.GetAddress(Network.Main);
      Console.WriteLine("Address MainNet: " + addressMainNet);

      var addressTestNet = publicKey.GetAddress(Network.TestNet);
      Console.WriteLine("Address TestNet: " + addressTestNet);

      Console.WriteLine("*******************************");
    }
  }
}
