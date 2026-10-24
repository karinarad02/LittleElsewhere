using UnityEngine;
using LittleElsewhere.Player;

namespace LittleElsewhere.Core
{
    /// <summary>
    /// Smooth orthographic camera controller supporting wraparound world shifts.
    /// Responds to player world wrapping events to shift camera target without visual pop.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class WrappedCameraController : MonoBehaviour
    {
        [Header("Target & Follow")]
        [SerializeField] private Transform target;
        [SerializeField] private WrappedPlayerController wrappedPlayer;
        [SerializeField] private float smoothTime = 0.25f;
        [SerializeField] private Vector3 positionOffset = new Vector3(0, 0, -10f);

        private Camera cam;
        private Vector3 currentVelocity;

        private void Awake()
        {
            cam = GetComponent<Camera>();
            cam.orthographic = true;
        }

        private void OnEnable()
        {
            if (wrappedPlayer != null)
            {
                wrappedPlayer.OnWorldWrapped += HandlePlayerWrapped;
            }
        }

        private void OnDisable()
        {
            if (wrappedPlayer != null)
            {
                wrappedPlayer.OnWorldWrapped -= HandlePlayerWrapped;
            }
        }

        private void HandlePlayerWrapped(Vector2 deltaShift)
        {
            // Instantly apply same coordinate shift to camera to preserve relative offset and eliminate camera pop
            transform.position += (Vector3)deltaShift;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 targetPosition = target.position + positionOffset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
        }
    }
}
