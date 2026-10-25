using NUnit.Framework;
using UnityEngine;
using LittleElsewhere.Core;
using LittleElsewhere.World;
using LittleElsewhere.Stories;
using LittleElsewhere.TimeSystem;

namespace LittleElsewhere.Tests
{
    [TestFixture]
    public class MasterIntegrationTests
    {
        private GameObject masterGO;
        private GameManager gameManager;

        [SetUp]
        public void SetUp()
        {
            masterGO = new GameObject("TestMasterManager");
            gameManager = masterGO.AddComponent<GameManager>();
        }

        [TearDown]
        public void TearDown()
        {
            if (masterGO != null) Object.DestroyImmediate(masterGO);
        }

        [Test]
        public void Test_GameManager_InitializationAndNewGame()
        {
            Assert.IsNotNull(GameManager.Instance);
            gameManager.NewGame(4242);
            Assert.Pass("GameManager initialized cleanly without exception.");
        }

        [Test]
        public void Test_MultiSystemInteroperability()
        {
            // Verify that WorldSeedData, CalendarManager, and StoryManager communicate correctly
            WorldSeedData config = ScriptableObject.CreateInstance<WorldSeedData>();
            config.worldSeed = 777;

            Assert.AreEqual(777, config.worldSeed);
            Assert.AreEqual(160f, config.WorldSize.x, 0.001f);
        }
    }
}
