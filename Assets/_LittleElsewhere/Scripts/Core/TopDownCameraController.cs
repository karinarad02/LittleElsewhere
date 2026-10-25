using UnityEngine;

namespace LittleElsewhere.Core
{
    /// <summary>
    /// Fixed top-down overhead orthographic camera controller.
    /// Smoothly dampens movement tracking to follow the player while preserving framing and visual composition.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class TopDownCameraController : MonoBehaviour
    {
        [Header("Target & Follow Settings")]
        [SerializeField, Tooltip("Target transform to follow (Player character).")]
        private Transform target;

        [SerializeField, Tooltip("Smooth dampening duration for camera movement.")]
        private float smoothTime = 0.25f;

        [SerializeField, Tooltip("Camera position offset relative to target.")]
        private Vector3 positionOffset = new Vector3(0, 0, -10f);

        [Header("Orthographic Projection")]
        [SerializeField, Tooltip("Orthographic camera view size (viewport height control).")]
        private float orthographicSize = 6.5f;

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
            if (target == null)
            {
                GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
                if (playerGO != null)
                {
                    target = playerGO.transform;
                }
                else return;
            }

            Vector3 targetPosition = target.position + positionOffset;
            if (smoothTime <= 0.001f)
            {
                transform.position = targetPosition;
            }
            else
            {
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
            }
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}
