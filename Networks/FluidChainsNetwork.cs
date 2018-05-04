/* using NBitcoin;
using NBitcoin.BouncyCastle.Math;
using NBitcoin.DataEncoders;
using NBitcoin.Protocol;
using System;
using System.Collections.Generic;

namespace FluidExplorer.WebApi
{
    public class FluidChainsNetwork
    {
        public int NetworkPort => 39391;

        public void Register()
        {
            Block.BlockSignature = true;
            Transaction.TimeStamp = true;

            var consensus = new Consensus();

            consensus.NetworkOptions = new NetworkOptions() { IsProofOfStake = true };
            consensus.GetPoWHash = (n, h) => NBitcoin.Crypto.HashX13.Instance.Hash(h.ToBytes(options: n));

            consensus.SubsidyHalvingInterval = 210000;
            consensus.MajorityEnforceBlockUpgrade = 750;
            consensus.MajorityRejectBlockOutdated = 950;
            consensus.MajorityWindow = 1000;
            consensus.BuriedDeployments[BuriedDeployments.BIP34] = 227931;
            consensus.BuriedDeployments[BuriedDeployments.BIP65] = 388381;
            consensus.BuriedDeployments[BuriedDeployments.BIP66] = 363725;
            consensus.BIP34Hash = new uint256("0x000000000000024b89b42a942fe0d9fea3bb44ab7bd1b19115dd6a759c0808b8");
            consensus.PowLimit = new Target(new uint256("00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff"));
            consensus.PowTargetTimespan = TimeSpan.FromSeconds(14 * 24 * 60 * 60); // two weeks
            consensus.PowTargetSpacing = TimeSpan.FromSeconds(10 * 60);
            consensus.PowAllowMinDifficultyBlocks = false;
            consensus.PowNoRetargeting = false;
            consensus.RuleChangeActivationThreshold = 1916; // 95% of 2016
            consensus.MinerConfirmationWindow = 2016; // nPowTargetTimespan / nPowTargetSpacing

            consensus.BIP9Deployments[BIP9Deployments.TestDummy] = new BIP9DeploymentsParameters(28, 1199145601, 1230767999);
            consensus.BIP9Deployments[BIP9Deployments.CSV] = new BIP9DeploymentsParameters(0, 1462060800, 1493596800);
            consensus.BIP9Deployments[BIP9Deployments.Segwit] = new BIP9DeploymentsParameters(1, 0, 0);

            consensus.LastPOWBlock = 10000;

            consensus.ProofOfStakeLimit = new BigInteger(uint256.Parse("00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff").ToBytes(false));
            consensus.ProofOfStakeLimitV2 = new BigInteger(uint256.Parse("000000000000ffffffffffffffffffffffffffffffffffffffffffffffffffff").ToBytes(false));

            consensus.CoinType = 105;

            consensus.DefaultAssumeValid = new uint256("0x8c2cf95f9ca72e13c8c4cdf15c2d7cc49993946fb49be4be147e106d502f1869"); // 642930

            Block genesis = CreateStratisGenesisBlock(1519534128, 55270, 0x1e0fffff, 1, Money.Zero);
            consensus.HashGenesisBlock = genesis.GetHash(consensus.NetworkOptions);

            // The message start string is designed to be unlikely to occur in normal data.
            // The characters are rarely used upper ASCII, not valid as UTF-8, and produce
            // a large 4-byte int at any alignment.
            var messageStart = new byte[4];
            messageStart[0] = 0x69;
            messageStart[1] = 0x75;
            messageStart[2] = 0x38;
            messageStart[3] = 0x06;
            var magic = BitConverter.ToUInt32(messageStart, 0); //0x5223570; 

            Assert(consensus.HashGenesisBlock == uint256.Parse("000007c70a931f5cafa42d48eb0f7a627007cada84d84442e7369cc962a0d38f"));
            Assert(genesis.Header.HashMerkleRoot == uint256.Parse("0x50b30f93db7ee932ee4a01a9673cc0cb114cd8239c5806e8ea47b3c497057c9f"));


            var builder = new NetworkBuilder()
                .SetName("FluidMain")
                .SetRootFolderName("fluid")
                .SetDefaultConfigFilename("fluid.conf")
                .SetConsensus(consensus)
                .SetMagic(magic)
                .SetGenesis(genesis)
                .SetPort(NetworkPort)
                .SetRPCPort(39390)
                .SetTxFees(10000, 60000, 10000)
                // maximal value for the calculated time offset. If the value is over this limit, the time syncing feature will be switched off.
                .SetMaxTimeOffsetSeconds(25 * 60)
                // default value for the maximum tip age in seconds to consider the node in initial block download (2 hours).
                .SetMaxTipAge(2 * 60 * 60)

                .AddDNSSeeds(new[]
                {
                    new DNSSeedData("seednode1.fluidchains.net", "seednode1.fluidchains.net"),
                    new DNSSeedData("seednode2.fluidchains.cloud", "seednode2.fluidchains.cloud"),
                    new DNSSeedData("seednode3.fluidchains.net", "seednode3.fluidchains.net"),
                    new DNSSeedData( "seednode4.fluidchains.cloud", "seednode4.fluidchains.cloud")
                  })

                .SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, new byte[] { (35) })
                .SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, new byte[] { (95) })
                .SetBase58Bytes(Base58Type.SECRET_KEY, new byte[] { (35 + 128) })
                .SetBase58Bytes(Base58Type.ENCRYPTED_SECRET_KEY_NO_EC, new byte[] { 0x01, 0x42 })
                .SetBase58Bytes(Base58Type.ENCRYPTED_SECRET_KEY_EC, new byte[] { 0x01, 0x43 })
                .SetBase58Bytes(Base58Type.EXT_PUBLIC_KEY, new byte[] { (0x04), (0x88), (0xB2), (0x1E) })
                .SetBase58Bytes(Base58Type.EXT_SECRET_KEY, new byte[] { (0x04), (0x88), (0xAD), (0xE4) })
                .SetBase58Bytes(Base58Type.PASSPHRASE_CODE, new byte[] { 0x2C, 0xE9, 0xB3, 0xE1, 0xFF, 0x39, 0xE2 })
                .SetBase58Bytes(Base58Type.CONFIRMATION_CODE, new byte[] { 0x64, 0x3B, 0xF6, 0xA8, 0x9A })
                .SetBase58Bytes(Base58Type.STEALTH_ADDRESS, new byte[] { 0x2a })
                .SetBase58Bytes(Base58Type.ASSET_ID, new byte[] { 23 })
                .SetBase58Bytes(Base58Type.COLORED_ADDRESS, new byte[] { 0x13 })
                .SetBech32(Bech32Type.WITNESS_PUBKEY_ADDRESS, "bc")
                .SetBech32(Bech32Type.WITNESS_SCRIPT_ADDRESS, "bc");

            var seed = new[] { "73.204.72.128:54674", "73.204.72.128:52629", "73.204.72.128:39391", "64.139.137.74:39391", "localhost:39391" };
            var fixedSeeds = new List<NetworkAddress>();
            // Convert the pnSeeds array into usable address objects.
            Random rand = new Random();
            TimeSpan oneWeek = TimeSpan.FromDays(7);
            for (int i = 0; i < seed.Length; i++)
            {
                // It'll only connect to one or two seed nodes because once it connects,
                // it'll get a pile of addresses with newer timestamps.                
                NetworkAddress addr = new NetworkAddress();
                // Seed nodes are given a random 'last seen time' of between one and two
                // weeks ago.
                addr.Time = DateTime.UtcNow - (TimeSpan.FromSeconds(rand.NextDouble() * oneWeek.TotalSeconds)) - oneWeek;
                addr.Endpoint = Utils.ParseIpEndpoint(seed[i], NetworkPort);
                fixedSeeds.Add(addr);
            }

            builder.AddSeeds(fixedSeeds);
            builder.BuildAndRegister();
        }

        private static void Assert(bool condition)
        {
            // TODO: use Guard when this moves to the FN.
            if (!condition)
            {
                throw new InvalidOperationException("Invalid network");
            }
        }

        private static Block CreateStratisGenesisBlock(uint nTime, uint nNonce, uint nBits, int nVersion, Money genesisReward)
        {
            string pszTimestamp = "https://www.olympic.org/news/today-at-pyeongchang-2018-sunday-25-february";
            return CreateStratisGenesisBlock(pszTimestamp, nTime, nNonce, nBits, nVersion, genesisReward);
        }

        private static Block CreateStratisGenesisBlock(string pszTimestamp, uint nTime, uint nNonce, uint nBits, int nVersion, Money genesisReward)
        {
            Transaction txNew = new Transaction();
            txNew.Version = 1;
            txNew.Time = nTime;
            txNew.AddInput(new TxIn()
            {
                ScriptSig = new Script(Op.GetPushOp(0), new Op()
                {
                    Code = (OpcodeType)0x1,
                    PushData = new[] { (byte)42 }
                }, Op.GetPushOp(Encoders.ASCII.DecodeData(pszTimestamp)))
            });
            txNew.AddOutput(new TxOut()
            {
                Value = genesisReward,
            });
            Block genesis = new Block();
            genesis.Header.BlockTime = Utils.UnixTimeToDateTime(nTime);
            genesis.Header.Bits = nBits;
            genesis.Header.Nonce = nNonce;
            genesis.Header.Version = nVersion;
            genesis.Transactions.Add(txNew);
            genesis.Header.HashPrevBlock = uint256.Zero;
            genesis.UpdateMerkleRoot();
            return genesis;
        }
    }
}
 */