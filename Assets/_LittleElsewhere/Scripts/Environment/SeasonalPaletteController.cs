using UnityEngine;
using LittleElsewhere.TimeSystem;

namespace LittleElsewhere.Environment
{
    public enum WaterState
    {
        Frozen,
        Thawed
    }

    /// <summary>
    /// Swaps/blends ground and foliage material properties and toggles lake ice water state according to season.
    /// </summary>
    public class SeasonalPaletteController : MonoBehaviour
    {
        [Header("Water & Lake State")]
        [SerializeField] private WaterState currentWaterState = WaterState.Frozen;

        [Header("Material References")]
        [SerializeField] private Material groundMaterial;
        [SerializeField] private Material foliageMaterial;

        [Header("Seasonal Ground Colors")]
        [SerializeField] private Color winterGroundColor = new Color(0.97f, 0.98f, 0.99f); // Soft white snow
        [SerializeField] private Color springGroundColor = new Color(0.65f, 0.78f, 0.58f); // Soft spring green
        [SerializeField] private Color summerGroundColor = new Color(0.48f, 0.68f, 0.42f); // Vibrant summer green
        [SerializeField] private Color autumnGroundColor = new Color(0.78f, 0.62f, 0.42f); // Autumn gold/brown

        public WaterState CurrentWaterState => currentWaterState;

        public void ApplySeasonPalette(Season season)
        {
            currentWaterState = (season == Season.Winter) ? WaterState.Frozen : WaterState.Thawed;

            Color targetColor = winterGroundColor;
            switch (season)
            {
                case Season.Winter: targetColor = winterGroundColor; break;
                case Season.Spring: targetColor = springGroundColor; break;
                case Season.Summer: targetColor = summerGroundColor; break;
                case Season.Autumn: targetColor = autumnGroundColor; break;
            }

            if (groundMaterial != null)
            {
                groundMaterial.color = targetColor;
            }

            Debug.Log($"[SeasonalPaletteController] Season Palette Applied: {season}, Water State: {currentWaterState}");
        }
    }
}
