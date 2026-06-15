Shader "Custom/FluidraWaterUI"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _NoiseTex ("Textura de Soroll/Aigua (2D)", 2D) = "white" {}
        
        _FluidraBlue ("Blau Fluidra", Color) = (0.0, 0.733, 0.882, 1.0)
        _DeepBlue ("Blau Profund", Color) = (0.0, 0.55, 0.70, 1.0)
        
        _SpeedX1 ("Velocitat X Ona 1", Float) = 0.03
        _SpeedY1 ("Velocitat Y Ona 1", Float) = 0.02
        _SpeedX2 ("Velocitat X Ona 2", Float) = -0.02
        _SpeedY2 ("Velocitat Y Ona 2", Float) = 0.04
        
        _DistortionPower ("Força del Líquid", Range(0, 0.1)) = 0.03
    }
    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
        }
        
        Cull Off Lighting Off ZWrite Off ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

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
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            
            fixed4 _FluidraBlue;
            fixed4 _DeepBlue;
            
            float _SpeedX1;
            float _SpeedY1;
            float _SpeedX2;
            float _SpeedY2;
            float _DistortionPower;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Creem dos desplaçaments de temps diferents per a les textures
                float2 uvTime1 = i.uv * _NoiseTex_ST.xy + float2(_Time.y * _SpeedX1, _Time.y * _SpeedY1);
                float2 uvTime2 = i.uv * _NoiseTex_ST.xy + float2(_Time.y * _SpeedX2, _Time.y * _SpeedY2);

                // Llegim la textura de soroll en els dos sentits
                float noise1 = tex2D(_NoiseTex, uvTime1).r;
                float noise2 = tex2D(_NoiseTex, uvTime2).g;
                
                // Ajuntem els dos sorolls per crear un patró de moviment completament orgànic i fluid
                float combinedNoise = (noise1 + noise2) * 0.5;

                // Utilitzem aquest soroll combinat per distorsionar les UVs de fons
                float2 distortedUV = i.uv + (float2(combinedNoise, combinedNoise) - 0.5) * _DistortionPower;

                // Creem el degradat net de Fluidra sobre les coordenades distorsionades pel líquid
                float gradientFactor = (distortedUV.x + (1.0 - distortedUV.y)) * 0.5;
                fixed4 finalColor = lerp(_FluidraBlue, _DeepBlue, gradientFactor * 0.4);

                // Opcional: li donem un mini reflex de llum basat en el propi relleu de l'aigua
                finalColor += smoothstep(0.6, 0.8, combinedNoise) * 0.06;

                return finalColor * i.color;
            }
            ENDCG
        }
    }
}