Shader "Unlit/BuffEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color( "Buff Color ", color) = (1,0.5,0.5,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue" = "Transparent" "IgnoreProjector"="True" }
        LOD 100

        Pass
        {
            
            Cull Off
            Tags {"Queue"="Transparent" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            float random (float2 st, float n) {
                st = floor(st * n);
                return frac(sin(dot(st.xy, float2(12.9898,78.233)))*43758.5453123);
            }

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = _Color;
                float c = random(i.uv.xx+_Time.x,20);
                col.a = c;
                col.a*= pow(i.uv.y*c,3);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                // return fixed4(i.uv,0,1);
                return col;
            }
            ENDCG
        }
    }
}
