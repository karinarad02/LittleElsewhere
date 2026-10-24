using UnityEngine;

namespace LittleElsewhere.CameraSystem
{
    /// <summary>
    /// Smooth orthographic camera follower for top-down exploration.
    /// Dampens camera movement and maintains framing on the orange-coated protagonist.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class TopDownCameraController : MonoBehaviour
    {
        [Header("Target & Follow Settings")]
        [SerializeField, Tooltip("Target transform to follow (Player).")]
        private Transform target;

        [SerializeField, Tooltip("Smooth damp time for camera movement.")]
        private float smoothTime = 0.3f;

        [SerializeField, Tooltip("Fixed offset relative to target position.")]
        private Vector3 positionOffset = new Vector3(0, 0, -10f);

        [Header("Camera Settings")]
        [SerializeField, Tooltip("Orthographic camera size (viewport height control).")]
        private float orthographicSize = 7.0f;

        private Camera cam;
        private Vector3 currentVelocity;

        private void Awake()
        {
            cam = GetComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = orthographicSize;
        }

        private void Start()
        {
            if (target == null)
            {
                // Auto-find player by tag if not assigned
                GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
                if (playerGO != null)
                {
                    target = playerGO.transform;
                }
            }

            if (target != null)
            {
                transform.position = target.position + positionOffset;
            }
        }

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 targetPosition = target.position + positionOffset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}
