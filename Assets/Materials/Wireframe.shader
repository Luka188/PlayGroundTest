// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/wireframe" {
	Properties{


		_Color("Fill Color",Color) = (0,0,0,1)
		_Color1("Wire Color",Color) = (1,1,1,1)
		_Boundry("Wire Boundry",Range(0,0.5)) = 0.455

	}
		SubShader{
		Tags{ "Queue" = "Transparent" }
		ZWrite ON
		Blend SrcAlpha OneMinusSrcAlpha

		Pass{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

	uniform float4 _Color;
	uniform float4 _Color1;

	uniform float _Boundry;


	struct vertexInput {
		float4 vertex : POSITION;
		float4 uv : TEXCOORD0;
		float4 color : COLOR;
	};

	struct fragmentInput {
		float4 position : SV_POSITION;
		float4 uv : TEXCOORD0;
		float4 color:COLOR;

	};

	fragmentInput vert(vertexInput i) {
		fragmentInput o;
		o.position = UnityObjectToClipPos(i.vertex);


		o.uv = i.uv;
		o.color = i.color;

		return o;
	}

	fixed4 frag(fragmentInput i) : SV_Target{

	float4 c;
	//float coord = step(_Boundry, 0.25*(((i.uv.x) * 128)%2)) || step(_Boundry, 0.25*((i.uv.y * 128) % 2)); //1 if i.uv.x is inside boundry edges
	float wave = 0.1*sin(_Time[1] / 2 + (i.uv.x + i.uv.y) * 30);
	_Boundry += wave;
	float coord = max(max(abs(0.25 - 0.25*(((i.uv.x) * 128) % 2)) , abs(0.25 - 0.25*((i.uv.y * 128) % 2))), step(_Boundry, 0.25*(((i.uv.x) * 128 + _Boundry * 2) % 2)) || step(_Boundry, 0.25*((i.uv.y * 128 + _Boundry * 2) % 2)));
	c = lerp(_Color + wave, _Color1 + wave, coord *_Color1.a);

	return c;
	}
		ENDCG
	}

	}
}