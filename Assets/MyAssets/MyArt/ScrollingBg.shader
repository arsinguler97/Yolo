Shader "Custom/ScrollingBG_SoftBlend"
{
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _ScrollSpeed ("Scroll Speed", float) = 0.05
        _BlendWidth ("Edge Fade", Range(0,0.05)) = 0.01
    }
    SubShader {
        Tags { "Queue"="Background" }
        Pass {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _ScrollSpeed;
            float _BlendWidth;

            fixed4 frag(v2f_img i) : SV_Target
            {
                float2 uv = i.uv;
                float t = _Time.y * _ScrollSpeed;

                uv.x -= t;
                uv.y += t;
                uv = frac(uv);

                fixed4 col = tex2D(_MainTex, uv);

                // Kenar fade
                float fadeX = smoothstep(0, _BlendWidth, uv.x) * smoothstep(1, 1 - _BlendWidth, uv.x);
                float fadeY = smoothstep(0, _BlendWidth, uv.y) * smoothstep(1, 1 - _BlendWidth, uv.y);
                float edgeFade = fadeX * fadeY;

                return col * edgeFade;
            }
            ENDCG
        }
    }
}
