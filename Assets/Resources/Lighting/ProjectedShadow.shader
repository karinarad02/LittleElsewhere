Shader "LittleElsewhere/ProjectedShadow"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _ShadowTint ("Shadow", Color) = (0.2,0.35,0.5,0.25)
        _ShadowVector ("Projection", Vector) = (1,-0.5,0,0)
        _BaseY ("Foot pivot", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "PreviewType"="Plane" "CanUseSpriteAtlas"="False"}
        Cull Off Lighting Off ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex ShadowVert
            #pragma fragment ShadowFrag
            #pragma target 2.0
            #include "UnitySprites.cginc"
            float _BaseY;
            float4 _ShadowVector;
            fixed4 _ShadowTint;
            v2f ShadowVert(appdata_t IN)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                IN.vertex=UnityFlipSprite(IN.vertex,_Flip);
                float height=max(0,IN.vertex.y-_BaseY);
                IN.vertex.x+=height*_ShadowVector.x;
                IN.vertex.y=_BaseY+height*_ShadowVector.y;
                OUT.vertex=UnityObjectToClipPos(IN.vertex);
                OUT.texcoord=IN.texcoord;
                OUT.color=IN.color*_Color*_RendererColor;
                return OUT;
            }
            fixed4 ShadowFrag(v2f IN):SV_Target
            {
                fixed alpha=SampleSpriteTexture(IN.texcoord).a;
                return fixed4(_ShadowTint.rgb,alpha*_ShadowTint.a*IN.color.a);
            }
            ENDCG
        }
    }
}
