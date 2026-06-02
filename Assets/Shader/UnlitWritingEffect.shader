Shader "Custom/UnlitWritingEffect"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _MaskTex ("Writing Mask (R)", 2D) = "white" {}
        _NoiseTex ("Pencil Noise (R)", 2D) = "white" {}
        _Progress ("Progress", Range(0, 1)) = 0.0
        _NoiseStrength ("Noise Strength", Range(0, 0.2)) = 0.05
        
        // UI（Canvas）に対応するための標準プロパティ
        _Color ("Tint", Color) = (1,1,1,1)
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        // UI用のステンシル設定
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [ZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 uv       : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 uv       : TEXCOORD0;
            };

            sampler2D _MainTex;
            sampler2D _MaskTex;
            sampler2D _NoiseTex;
            fixed4 _Color;
            float _Progress;
            float _NoiseStrength;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 元の文字画像の色をサンプリング
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;

                // 書き順マスク（Rチャンネル）とノイズ（Rチャンネル）を取得
                fixed mask = tex2D(_MaskTex, i.uv).r;
                fixed noise = tex2D(_NoiseTex, i.uv).r;

                // マスク値にノイズを少しブレンドして、境界線をギザギザにする
                // (noise - 0.5) でノイズを -0.5 ～ 0.5 の範囲にして足し引きする
                fixed threshold = mask + (noise - 0.5) * _NoiseStrength;

                // _Progress（0～1）が threshold 未満の部分はまだ書かれていないので消す
                // clip関数を使い、値がマイナスになったピクセルの描画をスキップする
                clip(_Progress - threshold);

                return col;
            }
            ENDCG
        }
    }
}