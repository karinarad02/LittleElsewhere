using UnityEngine;
using LittleElsewhere.World;

namespace LittleElsewhere.Player
{
    /// <summary>
    /// Player controller with wraparound coordinate wrapping.
    /// When the player crosses world boundaries [0, worldSize.x) or [0, worldSize.y),
    /// position is wrapped canonically while preserving smooth velocity and camera continuity.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class WrappedPlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 3.5f;
        [SerializeField] private float acceleration = 14.0f;

        [Header("World Configuration")]
        [SerializeField] private WorldSeedData worldConfig;

        private Rigidbody2D rb;
        private Vector2 inputVector;
        private Vector2 targetVelocity;
        private Vector2 currentVelocity;

        public event System.Action<Vector2> OnWorldWrapped;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        private void Update()
        {
            float rawX = Input.GetAxisRaw("Horizontal");
            float rawY = Input.GetAxisRaw("Vertical");

            inputVector = new Vector2(rawX, rawY);
            if (inputVector.sqrMagnitude > 1f) inputVector.Normalize();

            targetVelocity = inputVector * moveSpeed;
        }

        private void FixedUpdate()
        {
            currentVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * acceleration);
            rb.linearVelocity = currentVelocity;

            CheckAndApplyWorldWrapping();
        }

        private void CheckAndApplyWorldWrapping()
        {
            if (worldConfig == null) return;

            Vector2 worldSize = worldConfig.WorldSize;
            Vector2 currentPos = rb.position;
            Vector2 wrappedPos = ToroidalMath.WrapPosition(currentPos, worldSize);

            if (currentPos != wrappedPos)
            {
                Vector2 deltaShift = wrappedPos - currentPos;
                rb.position = wrappedPos;
                transform.position = wrappedPos;

                OnWorldWrapped?.Invoke(deltaShift);
            }
        }

        public void SetWorldConfig(WorldSeedData config)
        {
            worldConfig = config;
        }
    }
}
