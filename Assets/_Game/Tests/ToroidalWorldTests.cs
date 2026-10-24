using NUnit.Framework;
using UnityEngine;
using LittleElsewhere.World;

namespace LittleElsewhere.Tests
{
    [TestFixture]
    public class ToroidalWorldTests
    {
        private Vector2 worldSize = new Vector2(160f, 160f);

        [Test]
        public void Test_WrapPosition_EastBoundary()
        {
            Vector2 pos = new Vector2(165f, 50f);
            Vector2 wrapped = ToroidalMath.WrapPosition(pos, worldSize);
            Assert.AreEqual(5f, wrapped.x, 0.001f);
            Assert.AreEqual(50f, wrapped.y, 0.001f);
        }

        [Test]
        public void Test_WrapPosition_WestBoundary()
        {
            Vector2 pos = new Vector2(-10f, 50f);
            Vector2 wrapped = ToroidalMath.WrapPosition(pos, worldSize);
            Assert.AreEqual(150f, wrapped.x, 0.001f);
            Assert.AreEqual(50f, wrapped.y, 0.001f);
        }

        [Test]
        public void Test_WrapPosition_NorthBoundary()
        {
            Vector2 pos = new Vector2(50f, 170f);
            Vector2 wrapped = ToroidalMath.WrapPosition(pos, worldSize);
            Assert.AreEqual(50f, wrapped.x, 0.001f);
            Assert.AreEqual(10f, wrapped.y, 0.001f);
        }

        [Test]
        public void Test_WrapPosition_SouthBoundary()
        {
            Vector2 pos = new Vector2(50f, -15f);
            Vector2 wrapped = ToroidalMath.WrapPosition(pos, worldSize);
            Assert.AreEqual(50f, wrapped.x, 0.001f);
            Assert.AreEqual(145f, wrapped.y, 0.001f);
        }

        [Test]
        public void Test_WrapPosition_Corner_Northeast()
        {
            Vector2 pos = new Vector2(165f, 170f);
            Vector2 wrapped = ToroidalMath.WrapPosition(pos, worldSize);
            Assert.AreEqual(5f, wrapped.x, 0.001f);
            Assert.AreEqual(10f, wrapped.y, 0.001f);
        }

        [Test]
        public void Test_WrapPosition_Corner_Southwest()
        {
            Vector2 pos = new Vector2(-5f, -10f);
            Vector2 wrapped = ToroidalMath.WrapPosition(pos, worldSize);
            Assert.AreEqual(155f, wrapped.x, 0.001f);
            Assert.AreEqual(150f, wrapped.y, 0.001f);
        }

        [Test]
        public void Test_RepeatedWrapping()
        {
            Vector2 pos = new Vector2(500f, -480f);
            Vector2 wrapped = ToroidalMath.WrapPosition(pos, worldSize);
            Assert.GreaterOrEqual(wrapped.x, 0f);
            Assert.Less(wrapped.x, worldSize.x);
            Assert.GreaterOrEqual(wrapped.y, 0f);
            Assert.Less(wrapped.y, worldSize.y);
        }

        [Test]
        public void Test_WrappedDelta_ShortestPathAcrossBoundary()
        {
            Vector2 from = new Vector2(5f, 50f);
            Vector2 to = new Vector2(155f, 50f);
            Vector2 delta = ToroidalMath.WrappedDelta(from, to, worldSize);
            Assert.AreEqual(-10f, delta.x, 0.001f); // Shortest path is west across wraparound
        }

        [Test]
        public void Test_PeriodicNoise_ContinuousAcrossBoundaries()
        {
            int seed = 1337;
            float noiseLeft = ToroidalMath.SamplePeriodicNoise(0f, 50f, worldSize, 1.0f, seed);
            float noiseRight = ToroidalMath.SamplePeriodicNoise(worldSize.x, 50f, worldSize, 1.0f, seed);

            Assert.AreEqual(noiseLeft, noiseRight, 0.001f, "Periodic noise must match seamlessly across East-West boundaries");

            float noiseBottom = ToroidalMath.SamplePeriodicNoise(50f, 0f, worldSize, 1.0f, seed);
            float noiseTop = ToroidalMath.SamplePeriodicNoise(50f, worldSize.y, worldSize, 1.0f, seed);

            Assert.AreEqual(noiseBottom, noiseTop, 0.001f, "Periodic noise must match seamlessly across North-South boundaries");
        }

        [Test]
        public void Test_ConsistentWorldGeneration()
        {
            WorldSeedData config = ScriptableObject.CreateInstance<WorldSeedData>();
            config.worldSeed = 9999;

            ChunkData chunkA = ProceduralChunkGenerator.GenerateChunk(new Vector2Int(0, 0), config);
            ChunkData chunkB = ProceduralChunkGenerator.GenerateChunk(new Vector2Int(0, 0), config);

            Assert.AreEqual(chunkA.trees.Count, chunkB.trees.Count);
            Assert.AreEqual(chunkA.type, chunkB.type);
        }
    }
}
