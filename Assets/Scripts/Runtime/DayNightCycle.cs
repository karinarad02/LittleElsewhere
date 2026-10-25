using System.Collections.Generic;
using UnityEngine;
namespace LittleElsewhere
{
    /// <summary>Continuous 24-hour palette and two independently projected sprite shadows.</summary>
    public sealed class DayNightCycle : MonoBehaviour
    {
        public struct Phase
        {
            public Color tint, shadow;
            public Vector2 sunDirection,moonDirection;
            public float sunLength, moonLength, sunOpacity,moonOpacity,night;
        }
        sealed class Target
        {
            public SpriteRenderer source,sun,moon;
            public Color original;
            public bool warm,ground;
            public MaterialPropertyBlock sunBlock=new MaterialPropertyBlock(),moonBlock=new MaterialPropertyBlock();
        }
        public GameSession game;
        public Phase Current {get;private set;}
        readonly List<Target> targets=new List<Target>();
        Material shadowMaterial;
        public float Hour {get{return game==null||game.data==null?9:(float)(game.data.worldHours%24);}}
        public string Clock {get {int minutes=Mathf.FloorToInt(Hour*60);return (minutes/60).ToString("00")+":"+(minutes%60).ToString("00");}}
        public string Period
        {
            get{float h=Hour;return h<5?"Night":h<8?"Dawn":h<17?"Day":h<20?"Dusk":"Night";}
        }
        public static Phase Evaluate(float hour)
        {
            hour=Mathf.Repeat(hour,24);
            Color midnight=new Color(.26f,.34f,.55f),dawn=new Color(.92f,.75f,.72f),day=new Color(.98f,.99f,1),dusk=new Color(.96f,.66f,.52f);
            Color tint;
            if(hour<5)tint=midnight;
            else if(hour<7)tint=Color.Lerp(midnight,dawn,(hour-5)/2);
            else if(hour<10)tint=Color.Lerp(dawn,day,(hour-7)/3);
            else if(hour<16)tint=day;
            else if(hour<18.5f)tint=Color.Lerp(day,dusk,(hour-16)/2.5f);
            else if(hour<21)tint=Color.Lerp(dusk,midnight,(hour-18.5f)/2.5f);
            else tint=midnight;
            float sunAltitude=Mathf.Sin((hour-6)/12*Mathf.PI);
            float moonAltitude=-sunAltitude;
            float daylight=Mathf.SmoothStep(0,1,Mathf.InverseLerp(-.12f,.3f,sunAltitude));
            float angle=(hour-6)/12*Mathf.PI;
            return new Phase{
                tint=tint,shadow=Color.Lerp(new Color(.17f,.28f,.48f),new Color(.32f,.49f,.65f),daylight),night=1-daylight,
                sunDirection=new Vector2(Mathf.Cos(angle),-.42f).normalized,
                moonDirection=new Vector2(-Mathf.Cos(angle),-.35f).normalized,
                sunLength=Mathf.Lerp(2.2f,.35f,Mathf.Clamp01(sunAltitude)),
                moonLength=Mathf.Lerp(1.4f,.7f,Mathf.Clamp01(moonAltitude)),
                sunOpacity=.27f*daylight,moonOpacity=.12f*(1-daylight)
            };
        }
        public void Bind(GameSession session)
        {
            game=session;targets.Clear();
            if(shadowMaterial==null)
            {
                var shader=Resources.Load<Shader>("Lighting/ProjectedShadow");
                if(shader!=null)shadowMaterial=new Material(shader);
                else Debug.LogError("Projected shadow shader is missing");
            }
            foreach(var ground in game.world.Ground)Add(ground,false,false,true);
            foreach(var item in game.world.items)
            {
                bool cast=item.id!="pond" && !item.id.StartsWith("trail");
                if(item.id.StartsWith("trail"))cast=false;
                Add(item.renderer,cast,false);
                if(item.glow!=null)Add(item.glow,false,true);
            }
            Add(game.player.Body,true,false);
            Apply();
        }
        public void RegisterTrack(SpriteRenderer source){Add(source,false,false);}
        void Add(SpriteRenderer source,bool cast,bool warm,bool ground=false)
        {
            var target=new Target{source=source,original=source.color,warm=warm,ground=ground};
            if(cast && shadowMaterial!=null)
            {
                target.sun=Shadow(source,"Sun shadow");target.moon=Shadow(source,"Moon shadow");
            }
            targets.Add(target);
        }
        SpriteRenderer Shadow(SpriteRenderer source,string name)
        {
            var go=new GameObject(name);go.transform.SetParent(source.transform,false);
            var renderer=go.AddComponent<SpriteRenderer>();renderer.sharedMaterial=shadowMaterial;
            renderer.sortingOrder=-25000;
            return renderer;
        }
        public void Tick(float dt)
        {
            if(game==null||game.data==null)return;
            game.data.worldHours+=dt*24/System.Math.Max(1,game.config.dayLengthSeconds);
            game.data.day=1+(int)System.Math.Floor(game.data.worldHours/24);
        }
        public void AdvanceHours(float hours)
        {
            game.data.worldHours+=Mathf.Max(0,hours);game.data.day=1+(int)System.Math.Floor(game.data.worldHours/24);Apply();
        }
        void LateUpdate(){if(game!=null&&game.data!=null)Apply();}
        public void Apply()
        {
            if(game==null||game.data==null)return;
            Current=Evaluate(Hour);
            foreach(var target in targets)
            {
                if(target.source==null)continue;
                Color local=Current.tint;
                if(target.warm)
                {
                    target.source.color=target.original*new Color(1,1,1,Mathf.Lerp(.35f,1,Current.night));
                    continue;
                }
                // Local orange light keeps the tent and cabin welcoming after sunset.
                if(Current.night>.02f && !target.ground)
                {
                    Vector2 p=target.source.transform.position;
                    float distance=Mathf.Min(game.world.Delta(p,Vector2.zero).magnitude,game.world.Delta(p,WorldBuilder.CampPosition).magnitude);
                    float warmth=Mathf.Clamp01(1-distance/5)*Current.night*.8f;
                    local=Color.Lerp(local,new Color(1,.74f,.48f),warmth);
                }
                target.source.color=target.original*local;
                if(target.sun!=null)
                {
                    UpdateShadow(target.source,target.sun,target.sunBlock,Current.sunDirection*Current.sunLength,Current.shadow,Current.sunOpacity);
                    UpdateShadow(target.source,target.moon,target.moonBlock,Current.moonDirection*Current.moonLength,new Color(.16f,.23f,.4f),Current.moonOpacity);
                }
            }
            game.cam.backgroundColor=new Color(.89f,.92f,.94f)*Current.tint;
        }
        static void UpdateShadow(SpriteRenderer source,SpriteRenderer shadow,MaterialPropertyBlock block,Vector2 vector,Color tint,float opacity)
        {
            shadow.enabled=source.enabled && opacity>.001f;
            if(!shadow.enabled||source.sprite==null)return;
            shadow.sprite=source.sprite;shadow.flipX=source.flipX;shadow.flipY=source.flipY;
            tint.a=opacity;shadow.color=Color.white;
            block.SetFloat("_BaseY",source.sprite.bounds.min.y);
            block.SetVector("_ShadowVector",new Vector4(vector.x,vector.y,0,0));
            block.SetColor("_ShadowTint",tint);shadow.SetPropertyBlock(block);
        }
        void OnDestroy(){if(shadowMaterial!=null){if(Application.isPlaying)Destroy(shadowMaterial);else DestroyImmediate(shadowMaterial);}}
    }
}
