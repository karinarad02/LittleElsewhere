using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace LittleElsewhere
{
    public static class SceneFlow
    {
        public const string MenuScene="MainMenu", LoadingScene="Loading", GameScene="LittleElsewhere";
        public static bool HasRequest {get;private set;}
        public static bool IsNewWalk {get;private set;}
        public static int Slot {get;private set;}=1;
        // Keep asynchronous resource requests alive across the scene activation boundary.
        public static readonly List<Object> PreloadedAssets=new List<Object>();
        public static void StartWalk(int slot,bool newWalk)
        {
            Slot=Mathf.Clamp(slot,1,3);IsNewWalk=newWalk;HasRequest=true;
            SceneManager.LoadScene(LoadingScene);
        }
        public static void Consume(){HasRequest=false;}
        public static void MainMenu(){HasRequest=false;PreloadedAssets.Clear();SceneManager.LoadScene(MenuScene);}
    }
}
