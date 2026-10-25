using UnityEngine;
public class LEAnimal : MonoBehaviour {
 public string species="Fox";public float roamRadius=2.5f,speed=.55f;
 Vector3 origin,target;float timer;SpriteRenderer sr;
 void Start(){origin=transform.position;target=origin;sr=GetComponent<SpriteRenderer>();}
 void Update(){if(LEGame.Paused)return;timer-=Time.deltaTime;if(timer<=0){target=origin+(Vector3)(Random.insideUnitCircle*roamRadius);timer=Random.Range(2f,5f);}transform.position=Vector3.MoveTowards(transform.position,target,speed*Time.deltaTime);if(sr)sr.flipX=target.x<transform.position.x;}
}
