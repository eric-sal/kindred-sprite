// Custom sprite shader - no lighting, on/off alpha
// Sources:
// - http://docs.unity3d.com/Documentation/Manual/Shaders.html
// - http://www.third-helix.com/2012/02/making-2d-games-with-unity/
Shader "Sprite" {
	Properties {
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	}
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}

		ZWrite Off
	    Blend SrcAlpha OneMinusSrcAlpha
	    Lighting Off
	    Cull Off	// so we can have one frame for both left/right states

	    Pass {
	        SetTexture [_MainTex] { combine texture }
	    }
	}
}
