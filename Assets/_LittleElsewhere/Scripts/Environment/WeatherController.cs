using UnityEngine;
using LittleElsewhere.TimeSystem;

namespace LittleElsewhere.Environment
{
    public enum WeatherState
    {
        Clear,
        Snowfall,
        Rain,
        Mist
    }

    /// <summary>
    /// Controls atmospheric weather states, particle systems (snowfall/rain), and ambient mist/fog.
    /// Weather state transitions dynamically based on season and random atmospheric factors.
    /// </summary>
    public class WeatherController : MonoBehaviour
    {
        [Header("Current Weather")]
        [SerializeField] private WeatherState currentWeather = WeatherState.Clear;

        [Header("Particle System References")]
        [SerializeField] private ParticleSystem snowfallParticleSystem;
        [SerializeField] private ParticleSystem rainParticleSystem;

        [Header("Mist & Fog Settings")]
        [SerializeField] private Color mistColor = new Color(0.82f, 0.88f, 0.94f, 0.4f);

        public WeatherState CurrentWeather => currentWeather;
        public event System.Action<WeatherState> OnWeatherChanged;

        public void SetWeather(WeatherState newState)
        {
            currentWeather = newState;

            if (snowfallParticleSystem != null)
            {
                var main = snowfallParticleSystem.main;
                main.loop = (newState == WeatherState.Snowfall);
                if (newState == WeatherState.Snowfall) snowfallParticleSystem.Play();
                else snowfallParticleSystem.Stop();
            }

            if (rainParticleSystem != null)
            {
                var main = rainParticleSystem.main;
                main.loop = (newState == WeatherState.Rain);
                if (newState == WeatherState.Rain) rainParticleSystem.Play();
                else rainParticleSystem.Stop();
            }

            RenderSettings.fog = (newState == WeatherState.Mist || newState == WeatherState.Snowfall);
            RenderSettings.fogColor = mistColor;

            Debug.Log($"[WeatherController] Weather changed to: {currentWeather}");
            OnWeatherChanged?.Invoke(currentWeather);
        }

        public void EvaluateSeasonalWeather(Season season)
        {
            // Pick appropriate weather for season
            switch (season)
            {
                case Season.Winter:
                    SetWeather(Random.value > 0.4f ? WeatherState.Snowfall : WeatherState.Clear);
                    break;
                case Season.Spring:
                case Season.Autumn:
                    SetWeather(Random.value > 0.5f ? WeatherState.Rain : WeatherState.Mist);
                    break;
                case Season.Summer:
                    SetWeather(WeatherState.Clear);
                    break;
            }
        }
    }
}
