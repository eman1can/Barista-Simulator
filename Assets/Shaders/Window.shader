Shader "Unlit/Window"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Size ("Size", float) = 1
        _Distortion ("Distortion", range(-5, 5)) = 0
        _T ("Time", float) = 0
        _Blur ("Blur", range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque"}
        LOD 100
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #define S(a, b, t) smoothstep(a, b, t)
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                //float4 grabUv : TEXCOORD1;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _GrabTexture;
            float4 _MainTex_ST;
            float _Size, _T, _Distortion, _Blur;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                //o.grabUv = UNITY_PROJ_COORD(ComputeGrabScreenPos(o.vertex));
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            /*float3 Layer(float2 UV, float t)
            {
                float2 aspect = float2(2, 1);
                float2 uv = UV * _Size * aspect;
                float2 id = floor(uv);
                uv.y += t * 0.25;
                
                float2 gv = frac(uv) - 0.5;
              

                float n = N21(id);
                t += n * 6.28;
                float w = UV.y * 7;
                float x = (n - 0.5) * 0.8;
                x += (0.4 - abs(x)) * sin(3 * w) * pow(sin(w), 6) * 0.45; // sin(2.7 * w) * pow(cos(sin(w)), 2) * 0.45;
                float y = -sin(t + sin(t + sin(t) * 0.5)) * 0.45;

                y -= (gv.x - x) * (gv.x - x);

                float2 dropPos = (gv - float2(x, y)) / aspect;
                float drop = S(0.05, 0.03, length(dropPos));

                float2 trailPos = (gv - float2(x, t * 0.25)) / aspect;
                trailPos.y = (frac(trailPos.y * 8) - 0.5) / 8;
                float trail = S(0.03, 0.01, length(trailPos));

                float fogTrail = S(-0.05, 0.05, dropPos.y);
                fogTrail *= S(0.5, y, gv.y);

                trail *= fogTrail;
                fogTrail *= S(0.05, 0.04, abs(dropPos.x));

                float2 offs = drop * dropPos + trail * trailPos;
                return float3(offs, fogTrail);
            }*/
            
            fixed4 frag (v2f i) : SV_Target
            {
                float t = fmod(_Time.y + _T, 7200);
                
                float4 col = tex2D(_MainTex, i.uv);

                float2 aspect = float2(2, 1);
                float2 uv = i.uv * _Size * aspect;
                uv.y += t * 0.28;
                float2 gv = frac(uv) - 0.5;
              
                float w = i.uv.y * 7;
                float x = sin(2.7 * w) * pow(cos(sin(w)), 2) * 0.45;
                float y = -sin(t + sin(t + sin(t) * 0.5)) * 0.45;
                y -= (gv.x - x) * (gv.x - x);

                float2 dropPos = (gv - float2(x, y)) / aspect;
                float drop = S(0.05, 0.03, length(dropPos));

                float2 trailPos = (gv - float2(x, t * 0.28)) / aspect;
                trailPos.y = (frac(trailPos.y * 8) - 0.5) / 8;
                float trail = S(0.03, 0.01, length(trailPos));

                trail *= S(-0.05, 0.05, dropPos.y);
                trail *= S(0.5, y, gv.y);

               
                col += trail;
                col += drop;

                if (gv.x > 0.48 || gv.y > 0.49) col = float4(1, 0, 0, 1);

                return col;
            }
            ENDCG
        }
    }
}
