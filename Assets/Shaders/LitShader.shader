Shader "Universe/LitShader"
{
	Properties
	{
		[HDR]_ColorLow("Color Slow Speed", Color) = (0, 0, 0.5, 0.3)
		[HDR]_ColorHigh("Color High Speed", Color) = (1, 0, 0, 0.3)
		[HDR]_ColorPos("Color Position", Color) = (1, 0, 0, 0.3)

		_HighSpeedValue("High speed Value", Range(0, 500)) = 25
	}
		SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct v2f
			{
				float4 position : SV_POSITION;
				float4 color : COLOR;
			};

			struct Particle {
				float3 position;
				float3 velocity;
			};

			StructuredBuffer<Particle> particleBuffer;

			float4 _ColorLow, _ColorHigh, _ColorPos;
			float _HighSpeedValue;

			v2f vert(uint vertex_id : SV_VertexID, uint instance_id : SV_InstanceID)
			{
				v2f o;
				o.position = UnityObjectToClipPos(float4(particleBuffer[instance_id].position, 1.0f));

				float speed = length(particleBuffer[instance_id].velocity);
				float lerpValue = clamp(speed / _HighSpeedValue, 0.0f, 1.0f);
				o.color.rgb = lerp(_ColorLow.rgb, _ColorHigh.rgb, lerpValue);
				o.color.rgb = lerp(o.color.rgb, _ColorPos.rgb, clamp(length(particleBuffer[instance_id].position) / 1000.0f,0.0f,1.0f));
				o.color.a = (_ColorLow.a * _ColorHigh.a);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return i.color;
			}
			ENDCG
		}
	}
}
