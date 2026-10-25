using UnityEngine;
public class LEPlayer : MonoBehaviour {
 public float speed=3f; public float stepDistance=.42f;
 Rigidbody2D body; SpriteRenderer sprite; Vector2 movement; Vector3 lastStep;
 Sprite[] down,up,left,right; float animClock;
 void Awake(){body=GetComponent<Rigidbody2D>();sprite=GetComponent<SpriteRenderer>();down=Load("WalkDown");up=Load("WalkUp");left=Load("WalkLeft");right=Load("WalkRight");lastStep=transform.position;}
 Sprite[] Load(string n){var s=new Sprite[4];for(int i=0;i<4;i++)s[i]=Resources.Load<Sprite>("Player/"+n+"_"+i.ToString("00"));return s;}
 void Update(){if(LEGame.Paused){movement=Vector2.zero;return;}movement=new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical")).normalized;animClock+=Time.deltaTime*7;var arr=movement.y>0.1?up:movement.y<-.1?down:movement.x<0?left:right;if(movement.sqrMagnitude>.01f)sprite.sprite=arr[Mathf.FloorToInt(animClock)%4];else sprite.sprite=Resources.Load<Sprite>("Player/Idle_00");if(Input.GetKeyDown(KeyCode.E))LEGame.Message="A quiet place, just for a little while.";}
 void FixedUpdate(){if(!LEGame.Paused)body.MovePosition(body.position+movement*speed*Time.fixedDeltaTime);if(Vector3.Distance(lastStep,transform.position)>stepDistance){var foot=Resources.Load<Sprite>("Props/Footprints"); if(foot){var g=new GameObject("Footprint");g.transform.position=transform.position+Vector3.up*.2f;g.transform.localScale=Vector3.one*.17f;var r=g.AddComponent<SpriteRenderer>();r.sprite=foot;r.sortingOrder=-3;r.color=new Color(1,1,1,.32f);Destroy(g,18);}lastStep=transform.position;}}
}
