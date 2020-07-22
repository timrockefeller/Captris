// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/PureGeometry"
{
    Properties
    {
        _Color("Color", COLOR) = (1,1,1,1)

        _MainTex("Texture", 2D) = "white" {}
        _FaceupFilter("Face-up Filter",range(0,1)) = 1
        _RimRang("Rim Range",range(0,1)) = 0.1
    }

    Subshader 
    {
        Tags
        { 
            "RenderType" = "Opaque" 
            "PerformanceChecks" = "False" 
        }
        Pass
        {
            Cull Off

            Tags { "Queue" = "Transparent" }
            blend srcalpha oneminussrcalpha
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
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
                UNITY_FOG_COORDS(1)
                float4 vertex:POSITION;
                float3 normal : NORMAL;
            };

            fixed4 _Color;
            fixed _RimRang;
            fixed _GOpacity;
            v2f vert(appdata  v)
            {
                v2f o;
                fixed4 vertex = v.vertex;
                vertex.xz += v.normal.xz * _RimRang;
                vertex.y = vertex.y/1.1;
                o.vertex = UnityObjectToClipPos(vertex);
                o.normal = v.normal;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i): SV_Target
            {   
                fixed4 col = _Color;
                // col.a = _GOpacity * saturate(abs(i.normal.x)+abs(i.normal.z));
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }

            ENDCG
        }
        
        Pass 
        {
            Name "Diffuse"
            Tags{ "Queue" = "Geometry" "LightMode" = "ForwardBase" }
            Lighting Off

            CGPROGRAM
                #pragma multi_compile_fwdbase
                #pragma vertex Vert
                #pragma fragment Frag
                #include "UnityCG.cginc"
                #include "AutoLight.cginc"

                uniform half4 _Color;
                uniform sampler2D _MainTex;
                uniform half4 _MainTex_ST;

                struct V2f {
                    half4 pos : SV_POSITION;
                    half2 uv : TEXCOORD0;
                    SHADOW_COORDS(1)
                };

                V2f Vert(appdata_base v)
                {
                    V2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

                    TRANSFER_SHADOW(o);

                    return o;
                }

                half4 Frag(V2f i) : SV_Target
                {
                    half4 tex = tex2D(_MainTex, i.uv) * _Color;

                    return tex * (step(0.5,SHADOW_ATTENUATION(i))*0.5+0.5);
                }
            ENDCG
        }
        
        Pass
        {
            Name "ShadowCaster"
            Tags
            { 
                "LightMode" = "ShadowCaster" 
                "IgnoreProjector" = "True"
            }

            ZWrite On

            CGPROGRAM
                #pragma target 3.0

                #pragma shader_feature _ALPHAPREMULTIPLY_ON
                #pragma multi_compile_shadowcaster

                #pragma vertex vertShadowCaster
                #pragma fragment fragShadowCaster

                #include "UnityStandardShadow.cginc"

            ENDCG
        }
    }
}
