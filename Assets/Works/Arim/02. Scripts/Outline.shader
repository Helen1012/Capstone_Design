Shader "Custom/Outline"
{
	Properties
	{
		_MainTex("Albedo", 2D) = "white" {}
		_BumpMap("BumpMap", 2D) = "bump" {}
		_OutlineColor("OutlineColor", Color) = (1,1,1,1)
		_Outline("Outline", Range(0.0005, 0.01)) = 0.01
	}

		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			Cull front

			// Pass1
			CGPROGRAM
			#pragma surface surf NoLighting vertex:vert noshadow noambient

			sampler2D _MainTex;
			sampler2D _BumpMap;
			struct Input {
				float2 uv_MainTex;
				float2 uv_BumpMap;
			};

			fixed4 _OutlineColor;
			float _Outline;

			void vert(inout appdata_full v)
			{
				v.vertex.xyz += v.normal.xyz * _Outline;
			}

			void surf(Input In, inout SurfaceOutput o)
			{
			}

			fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
			{
				return _OutlineColor;
			}
			ENDCG

				// Pass2
				Cull back
				CGPROGRAM
				#pragma surface surf Toon

				fixed4 _Color;
				sampler2D _MainTex;
				sampler2D _BumpMap;
				struct Input {
					float2 uv_MainTex;
					float2 uv_BumpMap;
					fixed4 color : Color;
				};

				void surf(Input In, inout SurfaceOutput o)
				{
					fixed4 c = tex2D(_MainTex, In.uv_MainTex);
					o.Albedo = c.rgb;
					o.Normal = UnpackNormal(tex2D(_BumpMap, In.uv_BumpMap));
					o.Alpha = c.a;
				}

				fixed4 LightingToon(SurfaceOutput s, fixed3 lightDir, fixed atten)
				{
					fixed halfLambert = dot(s.Normal, lightDir) * 0.5 + 0.5;
					halfLambert = ceil(halfLambert * 5) / 5;

					fixed4 final;
					final.rgb = s.Albedo * halfLambert * _LightColor0.rgb;
					final.a = s.Alpha;
					return final;
				}
				ENDCG
		}
			FallBack "Diffuse"
}


