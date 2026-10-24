using UnityEngine;

namespace LittleElsewhere.Player
{
    /// <summary>
    /// Smooth top-down character controller for the orange-coated protagonist.
    /// Handles input processing, velocity smoothing, character rotation/facing, and movement states.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField, Tooltip("Base move speed in units per second.")]
        private float moveSpeed = 4.0f;

        [SerializeField, Tooltip("Acceleration and deceleration smoothing factor.")]
        private float accelerationSmoothing = 12.0f;

        [Header("Visual References")]
        [SerializeField, Tooltip("Transform of the sprite or visual model to rotate towards movement direction.")]
        private Transform visualContainer;

        [SerializeField, Tooltip("Enable sprite flipping along X axis for left/right facing.")]
        private bool flipSpriteFacing = true;

        [SerializeField, Tooltip("SpriteRenderer reference for flipping visuals.")]
        private SpriteRenderer characterSpriteRenderer;

        private Rigidbody2D rb;
        private Vector2 currentInput;
        private Vector2 targetVelocity;
        private Vector2 currentVelocity;

        public Vector2 CurrentVelocity => currentVelocity;
        public bool IsMoving => currentVelocity.sqrMagnitude > 0.01f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f; // Ensure top-down physics without gravity
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            if (characterSpriteRenderer == null && visualContainer != null)
            {
                characterSpriteRenderer = visualContainer.GetComponent<SpriteRenderer>();
            }
            if (characterSpriteRenderer == null)
            {
                characterSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
        }

        private void Update()
        {
            // Read standard horizontal and vertical axes (Keyboard WASD / Arrow Keys / Gamepad)
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");

            currentInput = new Vector2(moveX, moveY).normalized;
            targetVelocity = currentInput * moveSpeed;

            // Update visual facing direction
            UpdateVisualFacing();
        }

        private void FixedUpdate()
        {
            // Smooth velocity transition for organic, natural top-down movement feel
            currentVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * accelerationSmoothing);
            rb.linearVelocity = currentVelocity;
        }

        private void UpdateVisualFacing()
        {
            if (currentInput.sqrMagnitude < 0.01f) return;

            if (flipSpriteFacing && characterSpriteRenderer != null)
            {
                if (currentInput.x < -0.1f)
                {
                    characterSpriteRenderer.flipX = true;
                }
                else if (currentInput.x > 0.1f)
                {
                    characterSpriteRenderer.flipX = false;
                }
            }
            else if (visualContainer != null)
            {
                float angle = Mathf.Atan2(currentInput.y, currentInput.x) * Mathf.Rad2Deg - 90f;
                visualContainer.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }
}
