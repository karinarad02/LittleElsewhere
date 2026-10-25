using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

public static class LESceneBuilder {
    [MenuItem("Little Elsewhere/Build Playable Scene")]
    public static void Build(){
        var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        var cam=new GameObject("Main Camera");
        cam.tag="MainCamera";
        var c=cam.AddComponent<Camera>();
        c.orthographic=true;
        c.orthographicSize=6;
        c.backgroundColor=new Color(.94f,.96f,.96f);
        cam.AddComponent<AudioListener>();
        
        var globalLight = new GameObject("Global Light 2D");
        var l2d = globalLight.AddComponent<Light2D>();
        l2d.lightType = Light2D.LightType.Global;
        l2d.color = Color.white;
        l2d.intensity = 1.0f;

        var player=Spawn("Player","Player/Idle_00",Vector3.zero,.75f,10);
        player.tag="Player";
        var rb=player.AddComponent<Rigidbody2D>();
        rb.gravityScale=0;
        rb.freezeRotation=true;
        player.AddComponent<CircleCollider2D>().radius=.23f;
        player.AddComponent<LEPlayer>();
        cam.AddComponent<LECamera>().target=player.transform;
        
        Random.InitState(44);
        for(int i=0;i<75;i++){
            var p=new Vector3(Random.Range(-21f,21f),Random.Range(-17f,17f),0);
            if(p.magnitude<3.3f)continue;
            var names=new[]{"PineTall_A","PineTall_B","PineMedium_A","PineMedium_B","PineSmall_A","PineSmall_B"};
            Spawn("Tree_"+i,"Trees/"+names[Random.Range(0,names.Length)],p,Random.Range(.7f,1.2f),Mathf.RoundToInt(-p.y*10));
        }
        Spawn("Cabin","Buildings/CabinMain",new Vector3(8,5,0),1.65f,-50);
        Spawn("Lake","Terrain/LakeWhole",new Vector3(-9,-6,0),2.3f,-50);
        for(int i=0;i<20;i++){
            var p=new Vector3(Random.Range(-18f,18f),Random.Range(-14f,14f),0);
            Spawn("Rock_"+i,"Props/Rock_"+new[]{"A","B","C","D"}[Random.Range(0,4)],p,.8f,Mathf.RoundToInt(-p.y*10));
        }
        var species=new[]{"Fox","Deer","Rabbit","Squirrel","Bird","Owl"};
        for(int i=0;i<18;i++){
            string type=species[i%species.Length];
            var p=new Vector3(Random.Range(-15f,15f),Random.Range(-12f,12f),0);
            var g=Spawn(type+"_"+i,"Animals/"+type+"_00",p,.9f,Mathf.RoundToInt(-p.y*10));
            g.AddComponent<LEAnimal>().species=type;
        }
        var game=new GameObject("Game Controller");
        game.AddComponent<LEGame>();
        LEGame.Current=LEGame.Screen.Main;
        LEGame.Paused=true;
        
        EditorSceneManager.SaveScene(scene,"Assets/Scenes/LittleElsewhere.unity");
        EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/LittleElsewhere.unity",true)};
        if (!Application.isBatchMode) {
            EditorUtility.DisplayDialog("Little Elsewhere","Playable scene created. Press Play to test!", "OK");
        }
    }

    static GameObject Spawn(string name,string path,Vector3 p,float scale,int order){
        var go=new GameObject(name);
        go.transform.position=p;
        go.transform.localScale=Vector3.one*scale;
        var r=go.AddComponent<SpriteRenderer>();
        r.sprite=Resources.Load<Sprite>(path);
        r.sortingOrder=order;
        return go;
    }
}

