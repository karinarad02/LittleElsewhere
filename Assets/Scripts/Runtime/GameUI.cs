using UnityEngine;
namespace LittleElsewhere
{
    public sealed class GameUI : MonoBehaviour
    {
        public GameSession game;
        GUIStyle heading,body,small,button,title,center;
        Texture2D white,landscape,paper;
        Vector2 journalScroll;
        bool styled;
        SaveData[] slotPreview=new SaveData[3];
        int pendingSlot=1;
        bool pendingSave;
        string slotsBack="Title";
        readonly Color ink=new Color(.16f,.29f,.37f);
        void Styles()
        {
            if(styled)return;styled=true;
            white=Texture2D.whiteTexture;landscape=Art.Texture("landscape");paper=Art.Texture("paper");
            Font font=Resources.Load<Font>("Fonts/ElsewhereSerif");
            heading=new GUIStyle(GUI.skin.label){fontSize=29,wordWrap=true};
            body=new GUIStyle(GUI.skin.label){fontSize=20,wordWrap=true};
            small=new GUIStyle(body){fontSize=16};
            center=new GUIStyle(body){alignment=TextAnchor.MiddleCenter};
            title=new GUIStyle(heading){fontSize=57,alignment=TextAnchor.MiddleCenter};
            button=new GUIStyle(GUI.skin.button){fontSize=20,alignment=TextAnchor.MiddleCenter,padding=new RectOffset(14,14,8,8)};
            foreach(var style in new[]{heading,body,small,center,title,button})
            {style.normal.textColor=ink;style.hover.textColor=ink;style.active.textColor=ink;if(font!=null)style.font=font;}
        }
        void Fill(Rect rect,Color color){Color old=GUI.color;GUI.color=color;GUI.DrawTexture(rect,white);GUI.color=old;}
        void Paper(Rect rect)
        {
            Fill(new Rect(rect.x+5,rect.y+7,rect.width,rect.height),new Color(.08f,.18f,.24f,.18f));
            if(paper!=null)GUI.DrawTexture(rect,paper,ScaleMode.StretchToFill);else Fill(rect,new Color(.96f,.95f,.91f));
            Fill(new Rect(rect.x+12,rect.y+12,rect.width-24,1),new Color(.4f,.52f,.58f,.2f));
        }
        bool Button(Rect rect,string label)
        {
            bool hover=rect.Contains(Event.current.mousePosition);
            Fill(rect,hover?new Color(.68f,.79f,.85f,.9f):new Color(.82f,.80f,.74f,.52f));
            var old=GUI.backgroundColor;GUI.backgroundColor=Color.clear;
            bool pressed=GUI.Button(rect,label,button);GUI.backgroundColor=old;
            return pressed;
        }
        void Label(float x,float y,float w,float h,string text,GUIStyle style=null){GUI.Label(new Rect(x,y,w,h),text,style??body);}
        void Icon(string name,Rect rect){var tex=Art.Texture(name);if(tex!=null)GUI.DrawTexture(rect,tex,ScaleMode.ScaleToFit);}
        void Settings(string back){game.returnPage=back;game.page="Settings";}
        void OnGUI()
        {
            if(game==null||game.cam==null)return;Styles();
            float scale=Mathf.Min(Screen.width/1280f,Screen.height/720f);
            // Letterbox UI only; world rendering keeps the native display resolution.
            GUI.matrix=Matrix4x4.TRS(new Vector3((Screen.width-1280*scale)/2,(Screen.height-720*scale)/2,0),Quaternion.identity,new Vector3(scale,scale,1));
            string page=game.page;
            if(page=="Title"||page=="ConfirmNew"||(page.StartsWith("Slots")&&slotsBack=="Title"))
            {
                if(landscape!=null)GUI.DrawTexture(new Rect(-500,-200,2280,1120),landscape,ScaleMode.ScaleAndCrop);
                Fill(new Rect(-500,-200,2280,1120),new Color(.9f,.95f,.98f,.15f));
            }
            if(game.preferences.snow) Snow();
            float darkness=Mathf.Clamp01(1-game.preferences.brightness);
            if(darkness>0)Fill(new Rect(-500,-200,2280,1120),new Color(.08f,.19f,.3f,darkness));
            if(page=="Title")Title();
            else if(page=="ConfirmNew")ConfirmNew();
            else
            {
                if(page=="Play")Hud();
                else
                {
                    Fill(new Rect(-500,-200,2280,1120),new Color(.09f,.19f,.25f,.28f));
                    if(page.StartsWith("Slots"))Slots();
                    else if(page=="Pause")Pause();
                    else if(page=="Settings")SettingsPanel();
                    else if(page=="Map")Map();
                    else if(page=="Journal")Journal();
                    else if(page=="Dialogue")Dialogue();
                    else if(page=="Fishing")Fishing();
                    else if(page=="Ending")Ending();
                }
            }
            if(Time.unscaledTime<game.toastUntil&&!string.IsNullOrEmpty(game.toast))
            {
                Paper(new Rect(310,650,660,48));Label(326,658,628,33,game.toast,center);
            }
            GUI.matrix=Matrix4x4.identity;
        }
        void Snow()
        {
            var t=Time.unscaledTime;var flake=Art.Texture("flake");if(flake==null)return;
            for(int i=0;i<70;i++)
            {
                float x=Mathf.Repeat(i*127.37f+Mathf.Sin(t*.3f+i)*17-t*(5+i%7),1380)-50;
                float y=Mathf.Repeat(i*73.17f+t*(12+i%9),800)-40;
                float s=3+i%4;GUI.DrawTexture(new Rect(x,y,s,s),flake);
            }
        }
        void Title()
        {
            Paper(new Rect(408,58,464,575));
            Label(440,88,400,150,"Little\nElsewhere",title);
            Label(446,245,388,55,"A quiet place, just for a little while.",center);
            bool exists=SaveSystem.Exists;
            GUI.enabled=exists;
            if(Button(new Rect(491,325,298,47),"Continue"))OpenSlots("SlotsLoad","Title");GUI.enabled=true;
            if(Button(new Rect(491,386,298,47),"New Walk"))OpenSlots("SlotsNew","Title");
            if(Button(new Rect(491,447,298,47),"Settings"))Settings("Title");
            if(Button(new Rect(491,508,298,47),"Quit"))Quit();
            Label(434,580,412,32,"An unhurried winter exploration",center);
            Label(35,680,1000,24,"WASD / arrows · E interact · M map · J journal · Esc pause",small);
        }
        public void OpenSlots(string mode,string back)
        {
            slotsBack=back;game.returnPage=back;
            for(int i=0;i<3;i++)slotPreview[i]=SaveSystem.PreviewSlot(i+1);
            game.page=mode;
        }
        void Slots()
        {
            bool load=game.page=="SlotsLoad",save=game.page=="SlotsSave";
            Paper(new Rect(238,47,804,594));
            Label(275,77,720,48,load?"Continue a winter walk":save?"Save your winter walk":"Choose a home for your walk",heading);
            Label(275,129,720,38,"Three separate saves · progress and time are kept in each slot",small);
            for(int i=0;i<3;i++)
            {
                int slot=i+1;float y=183+i*119;SaveData preview=slotPreview[i];
                Fill(new Rect(276,y,728,105),new Color(.7f,.78f,.79f,.2f));
                Icon(preview==null?"icon_pack":preview.craneMade?"icon_crane":"icon_journal",new Rect(289,y+15,70,70));
                Label(375,y+5,415,30,"Slot "+slot+"  —  "+(preview==null?(SaveSystem.ExistsSlot(slot)?"Unreadable save":"Empty slot"):(string.IsNullOrEmpty(preview.location)?"The Cabin":preview.location)),body);
                if(preview!=null)
                {
                    int minutes=Mathf.FloorToInt((float)(preview.worldHours%24)*60);
                    Label(375,y+39,420,50,"Day "+preview.day+"  ·  "+(minutes/60).ToString("00")+":"+(minutes%60).ToString("00")+"  ·  "+Mathf.FloorToInt(preview.elapsed/60)+" min played\n"+(preview.savedAt??"Earlier walk")+(preview.finished?"  ·  Story completed":""),small);
                }
                else Label(375,y+44,410,38,"Begin a new walk here.",small);
                GUI.enabled=!load||preview!=null;
                if(Button(new Rect(837,y+30,150,47),load?"Continue":save?"Save here":"New walk"))
                {
                    if(load){SaveSystem.SelectSlot(slot);game.ContinueGame();}
                    else if(SaveSystem.ExistsSlot(slot))
                    {pendingSlot=slot;pendingSave=save;game.page="ConfirmNew";}
                    else
                    {SaveSystem.SelectSlot(slot);if(save){game.Save();if(SaveSystem.LastError==null)game.page="Pause";}else game.NewGame();}
                }
                GUI.enabled=true;
            }
            Label(276,562,450,43,"Autosave updates only the active slot.",small);
            if(Button(new Rect(837,568,150,43),"Back"))game.page=slotsBack;
        }
        void ConfirmNew()
        {
            Paper(new Rect(380,192,520,325));Label(420,222,440,48,"Replace slot "+pendingSlot+"?",heading);
            Label(420,285,440,110,pendingSave?"Save your current walk into this slot? Its previous progress will be replaced. The other two saves will stay as they are.":"Begin a new walk in this slot? Its previous progress will be replaced. The other two saves will stay as they are.");
            if(Button(new Rect(420,425,205,49),"Keep existing"))OpenSlots(pendingSave?"SlotsSave":"SlotsNew",slotsBack);
            if(Button(new Rect(645,425,215,49),"Replace slot"))
            {SaveSystem.SelectSlot(pendingSlot);if(pendingSave){game.Save();if(SaveSystem.LastError==null)game.page="Pause";}else game.NewGame();}
        }
        void Hud()
        {
            Paper(new Rect(24,22,470,107));Label(43,33,425,37,"Little Elsewhere",heading);Label(43,80,425,35,game.Objective(),small);
            Paper(new Rect(521,24,400,48));Label(535,32,372,32,"Day "+game.data.day+"   "+game.dayNight.Clock+"   "+game.dayNight.Period+"   ·   Slot "+SaveSystem.CurrentSlot,center);
            if(Button(new Rect(951,24,92,48),"Map"))game.page="Map";
            if(Button(new Rect(1054,24,104,48),"Journal"))game.page="Journal";
            if(Button(new Rect(1170,24,85,48),"Pause"))game.page="Pause";
            if(game.nearby!=null)
            {
                Paper(new Rect(395,570,490,60));Label(413,582,454,39,"[E]  "+game.nearby.title,center);
            }
            Label(30,681,900,25,"WASD / arrows  Walk       Shift  Stroll faster       E  Interact       F5  Save",small);
            Icon(game.data.craneMade?"icon_crane":"icon_pack",new Rect(1185,596,64,70));
        }
        void Pause()
        {
            Paper(new Rect(430,86,420,528));Label(470,119,340,54,"A little pause",heading);
            if(Button(new Rect(485,204,310,48),"Resume"))game.page="Play";
            if(Button(new Rect(485,266,310,48),"Settings"))Settings("Pause");
            if(Button(new Rect(485,328,310,48),"Save Game"))OpenSlots("SlotsSave","Pause");
            if(Button(new Rect(485,390,310,48),"Save & Main Menu"))game.ReturnToMainMenu();
            if(Button(new Rect(485,452,310,48),"Save & Quit")){game.Save();if(SaveSystem.LastError==null)Quit();}
            Label(470,540,340,36,"The snow can wait.",center);
        }
        void SettingsPanel()
        {
            Paper(new Rect(330,65,620,592));Label(370,95,540,48,"Settings",heading);
            Label(370,162,230,30,"Music volume");game.preferences.music=GUI.HorizontalSlider(new Rect(620,177,270,24),game.preferences.music,0,1);
            Label(370,216,230,30,"Sound volume");game.preferences.sound=GUI.HorizontalSlider(new Rect(620,231,270,24),game.preferences.sound,0,1);
            Label(370,270,230,30,"Brightness");game.preferences.brightness=GUI.HorizontalSlider(new Rect(620,285,270,24),game.preferences.brightness,.65f,1f);
            game.preferences.footprints=GUI.Toggle(new Rect(377,338,510,30),game.preferences.footprints,(game.preferences.footprints?"[x]":"[ ]")+"  Show footprints",body);
            game.preferences.snow=GUI.Toggle(new Rect(377,384,510,30),game.preferences.snow,(game.preferences.snow?"[x]":"[ ]")+"  Falling snow",body);
            game.preferences.fullscreen=GUI.Toggle(new Rect(377,430,510,30),game.preferences.fullscreen,(game.preferences.fullscreen?"[x]":"[ ]")+"  Fullscreen",body);
            Label(370,485,540,60,"WASD / arrows: walk  ·  Shift: faster\nE: interact  ·  Space: fish  ·  M: map  ·  J: journal",small);
            game.sound.Apply(game.preferences);
            if(Button(new Rect(370,571,245,49),"Apply & Back")){game.ApplyPreferences();game.page=game.returnPage;}
            if(Button(new Rect(645,571,245,49),"Restore defaults")){game.preferences=new Preferences();game.ApplyPreferences();}
        }
        void Map()
        {
            Paper(new Rect(130,30,1020,642));Label(164,53,850,44,"A few familiar places",heading);
            Rect area=new Rect(170,116,940,438);Fill(area,new Color(.77f,.85f,.88f,.65f));
            for(int i=0;i<5;i++)
            {
                Vector2 a=MapPoint(WorldBuilder.Places[i],area),b=MapPoint(WorldBuilder.Places[(i+1)%5],area);
                for(int j=0;j<28;j++){Vector2 p=Vector2.Lerp(a,b,j/28f);Fill(new Rect(p.x,p.y,3,3),new Color(.47f,.6f,.66f,.4f));}
            }
            for(int i=0;i<5;i++)
            {
                Vector2 p=MapPoint(WorldBuilder.Places[i],area);
                bool known=game.data.discovered.Contains("place"+i);
                Fill(new Rect(p.x-5,p.y-5,10,10),known?ink:new Color(.5f,.64f,.69f));
                Label(p.x+10,p.y-11,195,45,WorldBuilder.PlaceNames[i]+(known?"":"  ?"),small);
            }
            Vector2 me=MapPoint(game.player.Position,area);Fill(new Rect(me.x-5,me.y-5,10,10),new Color(.82f,.32f,.12f));Label(me.x+10,me.y+10,90,24,"You",small);
            Label(170,566,920,48,"N ↑    ·    Orange: you    ·    ?: a place to discover\nPaths loop gently around the forest. The dotted route connects all five places.",small);
            if(Button(new Rect(918,615,190,38),"Back to the snow"))game.page="Play";
        }
        Vector2 MapPoint(Vector2 p,Rect rect)
        {
            float x=WorldBuilder.Wrap(p.x,game.config.worldSize.x),y=WorldBuilder.Wrap(p.y,game.config.worldSize.y);
            return new Vector2(rect.x+(x/game.config.worldSize.x+.5f)*rect.width,rect.y+(.5f-y/game.config.worldSize.y)*rect.height);
        }
        void Journal()
        {
            Paper(new Rect(175,28,930,645));Label(212,50,800,48,"Notes from a winter walk",heading);
            journalScroll=GUI.BeginScrollView(new Rect(210,111,858,472),journalScroll,new Rect(0,0,825,1070));
            var d=game.data;var c=game.config;
            Label(4,0,800,48,"Today’s little plans",heading);
            Label(4,55,790,230,
                Task(d.logs>=c.firewoodGoal,"Gather fallen firewood",d.logs,c.firewoodGoal)+"\n"+
                Task(d.berries>=c.berryGoal,"Collect winter berries",d.berries,c.berryGoal)+"\n"+
                Task(d.wildlife.Count>=c.wildlifeGoal,"Observe different animals",d.wildlife.Count,c.wildlifeGoal)+"\n"+
                Task(d.notes.Count>=3,"Find the three letters",d.notes.Count,3)+"\n"+
                Task(d.lanterns.Count>=3,"Light the trail lanterns",d.lanterns.Count,3)+"\n"+
                Task(d.fish>=c.fishGoal,"Fish at Glasswater Pond",d.fish,c.fishGoal)+"\n"+
                (d.craneReleased?"✓":"○")+"  Fold a paper crane at the cabin, carry it to the High Clearing.");
            Label(4,297,790,70,"Wildlife recorded: "+(d.wildlife.Count==0?"none yet. Look near the paths.":string.Join(", ",d.wildlife.ToArray())),body);
            Label(4,379,790,50,"Letters found",heading);
            for(int i=0;i<3;i++)
            {
                float y=445+i*181;bool found=d.notes.Contains("note"+i);
                Label(4,y,790,36,found?WorldBuilder.NoteTitles[i]:"An undiscovered letter",heading);
                Label(4,y+43,790,135,found?WorldBuilder.NoteTexts[i]:"Look near the pond, the high clearing, and the listening grove.",body);
            }
            GUI.EndScrollView();
            Label(212,603,555,38,game.Objective(),small);
            if(Button(new Rect(845,604,219,43),"Close journal"))game.page="Play";
        }
        string Task(bool done,string name,int count,int goal){return(done?"✓":"○")+"  "+name+"   "+count+" / "+goal;}
        void Dialogue()
        {
            Paper(new Rect(300,125,680,470));Label(340,155,600,80,game.dialogueTitle,heading);
            Label(340,243,600,260,game.dialogueText);
            if(Button(new Rect(715,520,220,48),"Carry on"))game.page="Play";
        }
        void Fishing()
        {
            Paper(new Rect(295,170,690,365));Label(335,201,610,49,"A moment by the water",heading);
            Label(335,268,610,62,"Press SPACE when the float is in the blue band.\nThere is no limit on tries. Esc puts the line away.");
            Fill(new Rect(350,369,580,16),new Color(.73f,.76f,.75f));Fill(new Rect(350+580*.34f,357,580*.32f,40),new Color(.48f,.66f,.76f));
            Fill(new Rect(350+game.FloatPosition*580-5,347,10,60),ink);
            if(Button(new Rect(349,444,280,49),"Cast / Catch [SPACE]"))game.CatchFish();
            if(Button(new Rect(651,444,280,49),"Put line away"))game.page="Play";
        }
        void Ending()
        {
            Paper(new Rect(310,100,660,518));Icon("icon_crane",new Rect(592,121,96,83));
            Label(352,211,576,56,"A little place to come back to",heading);
            Label(352,292,576,190,"The wood is stacked. The lanterns are lit. Somewhere above the clearing, a paper bird rests in a snowy branch.\n\nYou pour the tea and watch the window turn blue.\n\nFor today, this is enough.");
            if(Button(new Rect(352,533,270,49),"Stay a little longer"))game.page="Play";
            if(Button(new Rect(644,533,280,49),"Save & Main Menu"))game.ReturnToMainMenu();
        }
        void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying=false;
#else
            Application.Quit();
#endif
        }
    }
}
