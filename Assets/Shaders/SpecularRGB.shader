// Made with Amplify Shader Editor v1.9.2.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "RGB Mapped Specular"
{
	Properties
	{
		_BlueColor("Blue Color", Color) = (0,0,1,1)
		_ColorMask("Color Mask", 2D) = "white" {}
		_GreenColor("Green Color", Color) = (0,1,0,1)
		_RedColor("Red Color", Color) = (1,0,0,1)
		_SpecularTexture("Specular Texture", 2D) = "white" {}
		_SpecularColor("Specular Color", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf StandardSpecular keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _RedColor;
		uniform sampler2D _ColorMask;
		uniform float4 _ColorMask_ST;
		uniform float4 _GreenColor;
		uniform float4 _BlueColor;
		uniform sampler2D _SpecularTexture;
		uniform float4 _SpecularTexture_ST;
		uniform float4 _SpecularColor;

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 uv_ColorMask = i.uv_texcoord * _ColorMask_ST.xy + _ColorMask_ST.zw;
			float4 tex2DNode56 = tex2D( _ColorMask, uv_ColorMask );
			float4 temp_cast_0 = (tex2DNode56.r).xxxx;
			float4 temp_output_1_0_g30 = temp_cast_0;
			float4 color145 = IsGammaSpace() ? float4(1,1,1,1) : float4(1,1,1,1);
			float4 temp_cast_1 = (color145.r).xxxx;
			float4 temp_output_2_0_g30 = temp_cast_1;
			float temp_output_11_0_g30 = distance( temp_output_1_0_g30 , temp_output_2_0_g30 );
			float4 lerpResult21_g30 = lerp( _RedColor , temp_output_1_0_g30 , saturate( ( ( temp_output_11_0_g30 - 0.0 ) / max( 0.0 , 1E-05 ) ) ));
			float4 temp_cast_2 = (tex2DNode56.g).xxxx;
			float4 temp_output_1_0_g33 = temp_cast_2;
			float4 temp_cast_3 = (color145.g).xxxx;
			float4 temp_output_2_0_g33 = temp_cast_3;
			float temp_output_11_0_g33 = distance( temp_output_1_0_g33 , temp_output_2_0_g33 );
			float4 lerpResult21_g33 = lerp( _GreenColor , temp_output_1_0_g33 , saturate( ( ( temp_output_11_0_g33 - 0.0 ) / max( 0.0 , 1E-05 ) ) ));
			float4 temp_cast_4 = (tex2DNode56.b).xxxx;
			float4 temp_output_1_0_g32 = temp_cast_4;
			float4 temp_cast_5 = (color145.b).xxxx;
			float4 temp_output_2_0_g32 = temp_cast_5;
			float temp_output_11_0_g32 = distance( temp_output_1_0_g32 , temp_output_2_0_g32 );
			float4 lerpResult21_g32 = lerp( _BlueColor , temp_output_1_0_g32 , saturate( ( ( temp_output_11_0_g32 - 0.0 ) / max( 0.0 , 1E-05 ) ) ));
			o.Albedo = ( lerpResult21_g30 + lerpResult21_g33 + lerpResult21_g32 ).rgb;
			float2 uv_SpecularTexture = i.uv_texcoord * _SpecularTexture_ST.xy + _SpecularTexture_ST.zw;
			o.Specular = ( tex2D( _SpecularTexture, uv_SpecularTexture ) * _SpecularColor ).rgb;
			float4 color180 = IsGammaSpace() ? float4(0.490566,0.490566,0.490566,0) : float4(0.2054128,0.2054128,0.2054128,0);
			o.Smoothness = color180.r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19202
Node;AmplifyShaderEditor.ColorNode;85;923.9312,-104.79;Inherit;False;Property;_GreenColor;Green Color;2;0;Create;True;0;0;0;False;0;False;0,1,0,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;133;609.7323,-354.0471;Inherit;True;Replace Color;-1;;30;896dccb3016c847439def376a728b869;1,12,0;5;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;145;283.1273,-165.9594;Inherit;False;Constant;_SAMPLER1;SAMPLER;5;0;Create;True;0;0;0;False;0;False;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;74;625.8145,121.5868;Inherit;True;Replace Color;-1;;32;896dccb3016c847439def376a728b869;1,12,0;5;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;56;684.2705,-619.2117;Inherit;True;Property;_ColorMask;Color Mask;1;0;Create;True;0;0;0;False;0;False;-1;f181c9819c5bc7b488b3eedee117d8e0;f181c9819c5bc7b488b3eedee117d8e0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;80;936.1936,-305.71;Inherit;False;Property;_RedColor;Red Color;3;0;Create;True;0;0;0;False;0;False;1,0,0,1;0.8274511,0.1254902,0.8078432,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;177;1814.391,-405.1943;Inherit;True;Property;_SpecularTexture;Specular Texture;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;179;1983.39,-191.9944;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;180;1789.009,19.38913;Inherit;False;Constant;_Color4;Color 4;6;0;Create;True;0;0;0;False;0;False;0.490566,0.490566,0.490566,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;100;610.3962,-119.2751;Inherit;True;Replace Color;-1;;33;896dccb3016c847439def376a728b869;1,12,0;5;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;73;917.8559,126.702;Inherit;False;Property;_BlueColor;Blue Color;0;0;Create;True;0;0;0;False;0;False;0,0,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;32;1448.904,-264.5044;Float;False;True;-1;2;ASEMaterialInspector;0;0;StandardSpecular;Color Masking Test;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;_SpecularHighlights;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.SimpleAddOpNode;173;1205.289,-157.06;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;178;1744.19,-176.3944;Inherit;False;Property;_SpecularColor;Specular Color;5;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
WireConnection;133;1;56;1
WireConnection;133;2;145;1
WireConnection;133;3;80;0
WireConnection;74;1;56;3
WireConnection;74;2;145;3
WireConnection;74;3;73;0
WireConnection;179;0;177;0
WireConnection;179;1;178;0
WireConnection;100;1;56;2
WireConnection;100;2;145;2
WireConnection;100;3;85;0
WireConnection;32;0;173;0
WireConnection;32;3;179;0
WireConnection;32;4;180;0
WireConnection;173;0;133;0
WireConnection;173;1;100;0
WireConnection;173;2;74;0
ASEEND*/
//CHKSM=91F445FA4870F75F56B6AF4194C5218D6A21E584