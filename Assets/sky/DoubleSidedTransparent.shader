Shader "Custom/DoubleSidedTransparent"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,0.3)
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _DissolveAmount ("Dissolve Amount", Range(0,1)) = 0.0
        _EdgeColor ("Edge Color", Color) = (1,0.5,0,1)
        _EdgeWidth ("Edge Width", Range(0,1)) = 0.1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        Cull Off       // Render both sides
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf Standard alpha:fade

        sampler2D _MainTex;
        sampler2D _NoiseTex;
        fixed4 _Color;
        float _DissolveAmount;
        fixed4 _EdgeColor;
        float _EdgeWidth;

        struct Input {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float noise = tex2D(_NoiseTex, IN.uv_MainTex).r;

            if (noise < _DissolveAmount)
                clip(-1);  // descarta o pixel

            float edge = smoothstep(_DissolveAmount, _DissolveAmount + _EdgeWidth, noise);
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            // mistura com a cor de borda
            c.rgb = lerp(_EdgeColor.rgb, c.rgb, edge);

            o.Albedo = c.rgb;
            o.Alpha = c.a * edge;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
