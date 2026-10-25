using UnityEngine;
public class LECamera : MonoBehaviour {public Transform target;public float smooth=4f;void LateUpdate(){if(target){var p=target.position;p.z=-10;transform.position=Vector3.Lerp(transform.position,p,Time.deltaTime*smooth);}}}
