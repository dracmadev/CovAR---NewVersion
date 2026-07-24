Shader "Custom/RadialSectorSpinner"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Angle ("Angle Centre", Range(0, 360)) = 0
        _ArcSize ("Mida del Sector (Graus)", Range(1, 360)) = 60 // Ample de la franja visible
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }

    SubShader
    {
        Tags
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
            };
            
            fixed4 _Color;
            sampler2D _MainTex;
            float _Angle;
            float _ArcSize;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // 1. Texturem el píxel real de la imatge
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;

                // 2. Calculem l'angle en graus des del centre (0.5, 0.5)
                float2 dir = IN.texcoord - float2(0.5, 0.5);
                float pixelAngle = atan2(dir.y, dir.x) * 57.29578; // Convertim a graus
                if (pixelAngle < 0) pixelAngle += 360.0;

                // 3. Calculem la distància angular entre el píxel i el centre del nostre sector (_Angle)
                float angleDiff = abs(pixelAngle - _Angle);
                
                // Gestionem el salt de 360 a 0 graus per a que la rotació sigui fluida en el punt d'unió
                if (angleDiff > 180.0)
                {
                    angleDiff = 360.0 - angleDiff;
                }

                // 4. Si el píxel està fora de la meitat de l'amplada del sector, el descartem
                float halfArc = _ArcSize * 0.5;
                if (angleDiff > halfArc)
                {
                    discard;
                }

                c.rgb *= c.a;
                return c;
            }
        ENDCG
        }
    }
}