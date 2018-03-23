Shader "PostProd/VintageScreen"
{
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_RampTex("Base (RGB)", 2D) = "grayscaleRamp" {}
	}

	SubShader{
		Pass{
		ZTest Always Cull Off ZWrite Off

		CGPROGRAM
	#pragma vertex vert_img
	#pragma fragment frag
	#include "UnityCG.cginc"

		uniform sampler2D _MainTex;
		uniform sampler2D _RampTex;
		uniform half _RampOffset;
		uniform half _DryMix;
		uniform half _CurveMix;
		uniform half _VignetteMix;
		uniform half _DistortAmp;
		uniform half _ChromaticAmp;
		uniform half _ScanLineMix;
		uniform half _RMix;
		uniform half _GMix;
		uniform half _BMix;


		half4 _MainTex_ST;


		float2 curve(float2 uv)
		{
			uv = (uv - 0.5) * 2.0;
			uv *= 1.1;
			uv.x *= 1.0 + pow((abs(uv.y) / 5.0), 2.0);
			uv.y *= 1.0 + pow((abs(uv.x) / 4.0), 2.0);
			uv = (uv / 2.0) + 0.5;
			uv = uv *0.92 + 0.04;
			return uv;
		}

		fixed4 frag(v2f_img i) : SV_Target
		{
			//	fixed4 original = tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST));
			float3 col;
			float2 uv = i.uv;

			// Torsion ecran vieux
			uv = lerp(uv, curve(uv), _CurveMix); ;
			float x = _DistortAmp*sin(0.3*_Time.x + uv.y*21.0)*sin(0.7*_Time.x + uv.y*29.0)*sin(0.3 + 0.33*_Time.x + uv.y*31.0)*0.0017;
			float2 fuv = UnityStereoScreenSpaceUVAdjust(uv, _MainTex_ST);
			col = tex2D(_MainTex, float2(fuv.x, fuv.y));
			float3 oricol = col;


			// l'abbération chromatique
			float3 colAbbered;
			colAbbered.r = tex2D(_MainTex, float2(x + fuv.x + 0.001, fuv.y + 0.001)).x + 0.05;
			colAbbered.g = tex2D(_MainTex, float2(x + fuv.x + 0.000, fuv.y - 0.002)).y + 0.05;
			colAbbered.b = tex2D(_MainTex, float2(x + fuv.x - 0.002, fuv.y + 0.000)).z + 0.05;
			colAbbered.r += 0.08*tex2D(_MainTex, 0.75*float2(x + 0.025, -0.027) + float2(fuv.x + 0.001, fuv.y + 0.001)).x;
			colAbbered.g += 0.05*tex2D(_MainTex, 0.75*float2(x + -0.022,  -0.02) + float2(fuv.x + 0.000, fuv.y - 0.002)).y;
			colAbbered.b += 0.08*tex2D(_MainTex, 0.75*float2(x + -0.02, -0.018) + float2(fuv.x - 0.002, fuv.y + 0.000)).z;
			col = lerp(col, colAbbered, _ChromaticAmp);

			// Desaturation ou j'sais pas trop quoi ????
			col = clamp(col*0.6 + 0.4*col*col*1.0, 0.0, 1.0);

			// Vignette -OK
			float vig = pow(0.0 + 1.0*16.0*uv.x*uv.y*(1.0 - uv.x)*(1.0 - uv.y), 0.3f);
			col = lerp(col, col * float3(vig, vig, vig), _VignetteMix);

			// Gamma ???
			col *= float3(0.95, 1.05, 0.95);
			col *= 2.8;

			// Scanlines
			float scans = clamp(0.35 + 0.35*sin(3.5*_Time.x + uv.y*_ScreenParams.y*1.5), 0.0, 1.0);
			float s = pow(scans, 1.7);
			float3 col2 = col*float3(0.4 + 0.7*s, 0.4 + 0.7*s, 0.4 + 0.7*s);
			col = lerp(col, col2, _ScanLineMix);


			col *= 1.0 + 0.01*sin(110.0*_Time.x);
			if (uv.x < 0.0 || uv.x > 1.0)
				col *= 0.0;
			if (uv.y < 0.0 || uv.y > 1.0)
				col *= 0.0;

			//float AA = (mod(_ScreenParams.y, 2.0) - 1.0)*2.0;
			float AA = ((_ScreenParams.y% 2.0) - 1.0)*2.0;
			col *= 1.0 - 0.65*float3(clamp(AA, 0.0, 1.0), clamp(AA, 0.0, 1.0), clamp(AA, 0.0, 1.0));


			//fixed4 output = lerp(float4(oricol, 1.f), float4(col, 1.0f), 0.5f);
			fixed4 output = lerp(float4(oricol, 1.f), float4(col, 1.0f), _DryMix);
			output = output * float4(_RMix, _GMix, _BMix, 1.0f);
			return output;
			}
				ENDCG

			}
	}

		Fallback off

}
