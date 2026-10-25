#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace LittleElsewhere.Editor
{
    [InitializeOnLoad]
    public static class SmokeCheck
    {
        static bool armed,sceneFlowStarted,sawLoading;
        static double started;
        static int stage,originalSlot;
        static readonly Dictionary<string,byte[]> originals=new Dictionary<string,byte[]>();
        static SmokeCheck(){EditorApplication.update+=Tick;EditorApplication.playModeStateChanged+=OnMode;}
        [MenuItem("Little Elsewhere/Run Play Mode Smoke Check")]
        public static void Run()
        {
            if(EditorApplication.isPlaying){Debug.LogWarning("Stop Play Mode before running the smoke check.");return;}
            ProjectTools.OpenScene();SessionState.SetBool("LE.Smoke",true);EditorApplication.isPlaying=true;
        }
        static void OnMode(PlayModeStateChange mode)
        {
            if(mode==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool("LE.Smoke",false))
            {
                SessionState.SetBool("LE.Smoke",false);started=EditorApplication.timeSinceStartup;stage=0;sceneFlowStarted=false;sawLoading=false;originalSlot=SaveSystem.CurrentSlot;
                originals.Clear();
                for(int i=1;i<=3;i++)foreach(string suffix in new[]{"",".bak",".tmp"})
                {string path=SaveSystem.SlotPath(i)+suffix;originals[path]=File.Exists(path)?File.ReadAllBytes(path):null;}
                armed=true;
            }
            if(mode==PlayModeStateChange.ExitingPlayMode && armed)Finish(false,"Smoke check stopped before completion.");
        }
        static void Tick()
        {
            if(!armed||!EditorApplication.isPlaying)return;
            try
            {
                string active=UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                if(active==SceneFlow.LoadingScene)sawLoading=true;
                var g=UnityEngine.Object.FindFirstObjectByType<GameSession>();
                if(active==SceneFlow.MenuScene && g!=null && g.cam!=null && !sceneFlowStarted)
                {sceneFlowStarted=true;SaveSystem.SelectSlot(1);g.NewGame();return;}
                if(g==null||g.world==null){if(EditorApplication.timeSinceStartup-started>120)throw new Exception("Menu/loading/game startup timed out");return;}
                if(stage==0)
                {
                    Require(sceneFlowStarted&&sawLoading,"Dedicated menu/loading transition was not observed");
                    Require(g.world.items.Count>100,"World was not generated");Require(g.player!=null,"Player missing");
                    for(int i=1;i<=3;i++)
                    {
                        SaveSystem.SelectSlot(i);
                        Require(SaveSystem.Write(new SaveData{logs=i,worldHours=8+i,location="Test slot "+i}),"Slot write failed");
                    }
                    for(int i=1;i<=3;i++){var slot=SaveSystem.PreviewSlot(i);Require(slot!=null&&slot.logs==i&&Math.Abs(slot.worldHours-(8+i))<.001,"Save slots are not independent");}
                    SaveSystem.SelectSlot(3);Require(SaveSystem.Write(new SaveData{logs=3,worldHours=11}),"Backup setup failed");File.WriteAllText(SaveSystem.FilePath,"invalid json");
                    Require(SaveSystem.Read()!=null&&SaveSystem.Read().logs==3,"Backup recovery failed");
                    SaveSystem.SelectSlot(1);g.NewGame();g.page="Play";
                    double before=g.data.worldHours;g.dayNight.Tick(g.config.dayLengthSeconds/24);
                    Require(Math.Abs(g.data.worldHours-before-1)<.001,"Clock rate is incorrect");
                    g.data.worldHours=23.99;g.dayNight.AdvanceHours(1);
                    Require(g.data.day==2&&g.dayNight.Hour<1,"Midnight rollover failed");
                    var noon=DayNightCycle.Evaluate(12);var midnight=DayNightCycle.Evaluate(0);
                    Require(noon.sunOpacity>0&&midnight.moonOpacity>0&&midnight.sunOpacity<.001f,"Sun/moon phases failed");
                    Require(DayNightCycle.Evaluate(7).sunLength>noon.sunLength,"Dawn shadows should be longer than noon shadows");
                    foreach(var item in g.world.items)
                        if(item.kind==Kind.Wood||item.kind==Kind.Berries||item.kind==Kind.Wildlife||item.kind==Kind.Note||item.kind==Kind.Lantern)g.Interact(item);
                    g.data.fish=g.config.fishGoal;Require(g.data.Ready(g.config),"Objectives did not complete");
                    foreach(var item in g.world.items)if(item.kind==Kind.Cabin)g.Interact(item);
                    Require(g.data.craneMade,"Cabin did not craft crane");
                    foreach(var item in g.world.items)if(item.kind==Kind.Release)g.Interact(item);
                    Require(g.data.craneReleased,"Crane did not release");
                    foreach(var item in g.world.items)if(item.kind==Kind.Cabin)g.Interact(item);
                    Require(g.data.finished&&g.page=="Ending","Ending did not trigger");
                    g.Save();var loaded=SaveSystem.Read();Require(loaded!=null&&loaded.finished&&loaded.day==2,"Save/load lost ending or time");
                    g.Rebuild(loaded);g.page="Play";stage=1;started=EditorApplication.timeSinceStartup;
                }
                else if(EditorApplication.timeSinceStartup-started>1)
                {
                    Require(g.data.finished&&g.world!=null,"Reloaded session invalid");Finish(true,"Menu/loading/game scenes, three save slots, clock rate, midnight rollover, sun/moon phases, world, objectives, ending and save/load passed.");
                }
            }
            catch(Exception ex){Finish(false,ex.ToString());}
        }
        static void Require(bool condition,string message){if(!condition)throw new Exception(message);}
        static void Finish(bool passed,string message)
        {
            var g=UnityEngine.Object.FindFirstObjectByType<GameSession>();if(g!=null)g.page="Title";
            armed=false;
            foreach(var original in originals)
            {
                if(original.Value!=null)File.WriteAllBytes(original.Key,original.Value);
                else if(File.Exists(original.Key))File.Delete(original.Key);
            }
            SaveSystem.SelectSlot(originalSlot);
            if(passed)Debug.Log("Little Elsewhere smoke check: PASS. "+message);else Debug.LogError("Little Elsewhere smoke check: FAIL. "+message);
            EditorApplication.isPlaying=false;
        }
    }
}
#endif
