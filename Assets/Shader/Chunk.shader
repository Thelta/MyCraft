Shader "Custom/Chunk"
{
	Properties
	{
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		_MainTexArr("Tex", 2DArray ) = "" {}
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
	}
		SubShader{
		Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert addshadow alphatest:_Cutoff vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.5

		sampler2D _MainTex;
		UNITY_DECLARE_TEX2DARRAY(_MainTexArr);

		struct Input
		{
			float2 uv_MainTexArr;
			int texType;
		};

		void vert(inout appdata_full v, out Input data) 
		{
			UNITY_INITIALIZE_OUTPUT(Input, data);
			data.texType = (int) v.texcoord2.x;
		}

		void surf(Input IN, inout SurfaceOutput o)
		{
			float3 realUV = float3(IN.uv_MainTexArr.xy, IN.texType);

			o.Albedo = UNITY_SAMPLE_TEX2DARRAY(_MainTexArr, realUV);
			//o.Albedo = tex2D(_MainTex, realUV.xy).rgb;
			o.Alpha = 1;
		}
	ENDCG
	}
}
