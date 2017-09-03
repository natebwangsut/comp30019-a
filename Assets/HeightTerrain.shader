Shader "Custom/HeightTerrain"
{
    Properties
    {
        _Color("Color",Color) = (1.0,1.0,1.0,1.0)
        _GroundHeight ("GroundHeight", Float) = -1
        _DirtBlendHeight ("DirtBlendHeight", Float) = -0.5
        _DirtTex("DirtTex", 2D) = "white"{}
        // why not inout ______ inout  family?
        _BaseTex("BaseTex", 2D) = "white"{}
         }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
       
        CGPROGRAM
        #pragma surface surf Lambert alpha
       
        float _GroundHeight;
        float _DirtBlendHeight;
        float4 _Color;
        sampler2D _DirtTex;
 
        struct Input
        {
            float3 customColor;
            float3 worldPos;
            float2 uv_DirtTex;
            float4 color : COLOR;
        };
 
        void surf (Input IN, inout SurfaceOutput o)
        {    
            half4 c = tex2D (_DirtTex, IN.uv_DirtTex);
            o.Albedo = IN.color;
 
            o.Alpha = 0;
            if (IN.worldPos.y > _GroundHeight && IN.worldPos.y < _DirtBlendHeight)
            {
                o.Alpha = 1 - ( ( ( _DirtBlendHeight - _GroundHeight ) - IN.worldPos.y ) / _DirtBlendHeight );
                
                
            } else if(IN.worldPos.y < _GroundHeight){
                o.Alpha = 1;
              
            }
            else if (IN.worldPos.y > _GroundHeight)
            {
                o.Alpha = c.a;
                
            }
            o.Albedo *= _Color.rgb;    
        }
        ENDCG
    }
    FallBack "Diffuse"
}