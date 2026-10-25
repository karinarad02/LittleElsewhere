using UnityEngine;
namespace LittleElsewhere
{
    public sealed class GameSession : MonoBehaviour
    {
        public GameConfig config;
        public SaveData data;
        public Preferences preferences;
        public WorldBuilder world;
        public PlayerController player;
        public Camera cam;
        public Soundscape sound;
        public GameUI ui;
        public DayNightCycle dayNight;
        public WorldItem nearby;
        public string page="Title", returnPage="Title", dialogueTitle, dialogueText, toast;
        public float toastUntil;
        public bool Playing { get { return page=="Play"; } }
        bool sessionActive;
        public bool HasSession {get{return sessionActive;}}
        float autosave;
        public float fishingClock, fishingSpeed=1;
        public int fishAttempts;
        bool IsMenuScene {get{return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name==SceneFlow.MenuScene;}}
        void Start()
        {
            Application.targetFrameRate=60;
            config=config!=null?config:Resources.Load<GameConfig>("GameConfig");
            if(config==null)config=ScriptableObject.CreateInstance<GameConfig>();
            preferences=SaveSystem.ReadPreferences();Screen.fullScreen=preferences.fullscreen;
            var cameraObject=new GameObject("Main Camera");cam=cameraObject.AddComponent<Camera>();cameraObject.tag="MainCamera";
            cam.orthographic=true;cam.clearFlags=CameraClearFlags.SolidColor;cam.backgroundColor=new Color(.89f,.92f,.94f);cam.nearClipPlane=.1f;cam.farClipPlane=100;
            cameraObject.transform.position=new Vector3(0,0,-10);cameraObject.AddComponent<AudioListener>();
            sound=gameObject.AddComponent<Soundscape>();sound.Setup();sound.Apply(preferences);
            ui=gameObject.AddComponent<GameUI>();ui.game=this;
            dayNight=gameObject.AddComponent<DayNightCycle>();
            data=new SaveData();
            if(IsMenuScene){page="Title";return;}
            if(SceneFlow.HasRequest)
            {
                bool newWalk=SceneFlow.IsNewWalk;SaveSystem.SelectSlot(SceneFlow.Slot);SceneFlow.Consume();
                if(newWalk)NewGame();else ContinueGame();
            }
            else Rebuild(new SaveData());
        }
        public void Rebuild(SaveData save)
        {
            if(world!=null){world.gameObject.SetActive(false);if(Application.isPlaying)Destroy(world.gameObject);else DestroyImmediate(world.gameObject);}
            if(player!=null){player.gameObject.SetActive(false);if(Application.isPlaying)Destroy(player.gameObject);else DestroyImmediate(player.gameObject);}
            data=save;
            var p=new GameObject("Player • orange coat");player=p.AddComponent<PlayerController>();player.Setup(this);
            player.Position=new Vector2(save.x,save.y);
            // New walks begin safely in front of the cabin.
            if(data.elapsed<=0 && data.x==0 && data.y==0)player.Position=new Vector2(0,-4);
            var w=new GameObject("Forest • seeded wrapped world");world=w.AddComponent<WorldBuilder>();world.Build(this);
            cam.transform.position=new Vector3(player.Position.x,player.Position.y,-10);
            nearby=null;autosave=0;
            if(dayNight!=null)dayNight.Bind(this);
        }
        public void NewGame()
        {
            if(IsMenuScene){SceneFlow.StartWalk(SaveSystem.CurrentSlot,true);return;}
            sessionActive=true;Rebuild(new SaveData());page="Play";
            Dialogue("A little elsewhere", "The cabin has been waiting for you.\n\nBring home five pieces of fallen wood and three handfuls of berries. Explore the paths, note four kinds of wildlife, find three letters, and light the three trail lanterns.\n\nThere is no hurry, no hunger, and no wrong turn. Your journal [J] and map [M] will help you find your way.");
            Save();
        }
        public void ContinueGame()
        {
            if(IsMenuScene){SceneFlow.StartWalk(SaveSystem.CurrentSlot,false);return;}
            var save=SaveSystem.Read();
            if(save==null){Notify(SaveSystem.LastError);return;}
            sessionActive=true;Rebuild(save);page="Play";Notify("Welcome back. The forest kept your place.");
        }
        public void Save()
        {
            if(!sessionActive)return;
            data.x=player.Position.x;data.y=player.Position.y;
            Notify(SaveSystem.Write(data)?"Saved to slot "+SaveSystem.CurrentSlot+".":"Could not save: "+SaveSystem.LastError);
        }
        public void ReturnToMainMenu()
        {
            Save();if(SaveSystem.LastError==null)SceneFlow.MainMenu();
        }
        public void Notify(string text){toast=text;toastUntil=Time.unscaledTime+5;}
        public void Dialogue(string title,string text)
        {
            dialogueTitle=title;dialogueText=text;page="Dialogue";
        }
        public void ApplyPreferences()
        {
            preferences.music=Mathf.Clamp01(preferences.music);preferences.sound=Mathf.Clamp01(preferences.sound);
            preferences.brightness=Mathf.Clamp(preferences.brightness,.65f,1f);
            sound.Apply(preferences);if(player!=null)player.ShowFootprints(preferences.footprints);
            Screen.fullScreen=preferences.fullscreen;SaveSystem.WritePreferences(preferences);
        }
        void Update()
        {
            if(cam==null)return;
            if(world==null)
            {
                if(Input.GetKeyDown(KeyCode.Escape))
                {if(page=="Settings"||page.StartsWith("Slots")||page=="ConfirmNew")page=returnPage;}
                return;
            }
            cam.orthographicSize=Mathf.Min(config.cameraSize,19f/Mathf.Max(.8f,cam.aspect));
            float dt=Mathf.Min(Time.deltaTime,.05f);
            if(Playing)
            {
                data.elapsed+=Time.deltaTime;dayNight.Tick(Time.deltaTime);player.Tick(dt);
                foreach(var item in world.items)
                {
                    if(item.kind==Kind.Landmark&&!data.discovered.Contains(item.id)&&world.Delta(item.anchor,player.Position).magnitude<4.7f)
                    {data.location=item.title;data.discovered.Add(item.id);Notify("Discovered: "+item.title);sound.Bell();}
                }
                nearby=world.Nearest();
                if(Input.GetKeyDown(KeyCode.E)&&nearby!=null)Interact(nearby);
                if(Input.GetKeyDown(KeyCode.Escape))page="Pause";
                else if(Input.GetKeyDown(KeyCode.M))page="Map";
                else if(Input.GetKeyDown(KeyCode.J)||Input.GetKeyDown(KeyCode.Tab))page="Journal";
                if(Input.GetKeyDown(KeyCode.F5))Save();
                autosave+=dt;if(autosave>=60){autosave=0;Save();}
            }
            else if(page=="Fishing")
            {
                dayNight.Tick(Time.deltaTime);data.elapsed+=Time.deltaTime;fishingClock+=dt*fishingSpeed;
                if(Input.GetKeyDown(KeyCode.Space))CatchFish();
                if(Input.GetKeyDown(KeyCode.Escape)){page="Play";Notify("You put the fishing line away.");}
            }
            else if(Input.GetKeyDown(KeyCode.Escape))
            {
                if(page=="Settings")page=returnPage;
                else if(page=="Pause"||page=="Map"||page=="Journal"||page=="Dialogue")page="Play";
                else if(page=="ConfirmNew")page=returnPage;
                else if(page.StartsWith("Slots"))page=returnPage;
                else if(page=="Ending")page="Play";
            }
            world.Render(Playing?dt:0);
            Vector3 target=new Vector3(player.Position.x,player.Position.y+1.4f,-10);
            cam.transform.position=Vector3.Lerp(cam.transform.position,target,1-Mathf.Exp(-dt*5));
        }
        public void Interact(WorldItem item)
        {
            switch(item.kind)
            {
                case Kind.Camp:
                    data.location="The Campsite";dayNight.AdvanceHours(8);sound.Bell();Dialogue("A little rest","The fire settles into a gentle glow. Eight hours pass beside the tent.\n\nDay "+data.day+", "+dayNight.Clock+". You wake rested and ready for the snow.");Save();break;
                case Kind.Wood:
                    data.collected.Add(item.id);data.logs++;sound.Pickup();Notify("Firewood "+data.logs+" / "+config.firewoodGoal);break;
                case Kind.Berries:
                    data.collected.Add(item.id);data.berries++;sound.Pickup();Notify("Winter berries "+data.berries+" / "+config.berryGoal);break;
                case Kind.Wildlife:
                    if(!data.wildlife.Contains(item.species))data.wildlife.Add(item.species);
                    sound.Bell();Dialogue("A moment with the "+item.species,"You stand quietly and watch.\n\n"+AnimalText(item.species)+"\n\nRecorded in your journal. Wildlife: "+data.wildlife.Count+" / "+config.wildlifeGoal);break;
                case Kind.Note:
                    if(!data.notes.Contains(item.id))data.notes.Add(item.id);sound.Pickup();Dialogue(item.title,item.text);break;
                case Kind.Lantern:
                    if(!data.lanterns.Contains(item.id))data.lanterns.Add(item.id);sound.Bell();Notify("A trail lantern glows. "+data.lanterns.Count+" / 3");break;
                case Kind.Fishing:
                    if(data.fish>=config.fishGoal)Dialogue("Glasswater Pond","You have enough for the evening. Beneath the glassy ice, the pond goes quietly on.");
                    else{page="Fishing";fishingClock=0;fishAttempts=0;fishingSpeed=.8f+data.fish*.1f;}break;
                case Kind.Cabin: VisitCabin();break;
                case Kind.Release:
                    if(data.craneMade&&!data.craneReleased){data.craneReleased=true;sound.Bell();Dialogue("A paper wish","You set the tiny crane on the wind. For a moment it floats above the clearing, then settles among the snowy branches.\n\nThe cabin light is waiting. Return home whenever you are ready.");Save();}
                    else Dialogue("The High Clearing",data.craneReleased?"Your paper wish is out in the world. There is room for you here, too.":"The valley opens beneath you. This would be a lovely place to leave a paper wish.\n\nFinish the small tasks in your journal, then return to the cabin to fold a crane.");break;
                case Kind.Landmark:
                    if(!data.discovered.Contains(item.id))data.discovered.Add(item.id);Dialogue(item.title,item.text);break;
            }
            world.Refresh();
            if(item.kind!=Kind.Cabin && item.kind!=Kind.Fishing) { data.x=player.Position.x;data.y=player.Position.y;SaveSystem.Write(data); }
        }
        void VisitCabin()
        {
            if(data.craneReleased)
            {
                data.finished=true;Save();page="Ending";sound.Bell();return;
            }
            if(data.Ready(config))
            {
                if(!data.craneMade){data.craneMade=true;sound.Bell();Dialogue("Something small to carry","The fire catches. Berry tea steams beside the window.\n\nFrom the blank corner of an old letter, you fold a tiny paper crane. Take it to the High Clearing, northwest of the cabin, and let the day go.");Save();}
                else Dialogue("The kettle is on","Your paper crane is ready. Carry it to the High Clearing, northwest of the cabin. The map [M] shows the way.");
            }
            else Dialogue("The cabin is warm","You brush snow from your boots and put your supplies by the door.\n\n"+Objective()+"\n\nCheck your journal [J] for the rest of your little plans. Your progress has been saved.");
            Save();
        }
        public string Objective()
        {
            if(data.finished)return "Stay a little longer. The forest is yours to explore.";
            if(data.craneReleased)return "Return to the cabin for a quiet evening.";
            if(data.craneMade)return "Carry your paper crane to the High Clearing.";
            if(data.logs<config.firewoodGoal)return "Gather fallen wood: "+data.logs+" / "+config.firewoodGoal;
            if(data.berries<config.berryGoal)return "Gather winter berries: "+data.berries+" / "+config.berryGoal;
            if(data.wildlife.Count<config.wildlifeGoal)return "Observe wildlife: "+data.wildlife.Count+" / "+config.wildlifeGoal;
            if(data.notes.Count<3)return "Find the three letters: "+data.notes.Count+" / 3";
            if(data.lanterns.Count<3)return "Light the trail lanterns: "+data.lanterns.Count+" / 3";
            if(data.fish<config.fishGoal)return "Fish at Glasswater Pond: "+data.fish+" / "+config.fishGoal;
            return "Return to the cabin and fold a paper wish.";
        }
        static string AnimalText(string species)
        {
            switch(species)
            {
                case "fox":return "A copper tail brushes the snow. The fox pauses, listens, and carries on with its own small errands.";
                case "deer":return "The deer lifts its head. You share a breath of still air before it lowers its nose to the snow again.";
                case "rabbit":return "Two ears rise above the drift. A rabbit leaves a neat little line of tracks where you thought there was nothing.";
                case "squirrel":return "The squirrel has important things to do. You watch it bury a treasure that only it will remember.";
                case "bird":return "A little bird tries a few notes against the hush. The forest seems to listen back.";
                default:return "An owl watches from the edge of the trees, as patient as the winter afternoon.";
            }
        }
        public float FloatPosition { get { return Mathf.PingPong(fishingClock,1); } }
        public void CatchFish()
        {
            if(page!="Fishing")return;
            if(Mathf.Abs(FloatPosition-.5f)<.16f)
            {
                data.fish++;sound.Bell();Save();
                if(data.fish>=config.fishGoal)Dialogue("Enough for the evening","Two small fish, and a quiet moment by the water. You pack away the line.\n\n"+Objective());
                else{fishingClock=0;Notify("A small silver fish. One more, then home.");}
            }
            else{fishAttempts++;sound.Pickup();Notify("Just a ripple. Try again when the float reaches the blue band.");}
        }
        void OnApplicationPause(bool paused){if(sessionActive&&paused&&data!=null&&page!="Title"&&page!="ConfirmNew")Save();}
        void OnApplicationQuit(){if(sessionActive&&data!=null&&player!=null&&page!="Title"&&page!="ConfirmNew")Save();}
    }
}
