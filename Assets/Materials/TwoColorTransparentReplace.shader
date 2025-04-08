Shader "Custom/TwoColorTransparentReplace"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}

        _Color1("Primary Color", Color) = (1,1,1,1)
        _Color2("Secondary Color", Color) = (0,0,0,1)

        _ReplaceColor1("Replace Color 1", Color) = (1,0,0,1)
        _ReplaceColor2("Replace Color 2", Color) = (0,1,0,1)

        _Threshold("Threshold", Range(0, 0.5)) = 0.05
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
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
            float4 _Color1;
            float4 _Color2;
            float4 _ReplaceColor1;
            float4 _ReplaceColor2;
            float _Threshold;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 texColor = tex2D(_MainTex, i.uv);

                float d1 = distance(texColor.rgb, _Color1.rgb);
                float d2 = distance(texColor.rgb, _Color2.rgb);

                if (d1 < _Threshold)
                {
                    return float4(_ReplaceColor1.rgb, texColor.a); // Zamieñ kolor 1
                }
                else if (d2 < _Threshold)
                {
                    return float4(_ReplaceColor2.rgb, texColor.a); // Zamieñ kolor 2
                }
                else
                {
                    return float4(0, 0, 0, 0); // Przezroczyste
                }
            }
            ENDCG
        }
    }
}