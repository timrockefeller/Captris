// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Pure"
{
    Properties
    {
        _Color("Color", color) = (1,1,1,1)

        _RimColor("Rim Color", Color) = (1,1,1,1)
        _RimRang("Rim Range",range(0,1)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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

            float4 _RimColor;
            float _RimRang;

            v2f vert(appdata_base  v)
            {
                v2f o;
                fixed4 vertex = v.vertex;
                vertex.xyz+=v.normal.xyz*_RimRang;
                
                o.vertex = UnityObjectToClipPos(vertex);    

                return o;
            }

            fixed4 frag (v2f IN):COLOR
            {    
                return _RimColor;
            }

            #pragma vertex vert
            #pragma fragment frag

            ENDCG
        }
        
        
        


        
        Pass
        {
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
            float4 _MainTex_ST;
           
            fixed4 _Color;
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
                // fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 col = _Color;

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
