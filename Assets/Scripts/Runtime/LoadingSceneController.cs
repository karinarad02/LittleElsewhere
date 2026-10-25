using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace LittleElsewhere
{
    [Serializable] public sealed class PreloadCatalog {public string[] art;}
    public sealed class LoadingSceneController : MonoBehaviour
    {
        float progress;
        string status="Preparing your winter walk…",error;
        Texture2D backdrop;
        GUIStyle heading,body,center;
        readonly string[] tips={"A quiet place, just for a little while.","Every trail leads somewhere familiar.","The campsite lets you rest for eight hours.","Each save keeps its own day, time, and discoveries."};
        IEnumerator Start()
        {
            var camera=new GameObject("Loading Camera").AddComponent<Camera>();camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.9f,.93f,.95f);
            if(!SceneFlow.HasRequest){SceneFlow.MainMenu();yield break;}
            backdrop=Art.Texture("landscape");
            yield return null; // Display a real loading frame before beginning work.
            SceneFlow.PreloadedAssets.Clear();
            var catalogText=Resources.Load<TextAsset>("PreloadCatalog");
            if(catalogText==null){error="The loading catalog is missing. Reimport the project and try again.";yield break;}
            var catalog=JsonUtility.FromJson<PreloadCatalog>(catalogText.text);
            if(catalog==null||catalog.art==null){error="The loading catalog could not be read.";yield break;}
            int total=catalog.art.Length+8,index=0;
            foreach(var name in catalog.art)
            {
                status="Preparing artwork";
                var request=Resources.LoadAsync<Texture2D>(name);
                while(!request.isDone){progress=.85f*(index+request.progress)/total;yield return null;}
                if(request.asset==null){error="Missing artwork: "+name;yield break;}
                SceneFlow.PreloadedAssets.Add(request.asset);index++;progress=.85f*index/total;
                yield return null;
            }
            foreach(var name in new[]{"wind","music","step","bell","pickup"})
            {
                status="Listening to the forest";var request=Resources.LoadAsync<AudioClip>("Audio/"+name);yield return request;
                if(request.asset==null){error="Missing audio: "+name;yield break;}
                SceneFlow.PreloadedAssets.Add(request.asset);index++;progress=.85f*index/total;
            }
            var font=Resources.LoadAsync<Font>("Fonts/ElsewhereSerif");yield return font;if(font.asset!=null)SceneFlow.PreloadedAssets.Add(font.asset);index++;
            var shader=Resources.LoadAsync<Shader>("Lighting/ProjectedShadow");yield return shader;if(shader.asset!=null)SceneFlow.PreloadedAssets.Add(shader.asset);index++;
            var config=Resources.LoadAsync<GameConfig>("GameConfig");yield return config;if(config.asset!=null)SceneFlow.PreloadedAssets.Add(config.asset);index++;
            status="Following the path home";
            AsyncOperation scene=null;
            try {scene=SceneManager.LoadSceneAsync(SceneFlow.GameScene,LoadSceneMode.Single);}
            catch(Exception ex){error="The gameplay scene could not be opened: "+ex.Message;}
            if(scene==null)yield break;
            scene.allowSceneActivation=false;
            while(scene.progress<.9f){progress=.85f+.15f*Mathf.Clamp01(scene.progress/.9f);yield return null;}
            progress=1;status="Your walk is ready";yield return null;
            scene.allowSceneActivation=true;
        }
        void Styles()
        {
            if(heading!=null)return;
            Font font=Resources.Load<Font>("Fonts/ElsewhereSerif");
            heading=new GUIStyle(GUI.skin.label){font=font,fontSize=42,alignment=TextAnchor.MiddleCenter};
            body=new GUIStyle(GUI.skin.label){font=font,fontSize=21,wordWrap=true};
            center=new GUIStyle(body){alignment=TextAnchor.MiddleCenter};
            foreach(var style in new[]{heading,body,center})style.normal.textColor=new Color(.17f,.29f,.37f);
        }
        void Box(Rect rect,Color color){Color old=GUI.color;GUI.color=color;GUI.DrawTexture(rect,Texture2D.whiteTexture);GUI.color=old;}
        void OnGUI()
        {
            Styles();float scale=Mathf.Min(Screen.width/1280f,Screen.height/720f);
            GUI.matrix=Matrix4x4.TRS(new Vector3((Screen.width-1280*scale)/2,(Screen.height-720*scale)/2,0),Quaternion.identity,new Vector3(scale,scale,1));
            if(backdrop!=null)GUI.DrawTexture(new Rect(-500,-200,2280,1120),backdrop,ScaleMode.ScaleAndCrop);
            Box(new Rect(280,246,720,335),new Color(.97f,.96f,.93f,.95f));
            GUI.Label(new Rect(320,270,640,65),"Little Elsewhere",heading);
            GUI.Label(new Rect(320,356,640,46),error??status,center);
            if(error==null)
            {
                Box(new Rect(371,422,538,9),new Color(.63f,.68f,.7f));
                Box(new Rect(371,422,538*progress,9),new Color(.25f,.41f,.51f));
                GUI.Label(new Rect(550,445,180,35),Mathf.RoundToInt(progress*100)+"%",center);
                GUI.Label(new Rect(320,495,640,52),tips[(int)(Time.unscaledTime/5)%tips.Length],center);
            }
            else if(GUI.Button(new Rect(505,476,270,48),"Return to Main Menu"))SceneFlow.MainMenu();
            GUI.matrix=Matrix4x4.identity;
        }
    }
}
