using System.Collections.Generic;
using UnityEngine;
namespace LittleElsewhere
{
    public sealed class PlayerController : MonoBehaviour
    {
        public Vector2 Position { get { return transform.position; } set { transform.position = value; } }
        GameSession game;
        SpriteRenderer body;
        public SpriteRenderer Body {get{return body;}}
        string facing="down";
        float animationTime,stepDistance;
        bool moving;
        int footprintIndex;
        readonly Queue<SpriteRenderer> footprints=new Queue<SpriteRenderer>();
        public void Setup(GameSession session)
        {
            game=session;body=Art.Renderer("player_idle_0",transform,.95f);body.transform.localPosition=new Vector3(0,0,0);
        }
        public void Tick(float dt)
        {
            float x=(Input.GetKey(KeyCode.D)||Input.GetKey(KeyCode.RightArrow)?1:0)-(Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.LeftArrow)?1:0);
            float y=(Input.GetKey(KeyCode.W)||Input.GetKey(KeyCode.UpArrow)?1:0)-(Input.GetKey(KeyCode.S)||Input.GetKey(KeyCode.DownArrow)?1:0);
            var input=new Vector2(x,y).normalized;
            float speed=Input.GetKey(KeyCode.LeftShift)||Input.GetKey(KeyCode.RightShift)?game.config.runSpeed:game.config.walkSpeed;
            // Ice gently carries your momentum without taking away control.
            float ice=game.world.Delta(Position,WorldBuilder.Places[1]).magnitude<3.7f?1.2f:1;
            Vector2 before=Position; Position=game.world.Resolve(Position+input*speed*ice*dt);
            moving=(Position-before).sqrMagnitude>.000001f;
            if(moving)
            {
                facing=Mathf.Abs(input.x)>Mathf.Abs(input.y)?(input.x>0?"right":"left"):(input.y>0?"up":"down");
                animationTime+=dt*(speed>4?11:8);stepDistance+=(Position-before).magnitude;
                if(stepDistance>.55f){stepDistance=0;game.sound.Step();if(game.preferences.footprints)Footprint();}
            }
            int frame=moving?(int)animationTime%4:0;
            body.sprite=Art.Sprite("player_"+(moving?facing:(facing=="down"?"idle":facing))+"_"+frame);
            body.sortingOrder=Mathf.RoundToInt(-Position.y*100)+1;
            // Keep long sessions close to the origin; the wrapped world moves continuously with us.
            if(Mathf.Abs(Position.x)>160||Mathf.Abs(Position.y)>128)
            {
                Vector2 shift=new Vector2(Mathf.Round(Position.x/game.config.worldSize.x)*game.config.worldSize.x,Mathf.Round(Position.y/game.config.worldSize.y)*game.config.worldSize.y);
                Position-=shift;game.cam.transform.position-=(Vector3)shift;
                foreach(var footprint in footprints)footprint.transform.position-=(Vector3)shift;
            }
        }
        void Footprint()
        {
            SpriteRenderer r;
            if(footprints.Count>=100)r=footprints.Dequeue(); else {r=Art.Renderer("footprint",game.world.transform,.16f,-28000);game.dayNight.RegisterTrack(r);}
            r.transform.position=Position+new Vector2((footprintIndex++%2==0?.1f:-.1f),0);
            r.transform.rotation=Quaternion.Euler(0,0,facing=="left"?90:facing=="right"?-90:facing=="up"?180:0);
            r.enabled=true;footprints.Enqueue(r);
        }
        public void ShowFootprints(bool show){foreach(var footprint in footprints)footprint.enabled=show;}
    }
}
