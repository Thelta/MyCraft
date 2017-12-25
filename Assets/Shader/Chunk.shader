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
			float texType;
		};

		void vert(inout appdata_full v, out Input data) 
		{
			UNITY_INITIALIZE_OUTPUT(Input, data);
			data.texType = v.texcoord2.x;
		}

		float3 rgb2hsv(float3 c) 
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
			float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

			float d = q.x - min(q.w, q.y);
			float e = 1.0e-10;
			return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}

		float3 hsv2rgb(float3 c) 
		{
			c = float3(c.x, clamp(c.yz, 0.0, 1.0));
			float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
			float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
			return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
		}

		float3 colorify(float3 baseColor, float3 pixelColor)
		{
			float3 baseColorHsv = rgb2hsv(baseColor);
			float3 pixelColorHsv = rgb2hsv(pixelColor);

			baseColorHsv.z = pixelColorHsv.z * baseColorHsv.z;

			return hsv2rgb(baseColorHsv);

		}

		void surf(Input IN, inout SurfaceOutput o)
		{
			//IN.texType = 4;
			float3 realUV = float3(IN.uv_MainTexArr, IN.texType);

			float3 textureColor = UNITY_SAMPLE_TEX2DARRAY(_MainTexArr, realUV);
			if (IN.texType < 2)
			{
				float3 baseColor = float3(0.521, 0.807, 0.353);
				textureColor = colorify(baseColor, textureColor);
			}
			//o.Albedo = tex2D(_MainTex, realUV.xy).rgb;
			o.Albedo = textureColor;
			o.Alpha = 1;
		}






	ENDCG
	}
}
