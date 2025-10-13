Shader "UI/InverseMask"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _MaskTex("Mask Texture", 2D) = "white" {}
        _MaskPos("Mask Position", Vector) = (0.5,0.5,0,0)
        _MaskSize("Mask Size", Vector) = (0.2,0.2,0,0)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100
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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float2 _MaskPos;
            float2 _MaskSize;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // Compute normalized distance from mask center
                float2 maskUV = (uv - _MaskPos) / _MaskSize;

                // Check if inside rectangle (both x and y in -0.5..0.5 range)
                float inside = step(-0.5, maskUV.x) * step(maskUV.x, 0.5) *
                               step(-0.5, maskUV.y) * step(maskUV.y, 0.5);

                // If inside the rectangle, make transparent (alpha = 0)
                float alpha = 1.0 - inside;

                fixed4 col = tex2D(_MainTex, uv);
                col.a = col.a * alpha;
                return col;
            }
            ENDCG
        }
    }
}
