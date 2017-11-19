Shader "Custom/Chunk"
{
	Properties
	{
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
	}
		SubShader{
		Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert addshadow alphatest:_Cutoff

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			float2 uv_MainTex;
		};

		fixed4 _Color;

		void surf(Input IN, inout SurfaceOutput o)
		{
			//does block facing x, y or z direction?
			float3 facePos = ceil(abs(IN.worldNormal));
			float2 uvOffset;
			if (facePos.x > 0)
			{
				uvOffset.x = IN.worldPos.z;
				uvOffset.y = IN.worldPos.y;
			}
			else if (facePos.y > 0)
			{
				uvOffset.x = IN.worldPos.z;
				uvOffset.y = IN.worldPos.x;
			}
			else
			{
				uvOffset.x = IN.worldPos.x;
				uvOffset.y = IN.worldPos.y;
			}
			//From worldPos to tilePos
			uvOffset = (uvOffset - trunc(uvOffset));
			//Negative numbers grow at left side so add 1.
			uvOffset.x = uvOffset.x < 0 ? uvOffset.x + 1 : uvOffset.x;
			uvOffset.y = uvOffset.y < 0 ? uvOffset.y + 1 : uvOffset.y;
			//TODO: It would be good practice to give rows of tile to shader
			uvOffset /= 4;

			float2 uvStart = (floor(IN.uv_MainTex * 4)) / 4;

			o.Albedo = tex2D(_MainTex, uvStart + uvOffset).rgb;

			o.Alpha = tex2D(_MainTex, uvStart + uvOffset).a;


		}
	ENDCG
	}
}
