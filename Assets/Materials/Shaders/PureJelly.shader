// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/PureJelly"
{
    Properties
    {
        _Color("Color", COLOR) = (1,1,1,1)
        _MainTex("Texture", 2D) = "white" {}
        _GOpacity("Rim Opacity", Range(0,1)) = 0
        // _Width("Border", Range(0,1)) = 0
    }

    Subshader 
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }

        Pass 
        {
            Name "Diffuse"
            Tags{ "Queue" = "Transparent"  "LightMode" = "ForwardBase" }
            Lighting Off
            Cull Off
            blend srcalpha oneminussrcalpha
            CGPROGRAM
                #pragma multi_compile_fwdbase
                #pragma vertex Vert
                #pragma fragment Frag
                #include "UnityCG.cginc"
                #include "AutoLight.cginc"

                uniform half4 _Color;
                uniform sampler2D _MainTex;
                uniform half4 _MainTex_ST;
                // fixed _Width;
                struct V2f {
                    half3 normal : NORMAL;
                    half4 pos : SV_POSITION;
                    half2 uv : TEXCOORD0;
                };

                V2f Vert(appdata_base v)
                {
                    V2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                    o.normal = v.normal;
                    return o;
                }

                half4 Frag(V2f i) : SV_Target
                {
                    half4 tex = tex2D(_MainTex, i.uv) * _Color;
                    
                    // 优化if
                    // if(i.normal.y<0) discard;
                    return tex * half4(1,1,1,0.5);
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
