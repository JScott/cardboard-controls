Shader "CardboardControls/Pointer" {
  Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB)", 2D) = "white" {}
  }
  Category {
    Blend SrcAlpha OneMinusSrcAlpha
    Lighting Off
    ZWrite Off
    ZTest Always
    Cull Back
    Fog { Mode Off }
    SubShader {
      Pass {
        SetTexture [_MainTex] {
          constantColor [_Color]
          Combine texture * constant, texture * constant 
        }
      }
    } 
  }
}
