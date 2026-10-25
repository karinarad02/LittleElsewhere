using UnityEngine;

namespace LittleElsewhere.Player
{
    /// <summary>
    /// Animates player character sprites in 8 directions (North, South, East, West, NE, NW, SE, SW)
    /// with idle and walking steps matching the storybook watercolor aesthetic.
    /// Supports automatic fallback to cardinal directions if diagonal sprite arrays are unassigned.
    /// </summary>
    [RequireComponent(typeof(PlayerController))]
    public class PlayerAnimationController : MonoBehaviour
    {
        [Header("8-Directional Sprites")]
        [SerializeField, Tooltip("North / Up / Back facing sprites.")]
        private Sprite[] northSprites;

        [SerializeField, Tooltip("South / Down / Front facing sprites.")]
        private Sprite[] southSprites;

        [SerializeField, Tooltip("East / Right facing sprites.")]
        private Sprite[] eastSprites;

        [SerializeField, Tooltip("West / Left facing sprites.")]
        private Sprite[] westSprites;

        [SerializeField, Tooltip("Northeast facing sprites.")]
        private Sprite[] northEastSprites;

        [SerializeField, Tooltip("Northwest facing sprites.")]
        private Sprite[] northWestSprites;

        [SerializeField, Tooltip("Southeast facing sprites.")]
        private Sprite[] southEastSprites;

        [SerializeField, Tooltip("Southwest facing sprites.")]
        private Sprite[] southWestSprites;

        [Header("Animation Tuning")]
        [SerializeField, Tooltip("Footstep animation speed in steps per second.")]
        private float stepRate = 6.0f;

        public enum Direction8
        {
            North,
            South,
            East,
            West,
            NorthEast,
            NorthWest,
            SouthEast,
            SouthWest
        }

        private PlayerController playerController;
        private SpriteRenderer spriteRenderer;
        private Direction8 currentFacing = Direction8.North;
        private float animTimer;
        private int activeWalkFrame = 1;

        public Direction8 CurrentFacing => currentFacing;

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Update()
        {
            Vector2 input = playerController.InputDirection;
            bool isMoving = playerController.IsMoving;

            if (input.sqrMagnitude > 0.05f)
            {
                currentFacing = Calculate8Direction(input);
            }

            Sprite[] targetSpriteSet = GetSpriteSet(currentFacing);
            if (targetSpriteSet == null || targetSpriteSet.Length == 0) return;

            if (!isMoving)
            {
                animTimer = 0f;
                activeWalkFrame = 1;
                if (targetSpriteSet.Length > 0 && targetSpriteSet[0] != null)
                {
                    spriteRenderer.sprite = targetSpriteSet[0]; // Idle pose
                }
            }
            else
            {
                animTimer += Time.deltaTime * stepRate;
                if (animTimer >= 1.0f)
                {
                    animTimer = 0f;
                    activeWalkFrame = (activeWalkFrame == 1) ? 2 : 1;
                }

                int frameIndex = (targetSpriteSet.Length > activeWalkFrame && targetSpriteSet[activeWalkFrame] != null) 
                    ? activeWalkFrame 
                    : 0;

                spriteRenderer.sprite = targetSpriteSet[frameIndex];
            }
        }

        public static Direction8 Calculate8Direction(Vector2 dir)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;

            // 8 sectors of 45 degrees each centered around cardinal & intercardinal angles
            if (angle >= 22.5f && angle < 67.5f) return Direction8.NorthEast;
            if (angle >= 67.5f && angle < 112.5f) return Direction8.North;
            if (angle >= 112.5f && angle < 157.5f) return Direction8.NorthWest;
            if (angle >= 157.5f && angle < 202.5f) return Direction8.West;
            if (angle >= 202.5f && angle < 247.5f) return Direction8.SouthWest;
            if (angle >= 247.5f && angle < 292.5f) return Direction8.South;
            if (angle >= 292.5f && angle < 337.5f) return Direction8.SouthEast;

            return Direction8.East;
        }

        private Sprite[] GetSpriteSet(Direction8 direction)
        {
            Sprite[] set = direction switch
            {
                Direction8.North => northSprites,
                Direction8.South => southSprites,
                Direction8.East => eastSprites,
                Direction8.West => westSprites,
                Direction8.NorthEast => (northEastSprites != null && northEastSprites.Length > 0) ? northEastSprites : eastSprites,
                Direction8.NorthWest => (northWestSprites != null && northWestSprites.Length > 0) ? northWestSprites : westSprites,
                Direction8.SouthEast => (southEastSprites != null && southEastSprites.Length > 0) ? southEastSprites : eastSprites,
                Direction8.SouthWest => (southWestSprites != null && southWestSprites.Length > 0) ? southWestSprites : westSprites,
                _ => northSprites
            };

            return (set != null && set.Length > 0) ? set : northSprites;
        }
    }
}
