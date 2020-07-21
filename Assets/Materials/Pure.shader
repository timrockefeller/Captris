// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Pure"
{
    Properties
    {
        _Color("Color", color) = (1,1,1,1)

        _RimColor("Rim Color", Color) = (1,1,1,1)
        _RimRang("Rim Range",range(0,1)) = 0.1

        _FaceupFilter("Face-up Filter",range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        LOD 100
        
        
        


        Pass
        {
            Cull Front

            Tags { "Queue" = "Geometry-1" }

            CGPROGRAM

            #include "UnityCG.cginc"

            struct v2f
            {
                float4 vertex:POSITION;
            };

float4 _Color;
            float4 _RimColor;
            float _RimRang;
            v2f vert(appdata_base  v)
            {
                v2f o;
                fixed4 vertex = v.vertex;
                vertex.xyz += v.normal.xyz * _RimRang;
                
                o.vertex = UnityObjectToClipPos(vertex);    
                // o.vertex.y = saturate(o.vertex.y);
                return o;
            }

            fixed4 frag (v2f IN):COLOR
            {    
                return _Color;
            }

            #pragma vertex vert
            #pragma fragment frag

            ENDCG
        }
        
        
        Pass
        {
            Tags {"Queue"="Transparent"}
            blend srcalpha oneminussrcalpha
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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
            };
            float4 _MainTex_ST;
           
            fixed4 _Color;
            fixed _FaceupFilter;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = v.normal;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                
                fixed4 col = _Color;
                
                // cut up-face
                col.a = saturate(saturate(i.normal.y) + (_FaceupFilter ));
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
