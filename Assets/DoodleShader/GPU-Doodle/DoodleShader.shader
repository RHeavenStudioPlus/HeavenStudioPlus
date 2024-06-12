Shader "GpuDoodle"{
	Properties{
		_Color("Tint", Color) = (0, 0, 0, 1)
		_MainTex("Texture", 2D) = "white" {}

		// Add properties
		_DoodleMaxOffset("Doodle Max Offset", vector) = (0.005, 0.005, 0, 0)
		_DoodleFrameTime("Doodle Frame Time", Float) = 0.2
		_DoodleFrameCount("Doodle Frame Count", Int) = 24
		_DoodleNoiseScale("Doodle Noise Scale", vector) = (35, 35, 1, 1)
	}

	SubShader{
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent"}

		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite off

		Pass{
			CGPROGRAM

			#include "UnityCG.cginc"
			// Add helper file
			#include "UtilsCG.cginc" 

			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			float4 _MainTex_ST;

			fixed4 _Color;

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata v) {
				v2f o;
				o.position = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			// Add identifiers
			float2 _DoodleMaxOffset;  // - How far the UV can be distorted
			float _DoodleFrameTime;   // - How long does a frame last
			int _DoodleFrameCount;    // - How many frames per animation
			float2 _DoodleNoiseScale; // - How noisy should the effect be

			fixed4 frag(v2f i) : SV_TARGET{

				// Add doodle code
				float2 offset = 0.0;
				offset = DoodleTextureOffset(i.uv, _DoodleMaxOffset, _Time.y, _DoodleFrameTime, _DoodleFrameCount,_DoodleNoiseScale); 

				fixed4 col = tex2D(_MainTex, i.uv + offset);
				col *= _Color;
				return col;
			}

			ENDCG
		}
	}
}