using UnityEngine;

namespace LittleElsewhere.Environment
{
    /// <summary>
    /// Animates campsite fire pit with subtle warm flickering light and flame effects.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class CampfireAnimator : MonoBehaviour
    {
        [Header("Flame Sprites")]
        [SerializeField, Tooltip("Flame animation frames.")]
        private Sprite[] flameFrames;

        [SerializeField, Tooltip("Animation frame rate.")]
        private float frameRate = 8.0f;

        [Header("Flicker Light")]
        [SerializeField, Tooltip("Optional Light2D component for dynamic warm fire light.")]
        private UnityEngine.Rendering.Universal.Light2D fireLight;

        [SerializeField] private float minIntensity = 0.8f;
        [SerializeField] private float maxIntensity = 1.3f;
        [SerializeField] private float flickerSpeed = 10.0f;

        private SpriteRenderer spriteRenderer;
        private float animTimer;
        private int currentFrame;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (fireLight == null)
            {
                fireLight = GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>();
            }
        }

        private void Update()
        {
            if (flameFrames != null && flameFrames.Length > 1)
            {
                animTimer += Time.deltaTime * frameRate;
                if (animTimer >= 1.0f)
                {
                    animTimer = 0f;
                    currentFrame = (currentFrame + 1) % flameFrames.Length;
                    spriteRenderer.sprite = flameFrames[currentFrame];
                }
            }

            if (fireLight != null)
            {
                float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f);
                fireLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
            }
        }
    }
}
