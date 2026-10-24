using NUnit.Framework;
using UnityEngine;
using LittleElsewhere.TimeSystem;
using LittleElsewhere.Environment;

namespace LittleElsewhere.Tests
{
    [TestFixture]
    public class CalendarSystemTests
    {
        private GameObject calendarGO;
        private CalendarManager calendarManager;

        [SetUp]
        public void SetUp()
        {
            calendarGO = new GameObject();
            calendarManager = calendarGO.AddComponent<CalendarManager>();
        }

        [TearDown]
        public void TearDown()
        {
            if (calendarGO != null) Object.DestroyImmediate(calendarGO);
        }

        [Test]
        public void Test_AdvanceDay_IncrementsDayAndTransitionsSeasonAfter12Days()
        {
            Assert.AreEqual(Season.Winter, calendarManager.CurrentSeason);
            Assert.AreEqual(1, calendarManager.CurrentDayOfSeason);

            // Advance 12 days
            for (int i = 0; i < 12; i++)
            {
                calendarManager.AdvanceDay();
            }

            Assert.AreEqual(Season.Spring, calendarManager.CurrentSeason, "Season should transition from Winter to Spring after 12 days.");
            Assert.AreEqual(1, calendarManager.CurrentDayOfSeason);
        }

        [Test]
        public void Test_FullYearSeasonProgression()
        {
            Assert.AreEqual(1, calendarManager.CurrentYear);

            // Advance 4 full seasons (48 days)
            for (int i = 0; i < 48; i++)
            {
                calendarManager.AdvanceDay();
            }

            Assert.AreEqual(Season.Winter, calendarManager.CurrentSeason);
            Assert.AreEqual(2, calendarManager.CurrentYear, "Year should increment after completing 4 seasons.");
        }

        [Test]
        public void Test_RestAndWakeUp_AdvancesTimeToTargetHour()
        {
            calendarManager.SetCalendarState(14.0f, 1, Season.Winter, 1); // 2 PM
            calendarManager.RestAndWakeUp(6.0f); // Wake up at 6 AM next day

            Assert.AreEqual(6.0f, calendarManager.CurrentHour, 0.001f);
            Assert.AreEqual(2, calendarManager.CurrentDayOfSeason, "Resting past midnight should advance day count.");
        }

        [Test]
        public void Test_SeasonalPaletteController_TogglesWaterState()
        {
            GameObject paletteGO = new GameObject();
            SeasonalPaletteController palette = paletteGO.AddComponent<SeasonalPaletteController>();

            palette.ApplySeasonPalette(Season.Winter);
            Assert.AreEqual(WaterState.Frozen, palette.CurrentWaterState, "Water should be frozen during Winter.");

            palette.ApplySeasonPalette(Season.Spring);
            Assert.AreEqual(WaterState.Thawed, palette.CurrentWaterState, "Water should be thawed during Spring.");

            Object.DestroyImmediate(paletteGO);
        }
    }
}
