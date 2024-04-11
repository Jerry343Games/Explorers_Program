Shader "Custom/Wave"
{
Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _HeightWaveFrequencey("HeightWaveFrequencey",Float)=1
        _WaveFactor("WaveFactor",Float)=0.1
        _WaveStrength("WaveStrengh",Float)=0
        _WaveSpeed("WaveSpeed",Float)=1
        _PlayerPosition("PlayerPosition",Vector)=(0,0,0)
        _DeformationRadius("DeformationRadius",Float)=0
        _DeformationAmount("_DeformationAmount",Float)=1
        _Cutoff("Alpha Cutoff",Range(0,1))=0.5
    }
    SubShader
    {
        Tags {  "RenderType"="Opaque"}
        LOD 100
   
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _HeightWaveFrequencey;
            float4 _MainTex_ST;
            float _WaveFactor;
            float _WaveSpeed;
            float _WaveStrength;
            float _DeformationRadius;
            fixed3 _PlayerPosition;
            float _DeformationAmount;

            v2f vert (appdata v)
            {
                v2f o;
                o.uv = v.uv;
                float4 originalVert=v.vertex;
                //_HeightWaveFrequencey:缩放 _WaveFactor:偏移 v.vertx.y:自变量 v.vertx.x:因变量
                float time=_Time.y*_WaveSpeed;
                v.vertex.x=sin(_HeightWaveFrequencey*3.14*v.vertex.y+time+_WaveFactor)+originalVert.x;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                if (col.a < 0.5)
                {
                    discard;
                }
                return col;
            }
            ENDCG
        }
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragShadowCaster
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                //SHADOW_COORDS(0)
                
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.uv = v.uv;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 fragShadowCaster(v2f i) : SV_Target
            {
                // 简单地返回一个固定颜色
                fixed4 col = tex2D(_MainTex, i.uv);
                if (col.a < 0.5)
                {
                    discard;
                }
                return float4(0,0,0,0.5);
            }
            ENDCG
        }

    }
}
