// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "HQ FPS Weapons/Interactable"
{
	Properties
	{
		_ASEOutlineColor( "Outline Color", Color ) = (0.8962264,0.7633126,0.2832412,0)
		_ASEOutlineWidth( "Outline Width", Float ) = 1
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Albedo("Albedo", 2D) = "white" {}
		_AlbedoTint("Albedo Tint", Color) = (1,1,1,0)
		_MaskMap("Mask Map", 2D) = "white" {}
		[Normal]_NormalMap("Normal Map", 2D) = "bump" {}
		[HDR]_LineColor("Line Color", Color) = (1.991077,1.306363,0.3513666,1)
		_Line("Line", 2D) = "white" {}
		_LineTransparency("Line Transparency", Range( 0 , 1)) = 1
		_LinesSpeed("Lines Speed", Range( 0.1 , 2)) = 1
		_LineSize("Line Size", Range( 0.1 , 2)) = 1
		_LineMask("Line Mask", 2D) = "white" {}
		_LineDetailContrast("Line Detail Contrast", Range( 0 , 5)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ }
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		
		
		
		
		struct Input {
			half filler;
		};
		float4 _ASEOutlineColor;
		float _ASEOutlineWidth;
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += ( v.normal * _ASEOutlineWidth );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			o.Emission = _ASEOutlineColor.rgb;
			o.Alpha = 1;
		}
		ENDCG
		

		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform float4 _AlbedoTint;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float _LineTransparency;
		uniform sampler2D _Line;
		uniform float _LinesSpeed;
		uniform float _LineSize;
		uniform float4 _LineColor;
		uniform sampler2D _LineMask;
		uniform float4 _LineMask_ST;
		uniform float _LineDetailContrast;
		uniform sampler2D _MaskMap;
		uniform float4 _MaskMap_ST;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			float3 tex2DNode2 = UnpackNormal( tex2D( _NormalMap, uv_NormalMap ) );
			float3 NormalMap266 = tex2DNode2;
			o.Normal = NormalMap266;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 tex2DNode1 = tex2D( _Albedo, uv_Albedo );
			o.Albedo = ( _AlbedoTint * tex2DNode1 ).rgb;
			float2 temp_cast_1 = (_LinesSpeed).xx;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float2 temp_cast_2 = (( ase_vertex3Pos.x * _LineSize )).xx;
			float2 panner243 = ( _Time.y * temp_cast_1 + temp_cast_2);
			float2 uv_LineMask = i.uv_texcoord * _LineMask_ST.xy + _LineMask_ST.zw;
			float4 temp_output_31_0 = ( ( tex2D( _Line, panner243 ) * _LineColor ) * tex2D( _LineMask, uv_LineMask ) );
			float3 desaturateInitialColor282 = float3( (tex2DNode2).xy ,  0.0 );
			float desaturateDot282 = dot( desaturateInitialColor282, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar282 = lerp( desaturateInitialColor282, desaturateDot282.xxx, 1.0 );
			float4 lerpResult248 = lerp( float4( 0,0,0,0 ) , ( _LineDetailContrast * temp_output_31_0 ) , float4( saturate( desaturateVar282 ) , 0.0 ));
			float2 uv_MaskMap = i.uv_texcoord * _MaskMap_ST.xy + _MaskMap_ST.zw;
			float4 tex2DNode3 = tex2D( _MaskMap, uv_MaskMap );
			float4 lerpResult278 = lerp( float4( 0,0,0,0 ) , ( ( temp_output_31_0 * float4( 0.245283,0.245283,0.245283,0 ) ) + lerpResult248 ) , tex2DNode3.g);
			float4 Line32 = ( _LineTransparency * lerpResult278 );
			o.Emission = Line32.rgb;
			o.Metallic = tex2DNode3.r;
			o.Smoothness = tex2DNode3.a;
			o.Occlusion = tex2DNode3.g;
			o.Alpha = 1;
			clip( tex2DNode1.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18800
263;243;1955;1072;2.192871;339.2661;1.587156;True;True
Node;AmplifyShaderEditor.PosVertexDataNode;156;-126.6492,834.1282;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;155;-136.8433,997.8064;Float;False;Property;_LineSize;Line Size;9;0;Create;True;0;0;0;False;0;False;1;1;0.1;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;245;-72.38606,1156.592;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;120;-134.506,1077.876;Float;False;Property;_LinesSpeed;Lines Speed;8;0;Create;True;0;0;0;False;0;False;1;1;0.1;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;246;165.1374,902.4351;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;243;321.1847,1004.361;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;116;513.4554,978.0359;Inherit;True;Property;_Line;Line;6;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;510.7338,744.0134;Inherit;True;Property;_NormalMap;Normal Map;4;1;[Normal];Create;True;0;0;0;False;0;False;-1;None;37f1d030c9c59564ab56e7a3719de378;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;11;461.0428,1177.778;Float;False;Property;_LineColor;Line Color;5;1;[HDR];Create;True;0;0;0;False;0;False;1.991077,1.306363,0.3513666,1;1.991077,1.306363,0.3513666,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;29;728.6205,1244.181;Inherit;True;Property;_LineMask;Line Mask;10;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;119;856.8676,1135.108;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;281;880.7221,838.6691;Inherit;False;True;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;1039.332,1140.153;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DesaturateOpNode;282;1113.949,816.5692;Inherit;False;2;0;FLOAT3;1,1,1;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;289;1001.995,1043.164;Float;False;Property;_LineDetailContrast;Line Detail Contrast;11;0;Create;True;0;0;0;False;0;False;1;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;287;1324.248,1090.485;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;258;1308.147,839.1075;Inherit;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;248;1483.231,1022.621;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;285;1493.077,1261.674;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.245283,0.245283,0.245283,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;284;1743.244,1135.52;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;3;1264.116,136.0759;Inherit;True;Property;_MaskMap;Mask Map;3;0;Create;True;0;0;0;False;0;False;-1;None;b78f3e9e16321be498374ca16fd8a2e9;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;296;1959.3,918.964;Float;False;Property;_LineTransparency;Line Transparency;7;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;278;1979.876,1048.127;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;297;2348.64,1010.46;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;290;1361.003,-451.8165;Float;False;Property;_AlbedoTint;Albedo Tint;2;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;32;2603.806,1034.757;Float;False;Line;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;266;1010.118,721.847;Float;False;NormalMap;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;1;1287.312,-258.8968;Inherit;True;Property;_Albedo;Albedo;1;0;Create;True;0;0;0;False;0;False;-1;None;aa571b11c49e7d84b8eab3fab40243c6;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;269;967.874,-130.397;Inherit;False;266;NormalMap;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;291;1655.385,-223.4897;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;265;969.6022,-47.64547;Inherit;False;32;Line;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;50;1825.649,21.80332;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;HQ FPS Weapons/Interactable;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;True;1;0.8962264,0.7633126,0.2832412,0;VertexOffset;False;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;246;0;156;1
WireConnection;246;1;155;0
WireConnection;243;0;246;0
WireConnection;243;2;120;0
WireConnection;243;1;245;0
WireConnection;116;1;243;0
WireConnection;119;0;116;0
WireConnection;119;1;11;0
WireConnection;281;0;2;0
WireConnection;31;0;119;0
WireConnection;31;1;29;0
WireConnection;282;0;281;0
WireConnection;287;0;289;0
WireConnection;287;1;31;0
WireConnection;258;0;282;0
WireConnection;248;1;287;0
WireConnection;248;2;258;0
WireConnection;285;0;31;0
WireConnection;284;0;285;0
WireConnection;284;1;248;0
WireConnection;278;1;284;0
WireConnection;278;2;3;2
WireConnection;297;0;296;0
WireConnection;297;1;278;0
WireConnection;32;0;297;0
WireConnection;266;0;2;0
WireConnection;291;0;290;0
WireConnection;291;1;1;0
WireConnection;50;0;291;0
WireConnection;50;1;269;0
WireConnection;50;2;265;0
WireConnection;50;3;3;1
WireConnection;50;4;3;4
WireConnection;50;5;3;2
WireConnection;50;10;1;4
ASEEND*/
//CHKSM=FF95E22282AE506219CAD42352C9374A59993C1B