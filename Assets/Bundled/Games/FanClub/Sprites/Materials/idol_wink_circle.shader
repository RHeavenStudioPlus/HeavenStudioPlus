Shader "Sprites/NtrIdolCircle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SDFThreshold ("Opacity Threshold", Float) = 0.5
        _Tint ("Tint", Color) = (1,1,1,1)
        _ColorOut ("Outer Colour", Color) = (1,1,1,1)
        _ColorIn ("Inner Colour", Color) = (0,0,0,0)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }
    SubShader
    {
        Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

        Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 texcoord  : TEXCOORD0;
                float4 color    : COLOR;
                float4 vertex : SV_POSITION;
            };

            fixed4 _ColorOut;
            fixed4 _ColorIn;
            fixed4 _Tint;
            float _SDFThreshold;

            v2f vert (appdata IN)
            {
                v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Tint;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
            }

            sampler2D _MainTex;
            sampler2D _AlphaTex;
			float _AlphaSplitEnabled;

            fixed4 SampleSpriteTexture (v2f i)
			{
				fixed4 col;
                fixed4 tex = tex2D(_MainTex, i.texcoord);
 
                col.rgb = i.color.rgb * tex.rgb;
                col.a = i.color.r * tex.r * _SDFThreshold;
                return col;
			}

			fixed4 frag(v2f i) : SV_Target
			{
                fixed4 outCol;

				fixed4 c = SampleSpriteTexture(i);
                outCol = lerp(_ColorIn, _ColorOut, c.r);
                outCol.a = i.color.r * c.r;
                return outCol;
            }
            ENDCG
        }
    }
}
