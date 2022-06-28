
Shader "DC/TimeKeeper"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _BgTex ("Bg Texture", 2D) = "gold" {}
        [NoScaleOffset] _BumpMap ("Bump Tex", 2D) = "bump" {}
        [Toggle] _FlipBumpMap ("Flip Bump Map", float) = 0
        _ReplaceColor ("Replace Color", Color) = (1,0,1)
        [Toggle] _NoUseBump ("No Use Bump", float) = 0
    }
    SubShader
    {
        Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "CanUseSpriteAtlas"="true" "PreviewType"="Plane" }
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 wNormal : TEXCOORD1;
				float3 wTangent : TEXCOORD2;
				float3 wBitangent : TEXCOORD3;
            };
            float2 _BumpOffset;
            float4 _MainTex_TexelSize;
            
            sampler2D _BgTex;
            sampler2D _MainTex;
            sampler2D _BumpMap;
            fixed4 _ReplaceColor;
            float4 _BumpMap_TexelSize;
            fixed _NoUseBump;
            fixed _FlipBumpMap;
            fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.wNormal = UnityObjectToWorldNormal(v.normal);
				o.wTangent = UnityObjectToWorldNormal(v.tangent);
				o.wBitangent = cross(-o.wTangent, o.wNormal) * v.tangent.w;
                return o;
            }
            

            fixed3 diffuse(v2f i, fixed3 col, fixed facing)
            {
                i.uv.x = _FlipBumpMap ? 1 - i.uv.x : i.uv.x;
                float3 normalTex = normalize(tex2D(_BumpMap, i.uv) * 2 - 1);
                //normalTex.z *= facing;
				float3 N = normalize(i.wTangent) * normalTex.r + normalize(i.wBitangent) * normalTex.g + normalize(i.wNormal) * normalTex.b;
                
                half3 toonLight = saturate(dot(N, _WorldSpaceLightPos0)) > 0.3 ? _LightColor0 : unity_AmbientSky;
                return col * (toonLight);
            }

            fixed4 frag (v2f i, fixed facing : VFACE) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed s = col.r > 0.7 ? col.b > 0.7 ? col.g < 0.5 ? 0 : 1 : 1 : 1;
                fixed4 col2 = s == 0 ? tex2D(_BgTex, i.uv) : col;
                col2.a = s == 0 ? 1 - col.g : col2.a;
                col2 = col2 * _Color;
                clip(col2.a - 0.01);
                clip(col.a - 0.01);
                fixed3 dcol = _NoUseBump ? col2 : diffuse(i, col2, facing);
                return fixed4(dcol, col2.a);
            }
            ENDCG
        }
    }
}
