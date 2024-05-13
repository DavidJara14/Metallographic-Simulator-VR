Shader "Unlit/Rayas"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _RotationVec("Rotation Vector", Vector) = (0,0,0,0)
        _AlphaValue("Alpha Value", Range(80, 800)) = 80
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
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
                float4 _RotationVec;
                float _AlphaValue;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);

                    float2 uv = v.uv - 0.5;
                    float cosRot = cos(_RotationVec.x);
                    float sinRot = sin(_RotationVec.y);
                    float2x2 rotMatrix = float2x2(cosRot, -sinRot, sinRot, cosRot);
                    uv = mul(uv, rotMatrix);
                    o.uv = uv + 0.5;

                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv);
                    col.a = _AlphaValue / 800.0; // Normaliza el valor alfa al rango [0, 1]
                    return col;
                }
                ENDCG
            }
        }
}
