﻿Shader "Custom/MaskShader" {
	Properties {
		_MainTex ("Main Texture", 2D) = "white" {}
		_Mask ("Mask Texture", 2D) = "white" {}
	}
	SubShader {
		Tags { "Queue"="Transparent" }
		Lighting Off
		Cull Off
		ZTest Always
		ZWrite Off
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass{
			SetTexture [_Mask] {combine texture}
			SetTexture [_MainTex] {combine texture, previous}
		}
	}
}
