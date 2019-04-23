Shader "Custom/Chunk"
{
	Properties
	{
		_MainTexArr("Texture Array", 2DArray ) = "" {}
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
		_AlphaTextureCount("Alpha Texture Count", Int) = 3
		_AlphaTextureColorifyCutoff("Alpha Texture Colorify Cutoff", Range(0,1)) = 0.5
		_WaterTex("Water Texture Index", Int) = 11
	}

	SubShader
	{
		Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert addshadow alphatest:_Cutoff vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.5

		UNITY_DECLARE_TEX2DARRAY(_MainTexArr);
		int _AlphaTextureCount;
		float _AlphaTextureColorifyCutoff;
		int _WaterTex;

		static const float3 biomeColors[3] = { float3(0.521, 0.807, 0.353), float3(0.521, 0.807, 0.353), float3(0.521, 0.807, 0.353) };

		struct Input
		{
			float2 uv_MainTexArr;
			float3 worldPos;
			float texType;
			float biomeType;
		};

		void vert(inout appdata_full v, out Input data) 
		{
			UNITY_INITIALIZE_OUTPUT(Input, data);
			data.texType = v.texcoord2.x;
			data.biomeType = v.texcoord2.y;
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
			float3 pixelColorHsv = rgb2hsv(pixelColor.rgb);

			baseColorHsv.z = pixelColorHsv.z * baseColorHsv.z;

			return hsv2rgb(baseColorHsv);

		}

#define SIN1(x, t) (0.04 * (sin(x * 3.5 + t * 0.35) + sin(x * 4.8 + t * 1.05) + sin(x * 7.3 + t * 0.45)))
#define SIN2(x, t) (0.04 * (sin(x * 4.0 + t * 0.5) + sin(x * 6.8 + t * 0.75) + sin(x * 11.3 + t * 0.2)))

		void surf(Input IN, inout SurfaceOutput o)
		{
			int texType = round(IN.texType);
			int biomeType = round(IN.biomeType);
			float2 uv = IN.uv_MainTexArr;

			if (texType == _WaterTex)	//simple water animation
			{
				//Using worldPos instead of uv, because if we use uv, animations won't connect correctly to other tiles/planes.
				uv.x += SIN1(IN.worldPos.y, _Time.z);
				uv.y -= SIN2(IN.worldPos.z, _Time.z);
			}

			float3 realUV = float3(uv, texType);

			float4 textureVals = UNITY_SAMPLE_TEX2DARRAY(_MainTexArr, realUV);
			float3 textureColor = textureVals.rgb;
			if (texType < _AlphaTextureCount && textureVals.a > _AlphaTextureColorifyCutoff)
			{
				float3 baseColor = biomeColors[biomeType];
				textureColor = colorify(baseColor, textureColor);
			}

			
			o.Albedo = textureColor;
			o.Alpha = textureVals.a;
		}

	ENDCG
	}
}
