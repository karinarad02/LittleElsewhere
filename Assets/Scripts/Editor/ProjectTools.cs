#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace LittleElsewhere.Editor
{
    public static class ProjectTools
    {
        const string ScenePath="Assets/Scenes/MainMenu.unity";
        static readonly string[] AllScenes={ScenePath,"Assets/Scenes/Loading.unity","Assets/Scenes/LittleElsewhere.unity"};
        [MenuItem("Little Elsewhere/Open Game Scene")]
        public static void OpenScene()
        {
            if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())EditorSceneManager.OpenScene(ScenePath);
        }
        [MenuItem("Little Elsewhere/Open Gameplay Scene")]
        public static void OpenGameplayScene()
        {
            if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())EditorSceneManager.OpenScene("Assets/Scenes/LittleElsewhere.unity");
        }
        [MenuItem("Little Elsewhere/Validate Project")]
        public static void Validate()
        {
            int count=0;
            string[] required={"camp","cabin","lake","logs","berries","lantern","snow","paper","landscape","icon_journal","icon_crane","sign","footprint","glow","dock","boat","bench","shed","woodstore","bush","bare_0","bare_1","flake"};
            foreach(string name in required){Require(Art.Texture(name)!=null,"Missing art: "+name);count++;}
            foreach(string direction in new[]{"idle","down","up","left","right"})for(int i=0;i<4;i++)
            {Require(Art.Texture("player_"+direction+"_"+i)!=null,"Missing player frame");count++;}
            foreach(string species in new[]{"fox","deer","rabbit","squirrel","bird","owl"})Require(Art.Texture(species+"_0")!=null,"Missing wildlife: "+species);
            foreach(string clip in new[]{"wind","music","bell","step","pickup"})Require(Resources.Load<AudioClip>("Audio/"+clip)!=null,"Missing audio: "+clip);
            for(int i=0;i<5;i++)Require(Art.Texture("pine_"+i)!=null,"Missing tree");
            for(int i=0;i<3;i++)Require(Art.Texture("rock_"+i)!=null,"Missing rock");
            foreach(var scene in AllScenes)Require(File.Exists(scene),"Scene missing: "+scene);
            Require(Resources.Load<TextAsset>("PreloadCatalog")!=null,"Loading catalog missing");
            var config=Resources.Load<GameConfig>("GameConfig");Require(config!=null,"Configuration missing");
            Require(config.worldSize.x>=60&&config.worldSize.y>=50,"World must fit camera and landmarks");
            Require(config.firewoodGoal<=8&&config.berryGoal<=6&&config.wildlifeGoal<=6,"Goals exceed available items");
            Require(config.fishGoal>=1,"Fishing goal must be positive");
            Require(config.dayLengthSeconds>0,"Day length must be positive");
            Require(Resources.Load<Shader>("Lighting/ProjectedShadow")!=null,"Shadow shader missing");
            Require(Resources.Load<Texture2D>("HD/pine_0").height>=1024,"High-resolution tree missing");
            Require(Resources.Load<Texture2D>("HD/cabin").height>=1024,"High-resolution cabin missing");
            var noon=DayNightCycle.Evaluate(12);var night=DayNightCycle.Evaluate(0);
            Require(noon.sunOpacity>0&&night.sunOpacity<.001f&&night.moonOpacity>0,"Day/night shadows failed");
            Require(DayNightCycle.Evaluate(7).sunLength>noon.sunLength,"Dawn shadows must be longer than noon shadows");
            Require(SaveSystem.SlotPath(1)!=SaveSystem.SlotPath(2)&&SaveSystem.SlotPath(2)!=SaveSystem.SlotPath(3),"Slot paths collide");
            Require(Mathf.Abs(WorldBuilder.Wrap(41,80)+39)<.001f,"World wrap check failed");
            Require(Mathf.Abs(WorldBuilder.Wrap(-41,80)-39)<.001f,"Negative world wrap check failed");
            var save=new SaveData();Require(!save.Ready(config),"Empty save should not complete objectives");
            save.logs=config.firewoodGoal;save.berries=config.berryGoal;save.fish=config.fishGoal;
            for(int i=0;i<config.wildlifeGoal;i++)save.wildlife.Add("species"+i);
            for(int i=0;i<3;i++){save.notes.Add("note"+i);save.lanterns.Add("lamp"+i);}
            Require(save.Ready(config),"Complete objectives should unlock the crane");
            var restored=JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(save));Require(restored.Ready(config),"Save round trip failed");
            Debug.Log("Little Elsewhere: validation passed. Assets, goals, wrapping, progression and JSON round trip checked ("+count+" primary artwork entries).");
        }
        [MenuItem("Little Elsewhere/Preview Lighting/Dawn 06-30")]
        public static void Dawn(){PreviewHour(6.5f);}
        [MenuItem("Little Elsewhere/Preview Lighting/Noon 12-00")]
        public static void Noon(){PreviewHour(12);}
        [MenuItem("Little Elsewhere/Preview Lighting/Dusk 18-30")]
        public static void Dusk(){PreviewHour(18.5f);}
        [MenuItem("Little Elsewhere/Preview Lighting/Night 23-00")]
        public static void Night(){PreviewHour(23);}
        static void PreviewHour(float hour)
        {
            var game=UnityEngine.Object.FindAnyObjectByType<GameSession>();
            if(!EditorApplication.isPlaying || game==null || game.dayNight==null || !game.HasSession){Debug.LogWarning("Enter Play Mode and start or load a walk first.");return;}
            game.data.worldHours=System.Math.Floor(game.data.worldHours/24)*24+hour;
            game.dayNight.Apply();game.page="Pause";
            Debug.Log("Lighting preview: "+game.dayNight.Clock+". Resume to inspect the world. This changes the active session's time.");
        }
        static void Require(bool ok,string message){if(!ok)throw new InvalidOperationException(message);}
        [MenuItem("Little Elsewhere/Build Windows x64")]
        public static void BuildWindows(){Build(BuildTarget.StandaloneWindows64,"Builds/Windows/LittleElsewhere.exe");}
        [MenuItem("Little Elsewhere/Build macOS")]
        public static void BuildMac(){Build(BuildTarget.StandaloneOSX,"Builds/macOS/LittleElsewhere.app");}
        [MenuItem("Little Elsewhere/Build Linux x64")]
        public static void BuildLinux(){Build(BuildTarget.StandaloneLinux64,"Builds/Linux/LittleElsewhere");}
        static void Build(BuildTarget target,string path)
        {
            Validate();Directory.CreateDirectory(Path.GetDirectoryName(path));
            var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=AllScenes,locationPathName=path,target=target,options=BuildOptions.None});
            if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Build failed: "+report.summary.result);
            Debug.Log("Build complete: "+Path.GetFullPath(path));
        }
    }
    public sealed class ArtworkImporter : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            if(!assetPath.StartsWith("Assets/Resources/Art/") && !assetPath.StartsWith("Assets/Resources/HD/"))return;
            var importer=(TextureImporter)assetImporter;
            importer.textureType=TextureImporterType.Default;
            importer.alphaSource=TextureImporterAlphaSource.FromInput;importer.alphaIsTransparency=true;
            importer.mipmapEnabled=false;importer.wrapMode=TextureWrapMode.Clamp;importer.filterMode=FilterMode.Bilinear;
            importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=4096;importer.npotScale=TextureImporterNPOTScale.None;
        }
    }
}
#endif
