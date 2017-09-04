// Original Cg/HLSL code stub copyright (c) 2010-2012 SharpDX - Alexandre Mutel
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// Adapted for COMP30019 by Jeremy Nicholson, 10 Sep 2012
// Adapted further by Chris Ewin, 23 Sep 2013
// Adapted further (again) by Alex Zable (port to Unity), 19 Aug 2016
// Adapted further (again x2) by Nate Bhurinat Wangsutthitham & Khai Mei Chin (for assignments), 1 Sep 2017
// This shader version implements the Phong Illumination for the terrain landscape

Shader "Unlit/PhongTest" {
   Properties {
      _SpecColor ("Specular Material Color", Color) = (1,1,1,1) 
      _Shininess ("Shininess", Float) = 10
   }
   SubShader {
      Pass {	
         Tags { "LightMode" = "ForwardBase" } 
            // pass for ambient light and first light source
 
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         #include "UnityCG.cginc"
         uniform float4 _LightColor0; 
         // color of light source (from "Lighting.cginc")
 
         // User-specified properties
         uniform float4 _SpecColor; 
         uniform float _Shininess;
 
         struct vertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float4 color : COLOR;
            
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 posWorld : TEXCOORD0;
            float3 normalDir : TEXCOORD1;
            float4 color : COLOR;
         };

         // Implementation of the vertex shader
         vertexOutput vert(vertexInput v) 
         {
            vertexOutput o;

            // Convert Vertex position and corresponding normal into world coords.
            // Note that we have to multiply the normal by the transposed inverse of the world 
            // transformation matrix (for cases where we have non-uniform scaling; we also don't
            // care about the "fourth" dimension, because translations don't affect the normal) 
            float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
            float3 worldNormal = normalize(mul(transpose((float3x3)unity_WorldToObject), v.normal.xyz));
    
            // Transform vertex in world coordinates to camera coordinates, and pass colour
            o.pos = UnityObjectToClipPos(v.vertex);
            o.color = v.color;
    
            // Pass out the world vertex position and world normal to be interpolated
            // in the fragment shader (and utilised)
            o.posWorld = posWorld;
            o.normalDir = worldNormal;
    
            return o;
         }
 
 
        // Implementation of the fragment shader
        float4 frag(vertexOutput input) : COLOR
         {
          
          
            // Our interpolated normal might not be of length 1
            float3 interpNormal = normalize(input.normalDir);
 
            
            float3 viewDirection = normalize(_WorldSpaceCameraPos - input.posWorld.xyz);
            float3 lightDirection;
            float fAtt;
 
            if (0.0 == _WorldSpaceLightPos0.w) // directional light?
            {
               fAtt = 1.0; // no attenuation
               lightDirection = normalize(_WorldSpaceLightPos0.xyz);
            } 
            else // point or spot light
            {
               float3 vertexToLightSource = 
                  _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
               float distance = length(vertexToLightSource);
               fAtt = 1.0 / distance; // linear attenuation 
               lightDirection = normalize(vertexToLightSource);
            }
 
 
            // Calculate ambient RGB intensities
            float Ka = 1;
            float3 amb = input.color.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb * Ka;
 
        
            // Calculate diffuse RBG reflections, we save the results of L.N because we will use it again
            // (when calculating the reflected ray in our specular component)
            float Kd = 1;
            float LdotN = dot(lightDirection, interpNormal);
            float3 dif = fAtt * _LightColor0.rgb * Kd * input.color.rgb * saturate(LdotN);
 
            float3 diffuseReflection = 
               fAtt * _LightColor0.rgb * input.color.rgb
               * max(0.0, dot(interpNormal, lightDirection));
 
            // Calculate specular reflections
            float Ks = 0.1;
            float specN = 10; // Values>>1 give tighter highlights
            float3 V = normalize(_WorldSpaceCameraPos - input.posWorld.xyz);
            // Using classic reflection calculation:
            //float3 R = normalize((2.0 * LdotN * interpNormal) - lightDirection);
            //float3 spe = fAtt * _LightColor0.rgb * Ks * pow(saturate(dot(V, R)), specN);
            // Using Blinn-Phong approximation:
            specN = 10; // We usually need a higher specular power when using Blinn-Phong
            float3 H = normalize(V + lightDirection);
            float3 spe = fAtt * _LightColor0.rgb * Ks * pow(saturate(dot(interpNormal, H)), specN);
 
 
 
            float3 specularReflection = 0;
            

            return float4(amb + dif + spe, 1.0);
         }
 
         ENDCG
      }
 
      
 
   }
   Fallback "Specular"
}