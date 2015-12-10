Shader "CardboardControls/Pointer" {
    Properties {
        _MainTex ("Base", 2D) = "white" {}
        _Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
    }

    SubShader {
      Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
      CGPROGRAM
      #pragma surface surf Lambert alpha
      struct Input {
          float2 uv_MainTex;
      };
      sampler2D _MainTex;
      fixed4 _Color;
      void surf (Input IN, inout SurfaceOutput o) {
          o.Albedo = _Color.rgb;
          o.Alpha = tex2D (_MainTex, IN.uv_MainTex).r * _Color.a;
      }
      ENDCG
    }
    Fallback "Diffuse"
  }
