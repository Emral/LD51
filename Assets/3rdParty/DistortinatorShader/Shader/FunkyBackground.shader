// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
Shader "Custom/FunkyBackground"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Amplitude("Wave Amplitude", Range(0,1)) = 0.4
		_Frequency("Wave Frequency", Range(0, 20)) = 2
		_AnimationSpeed("Animation Speed", Range(-4,4)) = 1
		_AnimationPhase("Animation Phase", Range(0,1)) = 0
		_OscilationYAmplitude("Vertical Oscilation Amplitude", Range(0,1)) = 0
		_OscilationYFrequency("Vertical Oscilation Frequency", Range(0, 10)) = 1
		_MoveX("Horizontal Move Speed", Range(-1,1)) = 0
		_MoveY("Vertical Move Speed", Range(-1,1)) = 0

			[HideInInspector]
		_MySrcMode("SrcMode", Int) = 5
			[HideInInspector]
		_MyDstMode("DstMode", Int) = 10

		[Toggle(INTERLACING_ON)] INTERLACING("Interlacing?", Int) = 0
		_InterlaceSize("Interlace size", Int) = 1
		[HideInInspector]
		_Interlace2("Interlace 2", Range(2,10)) = 2
			[Toggle(VERTICAL_WOBBLE_ON)] VERTICAL_WOBBLE("Vertical Wobble?", Int) = 0
			_Distortion("Distortion Power", Range(-10, 10)) = -1
			_DistortionX("Distortion Center X", Range(-10, 10)) = 0
			_DistortionY("Distortion Center Y", Range(-10, 10)) = 0

		[Toggle(NOISE_ON)] NOISING("Noise Distortion?", Int) = 0
		_NoiseLv("Noise Level", Range(0, 1)) = 0
		_NoiseTex ("Noise Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}
		Cull Off
		Lighting Off
		ZWrite Off
		Blend[_MySrcMode][_MyDstMode]

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature INTERLACING_ON
			#pragma shader_feature NOISE_ON
			#pragma shader_feature VERTICAL_WOBBLE_ON
			#pragma shader_feature TILESET_MODE_ON
			#define PI2 6.2832

			#include "UnityCG.cginc"

			struct appdata_t
			{
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 wp : TEXCOORD1;
				fixed4 color : COLOR;
				float4 vertex : SV_POSITION;

			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			float _Amplitude;
			float _Frequency;
			float _AnimationSpeed;
			float _AnimationPhase;
			float _MoveX;
			float _MoveY;
			float _OscilationYAmplitude;
			float _OscilationYFrequency;

			int _InterlaceSize;
			int _Interlace2;
			float _Distortion;
			float _DistortionX;
			float _DistortionY;

			float _NoiseLv;
			sampler2D _NoiseTex;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
				OUT.wp = mul(unity_ObjectToWorld, IN.vertex);
				OUT.color = IN.color;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);

#ifdef UNITY_HALF_TEXEL_OFFSET
				OUT.vertex.xy += (_ScreenParams.zw - 1.0)*float2(-1, 1);
#endif
				return OUT;
			}

			fixed4 frag(v2f IN) : SV_Target
			{ 
				fixed4 c;

				float x_distort = _Distortion * (IN.wp.y - _DistortionY) * (IN.wp.y - _DistortionY) * (IN.wp.x - _DistortionX) / 1000;
				float y_distort = _Distortion * (IN.wp.x - _DistortionX) * (IN.wp.x - _DistortionX) * (IN.wp.y - _DistortionY) / 1000;

				#if NOISE_ON
				x_distort = x_distort + _NoiseLv*tex2D(_NoiseTex, IN.uv)[0];
				y_distort = y_distort + _NoiseLv*tex2D(_NoiseTex, IN.uv)[0];
				#endif
				
				#if INTERLACING_ON
				if (floor(IN.vertex.y) % _Interlace2 < _InterlaceSize)
					c = tex2D(_MainTex,
						IN.uv + float4(
							x_distort + sin(IN.wp.y * _Frequency + _Time.y * _AnimationSpeed + PI2 * _AnimationPhase)* _Amplitude - _MoveX * _Time.y,
							#if VERTICAL_WOBBLE_ON
							y_distort + cos(IN.wp.x * _Frequency + _Time.y * _AnimationSpeed + PI2 * _AnimationPhase)* _Amplitude - _MoveY * _Time.y + 
								sin(IN.wp.y * _OscilationYFrequency + _Time.y * _AnimationSpeed + PI2) * _OscilationYAmplitude,
							#else
							y_distort - _MoveY * _Time.y + 
								sin(IN.wp.y * _OscilationYFrequency + _Time.y * _AnimationSpeed + PI2) * _OscilationYAmplitude,
							#endif
							0, 0)
					) * IN.color;
				else
					c = tex2D(_MainTex,
						IN.uv + float4(
							x_distort - sin(IN.wp.y * _Frequency + _Time.y * _AnimationSpeed + PI2 * _AnimationPhase)* _Amplitude - _MoveX * _Time.y,
							#if VERTICAL_WOBBLE_ON
							y_distort + sin(IN.wp.y * _OscilationYFrequency + _Time.y * _AnimationSpeed + PI2) * _OscilationYAmplitude +
								cos(IN.wp.x * _Frequency + _Time.y * _AnimationSpeed + PI2 * _AnimationPhase) * _Amplitude - _MoveY * _Time.y,
							#else
							y_distort + sin(IN.wp.y * _OscilationYFrequency + _Time.y * _AnimationSpeed + PI2) * _OscilationYAmplitude - 
								_MoveY * _Time.y,
							#endif
							0, 0)
					) * IN.color;
				#else
				c = tex2D(_MainTex, IN.uv + 
						float4(
						x_distort + sin(IN.wp.y * _Frequency + _Time.y * _AnimationSpeed + PI2 * _AnimationPhase) * _Amplitude - _MoveX * _Time.y,
						#if VERTICAL_WOBBLE_ON
						y_distort + cos(IN.wp.x * _Frequency + _Time.y * _AnimationSpeed + PI2 * _AnimationPhase) * _Amplitude - _MoveY * _Time.y +
							sin(IN.wp.y * _OscilationYFrequency + _Time.y * _AnimationSpeed ) * _OscilationYAmplitude,
						#else
						y_distort - _MoveY * _Time.y +
							sin(IN.wp.y * _OscilationYFrequency + _Time.y * _AnimationSpeed ) * _OscilationYAmplitude,
						#endif
						0,0)
				) * IN.color;
				#endif
				c.rgb *= c.a;

				return c;
			}
			ENDCG
		}
	}
	CustomEditor "FunkyBackgroundGUI"
}
