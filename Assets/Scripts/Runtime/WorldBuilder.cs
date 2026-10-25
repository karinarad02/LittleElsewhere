using System;
using System.Collections.Generic;
using UnityEngine;
namespace LittleElsewhere
{
    public enum Kind { Scenery, Cabin, Wood, Berries, Wildlife, Landmark, Note, Lantern, Fishing, Release, Camp }
    [System.Serializable]
    public sealed class WorldItem
    {
        public string id, title, text, species;
        public Kind kind;
        public Vector2 anchor, origin;
        public SpriteRenderer renderer, glow;
        public float radius, height, wanderPhase;
        public bool solid, hidden;
    }
    public sealed class WorldBuilder : MonoBehaviour
    {
        public readonly List<WorldItem> items = new List<WorldItem>();
        public static readonly string[] PlaceNames = { "The Cabin", "Glasswater Pond", "The High Clearing", "The Listening Grove", "Lantern Crossing" };
        public static readonly Vector2[] Places = { new Vector2(0,0), new Vector2(19,9), new Vector2(-22,20), new Vector2(-23,-15), new Vector2(23,-17) };
        public static readonly string[] NoteTitles = { "A pocket full of winter", "The long way home", "Small kindnesses" };
        public static readonly string[] NoteTexts = {
            "I used to hurry through this forest. Today a deer waited beside the path, and I waited too. Nothing happened. It was the best part of my day.",
            "The paths here have a way of bringing you back. You don't have to understand every turn. Find a familiar tree. Listen for the water. There is always another way home.",
            "A little wood by the door. Berries left beneath a branch. A lamp for whoever passes next. Perhaps a place becomes home because of what we leave for others."
        };
        public static readonly Vector2 CampPosition = new Vector2(-8,-20);
        public List<SpriteRenderer> Ground {get{return ground;}}
        GameSession game;
        Transform terrain;
        readonly List<SpriteRenderer> ground = new List<SpriteRenderer>();
        System.Random random;
        public void Build(GameSession session)
        {
            game = session; random = new System.Random(game.config.seed);
            terrain = new GameObject("Snow • repeating watercolor ground").transform; terrain.SetParent(transform);
            for (int x = -5; x <= 5; x++) for (int y = -4; y <= 4; y++)
            {
                var r = Art.Renderer("snow", terrain, 4, -30000); ground.Add(r);
                // Set tile dimensions exactly to avoid cracks.
                r.transform.localScale = new Vector3(4 / r.sprite.bounds.size.x, 4 / r.sprite.bounds.size.y, 1);
            }
            Add("home", Kind.Cabin, "cabin", Places[0], 5.6f, "The Cabin", "Warm the room, fill the kettle, and make something to remember your walk.", 1.25f, true);
            Add("camp",Kind.Camp,"camp",CampPosition,3.9f,"Rest at the campsite","Rest beside the fire and let eight hours pass.",.8f,true);
            Add("shed",Kind.Scenery,"shed",new Vector2(4,2),2.1f,"", "",.65f,true);
            Add("woodstore",Kind.Scenery,"woodstore",new Vector2(-4,1),1.7f,"", "",.5f,true);
            Add("bench",Kind.Scenery,"bench",new Vector2(-3,-3),1.2f,"", "",.35f,true);
            Add("pond",Kind.Scenery,"lake",Places[1],9,"", "",0,false).renderer.sortingOrder=-29000;
            Add("dock",Kind.Scenery,"dock",Places[1]+new Vector2(0,-3.2f),2.2f,"", "",0,false);
            Add("fish",Kind.Fishing,"boat",Places[1]+new Vector2(2,-3),1.8f,"Fish at Glasswater", "Wait for the float to reach the blue band, then press SPACE.",0,false);
            Add("crane",Kind.Release,"bench",Places[2],1.6f,"The High Clearing", "A place to let a little paper bird take flight.",.3f,true);
            Add("grove",Kind.Scenery,"bare_0",Places[3],3.5f,"", "",.3f,true);
            Add("crossing",Kind.Scenery,"dock",Places[4],3,"", "",0,false);
            for (int i=0;i<5;i++)
            {
                Add("place"+i,Kind.Landmark,"sign",Places[i]+new Vector2(-3,-1.4f),1.2f,PlaceNames[i],"A quiet place, just for a little while.",.15f,true);
            }
            Vector2[] notes={Places[1]+new Vector2(-4,2),Places[2]+new Vector2(2,-1),Places[3]+new Vector2(2,-2)};
            for(int i=0;i<3;i++) Add("note"+i,Kind.Note,"icon_journal",notes[i],.65f,NoteTitles[i],NoteTexts[i],0,false);
            for(int i=0;i<3;i++) Add("lamp"+i,Kind.Lantern,"lantern",new[]{Places[4]+new Vector2(-2,0),Places[3]+new Vector2(3,2),Places[2]+new Vector2(-3,1)}[i],1.65f,"Light the trail lantern","A small light for the next person on the path.",.2f,true);
            Vector2[] wood={new Vector2(6,-5),new Vector2(-9,5),new Vector2(12,3),new Vector2(-15,10),new Vector2(18,-10),new Vector2(-19,-8),new Vector2(29,5),new Vector2(5,17)};
            for(int i=0;i<wood.Length;i++) Add("wood"+i,Kind.Wood,"logs",wood[i],1,"Gather fallen firewood","Dry wood for a warm evening.",0,false);
            Vector2[] berries={new Vector2(-7,-6),new Vector2(10,14),new Vector2(-17,17),new Vector2(19,-20),new Vector2(-28,-12),new Vector2(6,-16)};
            for(int i=0;i<berries.Length;i++) Add("berry"+i,Kind.Berries,"berries",berries[i],1.05f,"Gather winter berries","A few berries for the kettle. You leave plenty for the birds.",0,false);
            string[] species={"fox","deer","rabbit","squirrel","bird","owl"};
            Vector2[] homes={new Vector2(11,-7),new Vector2(-13,13),new Vector2(-11,-11),new Vector2(24,17),new Vector2(-25,-19),new Vector2(-26,23)};
            for(int i=0;i<species.Length;i++)
            {
                var animal=Add("animal"+i,Kind.Wildlife,species[i]+"_0",homes[i],species[i]=="deer"?1.65f:.9f,"Observe the "+species[i],"",0,false);
                animal.species=species[i]; animal.wanderPhase=(float)random.NextDouble()*6.28f;
            }
            for(int i=0;i<game.config.treeCount;i++)
            {
                var p=RandomPoint(); if (Reserved(p,3.5f)||OnTrail(p,1.6f)) continue;
                Add("tree"+i,Kind.Scenery,"pine_"+random.Next(5),p,Range(2.6f,5.5f),"","",.36f,true);
            }
            for(int i=0;i<68;i++)
            {
                var p=RandomPoint(); if(Reserved(p,2.5f)||OnTrail(p,1f))continue;
                bool rock=i%3==0;
                Add("detail"+i,Kind.Scenery,rock?"rock_"+random.Next(3):(i%2==0?"bare_1":"bush"),p,rock?Range(.6f,1.35f):Range(.7f,1.8f),"","",rock?.3f:.12f,rock);
            }
            // A dotted snowy route joins landmarks, with periodic signposts and bare trees.
            for(int i=0;i<5;i++)
            {
                Vector2 dest=Places[(i+1)%5], start=Places[i];
                for(int j=1;j<8;j++)
                {
                    Vector2 p=Vector2.Lerp(start,dest,j/8f);
                    var r=Add("trail"+i+"_"+j,Kind.Scenery,"footprint",p,.25f,"","",0,false);
                    r.renderer.color=new Color(1,1,1,.4f);r.renderer.sortingOrder=-28500;
                }
            }
            Refresh(); Render(0);
        }
        float Range(float min,float max){return min+(float)random.NextDouble()*(max-min);}
        Vector2 RandomPoint(){return new Vector2(Range(-game.config.worldSize.x/2,game.config.worldSize.x/2),Range(-game.config.worldSize.y/2,game.config.worldSize.y/2));}
        bool Reserved(Vector2 p,float radius)
        {
            foreach(var item in items) if(item.kind!=Kind.Scenery && Vector2.Distance(p,item.anchor)<radius)return true;
            return Vector2.Distance(p,Places[1])<7 || Vector2.Distance(p,Vector2.zero)<7;
        }
        static float SegmentDistance(Vector2 p,Vector2 a,Vector2 b)
        {
            Vector2 d=b-a; float t=Mathf.Clamp01(Vector2.Dot(p-a,d)/d.sqrMagnitude);return Vector2.Distance(p,a+d*t);
        }
        bool OnTrail(Vector2 p,float width)
        {
            for(int i=0;i<5;i++) if(SegmentDistance(p,Places[i],Places[(i+1)%5])<width)return true;
            return false;
        }
        WorldItem Add(string id,Kind kind,string art,Vector2 p,float height,string title,string text,float radius,bool solid)
        {
            var item=new WorldItem{id=id,kind=kind,anchor=p,origin=p,height=height,title=title,text=text,radius=radius,solid=solid};
            item.renderer=Art.Renderer(art,transform,height);item.renderer.gameObject.name=id+" • "+art;
            if(kind==Kind.Lantern || kind==Kind.Cabin || kind==Kind.Camp)
            {
                item.glow=Art.Renderer("glow",transform,kind==Kind.Cabin?5:3);item.glow.sortingOrder=14000;
                item.glow.color=new Color(1,1,1,.6f);
            }
            items.Add(item);return item;
        }
        public void Refresh()
        {
            foreach(var item in items)
            {
                item.hidden=game.data.collected.Contains(item.id);
                item.renderer.enabled=!item.hidden;
                if(item.glow!=null) item.glow.enabled=item.kind==Kind.Cabin || item.kind==Kind.Camp || game.data.lanterns.Contains(item.id);
            }
        }
        public static float Wrap(float value,float length){return Mathf.Repeat(value+length*.5f,length)-length*.5f;}
        public Vector2 Delta(Vector2 a,Vector2 b)
        {
            Vector2 d=a-b;return new Vector2(Wrap(d.x,game.config.worldSize.x),Wrap(d.y,game.config.worldSize.y));
        }
        public Vector2 Resolve(Vector2 candidate)
        {
            foreach(var item in items)
            {
                if(!item.solid||item.hidden)continue;
                Vector2 d=Delta(candidate,item.anchor);float radius=item.radius+.23f;
                if(d.sqrMagnitude<radius*radius)candidate+=d.sqrMagnitude>.00001f?d.normalized*(radius-d.magnitude):Vector2.down*radius;
            }
            return candidate;
        }
        public WorldItem Nearest()
        {
            WorldItem best=null;float closest=2.1f;
            foreach(var item in items)
            {
                if(item.kind==Kind.Scenery || item.hidden)continue;
                if(item.kind==Kind.Landmark && game.data.discovered.Contains(item.id))continue;
                if(item.kind==Kind.Wildlife && game.data.wildlife.Contains(item.species))continue;
                if(item.kind==Kind.Note && game.data.notes.Contains(item.id))continue;
                if(item.kind==Kind.Lantern && game.data.lanterns.Contains(item.id))continue;
                float d=Delta(item.anchor,game.player.Position).magnitude;
                float reach=item.kind==Kind.Cabin?3.2f:item.kind==Kind.Wildlife?3f:2.1f;
                if(d<reach && (best==null||d<closest)){best=item;closest=d;}
            }
            return best;
        }
        public void Render(float dt)
        {
            var pos=game.player.Position;
            int tile=0;float gx=Mathf.Floor(pos.x/4)*4,gy=Mathf.Floor(pos.y/4)*4;
            for(int x=-5;x<=5;x++)for(int y=-4;y<=4;y++) ground[tile++].transform.position=new Vector3(gx+x*4,gy+y*4,0);
            foreach(var item in items)
            {
                if(item.kind==Kind.Wildlife && dt>0)
                {
                    float t=game.data.elapsed*.22f+item.wanderPhase;
                    Vector2 target=item.origin+new Vector2(Mathf.Sin(t),Mathf.Cos(t*.7f))*1.2f;
                    item.anchor=Vector2.MoveTowards(item.anchor,target,dt*.45f);
                    item.renderer.flipX=Mathf.Cos(t)<0;
                }
                Vector2 p=pos+Delta(item.anchor,pos);
                item.renderer.transform.position=p;
                if(item.id!="pond" && !item.id.StartsWith("trail"))item.renderer.sortingOrder=Mathf.RoundToInt(-p.y*100);
                if(item.glow!=null)item.glow.transform.position=p+Vector2.up*(item.kind==Kind.Cabin?1.4f:.5f);
            }
        }
    }
}
