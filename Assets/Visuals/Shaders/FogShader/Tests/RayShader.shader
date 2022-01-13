Shader "Unlit/RayShader" 
{
    Properties
    {
        _MainTex ("Fog texture", 2D) = "white" {}
        _Color ("Color", color) = (1., 1., 1., 1.)
 
        _FogSpeedX ("Fog Horizontal Speed", Float) = 1
		_FogSpeedY ("Fog Vertical Speed", Float) = 1
        _Speed ("Speed", float) = 1.
    }
 
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
           
            #include "UnityCG.cginc"
 
            struct v2f {
                float4 pos : SV_POSITION;
                fixed4 vertCol : COLOR0;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
            };
 
            sampler2D _MainTex;
            float4 _MainTex_ST;
 
            v2f vert(appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv2 = v.texcoord;
                o.vertCol = v.color;
                return o;
            }
 
            float _Speed;
            float _FogSpeedX;
            float _FogSpeedY;
            float4 _Color;
 
            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv + fixed2(_FogSpeedX, _FogSpeedY) * _Speed * _Time.x;
                fixed4 col = tex2D(_MainTex, uv) * _Color * i.vertCol;
                col.a *= 1 - (i.pos.z / i.pos.w);
                return col;
            }
            ENDCG
        }
    }
}
