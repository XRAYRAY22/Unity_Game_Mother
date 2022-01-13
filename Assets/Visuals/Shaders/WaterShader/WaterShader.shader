
Shader "Lit/WaterShader"
{
    //Main Reference https://roystan.net/articles/toon-water.html
    //Secondary reference http://tinkering.ee/unity/asset-unity-refractive-shader/ 
    Properties
    {
        _DepthGradientShallow("Depth Gradient - Shallow", Color) = (0.325,0.807,0.971,0.725)
        _DepthGradientDeep("Depth Gradient - Deep", Color) = (0.086,0.407,1,0.749)
        _Color("Color", Color) = (0.086,0.407,1,0.749)
        _DepthMaxDistance("DepthMaxDistance", Float) = 1
        _SurfaceNoise("Surface Noise", 2D) = "white" {}
        _SurfaceNoiseCutOff("Surface Noise Cutoff", Range(0,1)) = 0.777
        _FoamDistance("Foam Distance", Float) = 0.4
        _NoiseMovement("Scroll Noise", Vector) = (0.3,0.3,0,0)
        _NoiseDistortion("Distortion", 2D) = "white" {}
        _DistortionAmount("Distortion Amount", Float) = 0.2
        _WaveAmp ("Wave Amplitude", Range(0,1)) = 1
        _Refraction ("Water Transparency", Range(0,1)) = 0.5
        _RefractionSpeed("Refraction Speed", Range(0,5)) = 0.5
        _RefractionStrength("RefractionStrength", Range(0,10)) = 0.1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        //LOD 100

        Pass
        {
            //Blend SrcAlpha OneMinusSrcAlpha
            //Zwrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
           

            #include "UnityCG.cginc"
            #define SMOOTHSTEP_AA 0.01

            //max color shallow waters
            float4 _DepthGradientShallow;
            //max color Deep waters
            float4 _DepthGradientDeep;
            //max depth (for scaling large depth)
            float _DepthMaxDistance;
            //texture of camera depth retrieved from camera object
            sampler2D _CameraDepthTexture;
            //noise for waves
            sampler2D _SurfaceNoise;
            float4 _SurfaceNoise_ST;
            //make noise less varied
            float _SurfaceNoiseCutOff;
            //how far foam spreads from shore or other intersections
            float _FoamDistance;
            float2 _NoiseMovement;
            //distortion of wave effect
            sampler2D _NoiseDistortion;
            float4 _NoiseDistortion_ST;
            //amount of distortion
            float _DistortionAmount;
            //color of foam
            float4 _FoamColor;
            //wave amplitude
            float _WaveAmp;
            //Opaque layer render
            sampler2D _CameraOpaqueTexture;
            float _Refraction;
            float _RefractionSpeed;
            float _RefractionStrength;

            struct vertIn
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
            };

            struct vertOut
            {
                float4 screenPosition1 : TEXCOORD1;
                float4 screenPosition2 : TEXCOORD4;
                float2 noiseUV : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 distortionUV : TEXCOORD2;
                float2 checkUV : TEXCOORD3;
            };

            float4 alphaBlend(float4 top, float4 bottom){
                float3 color = (top.rgb*top.a) + (bottom.rgb * (1-top.a));
                float alpha = top.a + bottom.a * (1-top.a);
                return float4(color,alpha);
            }


            vertOut vert (vertIn i)
            {
                vertOut o;
                //displace y value of vertices for tide effect
                float wave = cos(i.uv.y - _Time.y*2)*5;
                i.vertex.y = wave*_WaveAmp;
                
                //vertex to clip space
                o.vertex = UnityObjectToClipPos(i.vertex);

                //texture coordinates forcamera depth texture mapping
                o.screenPosition1 = ComputeScreenPos(o.vertex);

                //displace and calculate screen texture coordinates for camera opaque texture mapping
                float4 refractionVertex = o.vertex;
                float refractionWave1 = sin(i.vertex- _Time.y*_RefractionSpeed)*5;//multiplied by 5 for effect to be visible
                float refractionWave2 = sin(i.vertex- _Time.y*_RefractionSpeed)*5;
                float refractionWave3 = sin(i.vertex - _Time.y*_RefractionSpeed)*5;
                refractionVertex.x = refractionVertex.x + refractionWave1*_RefractionStrength;
                refractionVertex.z = refractionVertex.z + refractionWave2*_RefractionStrength;
                refractionVertex.y = refractionVertex.y + refractionWave3*_RefractionStrength;
                o.screenPosition2 = ComputeGrabScreenPos(refractionVertex)/refractionVertex.w;

                //get texture mapping coordincates for noice and distortion maps
                o.noiseUV = TRANSFORM_TEX(i.uv, _SurfaceNoise);
                o.distortionUV = TRANSFORM_TEX(i.uv, _NoiseDistortion);
            
                return o;
            }

            float4 frag (vertOut i) : SV_Target
            {
                //DEPTH GRADIENT EFFECT
                //Sample the depth texture for depth gradient effect
                float4 existingDepth = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPosition1));
                float existingDepthLinear = LinearEyeDepth(existingDepth).r;
                //Get depth of objects underwater relative to surface level
                float depthDiff = existingDepthLinear - i.screenPosition1.w;
                //get relative depth as % of max depth (clamped) and use to calculate water color
                float waterDepthDifference = saturate(depthDiff/_DepthMaxDistance);
                float4 waterColor = lerp(_DepthGradientShallow, _DepthGradientDeep, waterDepthDifference);
                
                //TRANSPARENCY/UNDERWATER DISTORTION
                 //Sample the opaque texture for transparency/reflective effect
                float4 refracted = tex2Dproj(_CameraOpaqueTexture, i.screenPosition2);
                //Reduce water value alpha per _Refraction amount in inspector
                waterColor.a = 1 - _Refraction;
                //Blend water and underwater colors
                float4 riverColor = alphaBlend(waterColor, refracted);
                
                //FOAM AND SURFACE DISTORTION ANIMATION
                //Sample distortion map texture, transform to val between -1 and 1, and scale effect based on _DistortionAmount
                float2 distortionSample = _DistortionAmount * ((tex2D(_NoiseDistortion,i.distortionUV).xy)*2-1);
                //Animate perlin noise with respect to time and _NoiseMovement, adding distortion
                float2 noiseUV = float2((i.noiseUV.x + _Time.y * _NoiseMovement.x) + distortionSample.x, (i.noiseUV.y + _Time.y * _NoiseMovement.y)+ distortionSample.y);
                //Sample perlin noise texture
                float surfaceNoiseSample = tex2D(_SurfaceNoise, noiseUV).r;
                //Lower foam cutoff for shallower regions/partially submerged objects
                float foamDepthDiff = saturate(depthDiff / _FoamDistance);
                float surfaceNoiseCutOff = foamDepthDiff*_SurfaceNoiseCutOff;
                // Get surfaceNoiseColor using Antialiasing Macro
                float surfaceNoise = smoothstep(surfaceNoiseCutOff - SMOOTHSTEP_AA, surfaceNoiseCutOff + SMOOTHSTEP_AA, surfaceNoiseSample);      

                //FINAL COLOR
                return riverColor+surfaceNoise;
            }
            ENDCG
        }
    }
}
