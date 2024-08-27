Shader "Custom/MyShader"
{
    Properties
    {
        _MyDynamicValue ("Dynamic Value", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

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

            float _MyDynamicValue;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // ��������м��㲢����һЩ��ɫ
                float dynamicValue = sin(_MyDynamicValue) * 0.5 + 0.5; // �� _MyDynamicValue ����� 0-1 ��Χ��ֵ
                _MyDynamicValue = _Time.y;
                return fixed4(dynamicValue, 0.0, 0.0, 1.0); // ��ɫ�仯
            }
            ENDCG
        }
    }
}