Shader "Unlit/FogShader"
{
    Properties
    {
        _Color("Main Color", Color) = (1, 1, 1, 1)
        _IntersectionThreshold("Intersection Threshold", float) = 1
		_NoiseTex ("Noise Texture", 2D) = "white" {}
		_FogSpeedX ("Fog Horizontal Speed", Float) = 0.1
		_FogSpeedY ("Fog Vertical Speed", Float) = 0.1
		_NoiseLevel ("Noise Level", Float) = 1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent"  }
  
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"
  
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
  
            struct v2f
            {
                float4 scrPos : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD2;
            };
  
            sampler2D _CameraDepthTexture;
            float4 _Color;
            float _IntersectionThreshold;
            sampler2D _NoiseTex;
            float _FogSpeedX;
            float _FogSpeedY;
            float _NoiseLevel;
  
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.scrPos = ComputeScreenPos(o.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;   
            }
  
            half4 frag(v2f i) : SV_TARGET
            {
                // Fade away the texture (vertically) depending on the view angle
                float depth = LinearEyeDepth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos)));
                float diff = saturate(_IntersectionThreshold * (depth - i.scrPos.w));
                fixed4 col = lerp(fixed4(_Color.rgb, 0.0), _Color, diff);
  
                UNITY_APPLY_FOG(i.fogCoord, col);

                // Animate the fag effect by shifting the noise texture
                float speed = _Time.y * float2(_FogSpeedX, _FogSpeedY);
                float noise = (tex2D(_NoiseTex, i.uv + speed).r - 0.5) * _NoiseLevel;   
                return col * (1 - noise);
            }
  
            ENDCG
        }
    }
}
