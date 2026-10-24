using UnityEngine;

namespace LittleElsewhere.Player
{
    /// <summary>
    /// Smooth 8-direction top-down movement controller for the orange-coated protagonist.
    /// Manages velocity smoothing, input direction, visual orientation, and movement state.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField, Tooltip("Movement speed in world units per second.")]
        private float moveSpeed = 3.5f;

        [SerializeField, Tooltip("Acceleration smoothing speed.")]
        private float acceleration = 14.0f;

        [SerializeField, Tooltip("Deceleration smoothing speed when releasing controls.")]
        private float deceleration = 18.0f;

        [Header("Visual & Facing")]
        [SerializeField, Tooltip("Transform container for character sprite visuals.")]
        private Transform visualContainer;

        [SerializeField, Tooltip("Reference to SpriteRenderer for horizontal flip.")]
        private SpriteRenderer spriteRenderer;

        [SerializeField, Tooltip("Enable 8-direction rotation of visual container instead of sprite flipping.")]
        private bool useFullDirectionalRotation = false;

        private Rigidbody2D rb;
        private Vector2 inputVector;
        private Vector2 targetVelocity;
        private Vector2 currentVelocity;

        public Vector2 CurrentVelocity => currentVelocity;
        public bool IsMoving => currentVelocity.sqrMagnitude > 0.01f;
        public Vector2 InputDirection => inputVector;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            if (spriteRenderer == null && visualContainer != null)
            {
                spriteRenderer = visualContainer.GetComponent<SpriteRenderer>();
            }
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
        }

        private void Update()
        {
            // Read standard 8-direction input
            float rawX = Input.GetAxisRaw("Horizontal");
            float rawY = Input.GetAxisRaw("Vertical");

            inputVector = new Vector2(rawX, rawY);
            if (inputVector.sqrMagnitude > 1f)
            {
                inputVector.Normalize();
            }

            targetVelocity = inputVector * moveSpeed;
            UpdateVisualFacing();
        }

        private void FixedUpdate()
        {
            // Smooth acceleration and deceleration for organic motion feel
            float rate = targetVelocity.sqrMagnitude > 0.01f ? acceleration : deceleration;
            currentVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * rate);
            rb.linearVelocity = currentVelocity;
        }

        private void UpdateVisualFacing()
        {
            if (inputVector.sqrMagnitude < 0.01f) return;

            if (useFullDirectionalRotation && visualContainer != null)
            {
                float angle = Mathf.Atan2(inputVector.y, inputVector.x) * Mathf.Rad2Deg - 90f;
                visualContainer.rotation = Quaternion.Euler(0, 0, angle);
            }
            else if (spriteRenderer != null)
            {
                if (inputVector.x < -0.1f)
                {
                    spriteRenderer.flipX = true;
                }
                else if (inputVector.x > 0.1f)
                {
                    spriteRenderer.flipX = false;
                }
            }
        }
    }
}
