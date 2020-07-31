Shader "Unlit/Ground_Blank"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
        _BackColor("Background Color", color) = (1,1,1,1)
        _HighColor("HightColor", color) = (1,0,0,1)
        _Color("Line Color", color) = (1,1,1,1)
		_Width("Width", range(0,0.5)) = 0.1
        _Cutoff("Alpha Cutoff", Range(0, 1)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            blend srcalpha oneminussrcalpha
			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            // #pragma surface surf Lambert alphatest:_Cutoff
            
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
                float4 pos : SV_POSITION;
                // float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _BackColor;
            fixed4 _HighColor;
            fixed _Width;

            
            
            v2f vert (appdata v)
            {
                v2f o;
                // o.vertex = v.vertex;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = v.normal;
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                // fixed4 col = tex2D(_MainTex, i.uv);
                
                fixed4 col = _BackColor;
				fixed4 mask = _Color;
                mask *= pow(saturate(sin( -0.0005 * (0.5*i.pos.x + i.pos.y) +  _Time.z / 2)),100) * 0.4+0.1;
                
                
				col += saturate(step(i.uv.x, _Width) + step(1 - _Width, i.uv.x) + step(i.uv.y, _Width) + step(1 - _Width, i.uv.y)) * mask;
                
                fixed normalcut = step (abs(i.normal.y),0)*0.03;
                col += _Color * normalcut;

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }

            
            ENDCG
        }
        Pass{
            Name "Cutout"
            AlphaTest Greater 0.6
            SetTexture[_Cutout]{
    
            }
        }
    }
}
