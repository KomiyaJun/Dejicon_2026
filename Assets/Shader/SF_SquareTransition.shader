Shader "Custom/SF_IndependentSquareTransition_Fixed"
{
    Properties
    {
        _MainTex("Base Texture", 2D) = "white" {}
        _Progress("Progress", Range(0, 1)) = 0
        _GridSize("Grid Size", Vector) = (30, 20, 0, 0)
        _RandomIntensity("Randomness", Range(0, 1)) = 0.8
        _Gap("Chip Gap", Range(0, 0.5)) = 0.05
        _GlowColor("Glow Color", Color) = (0, 0.5, 1, 1)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _GridSize;
            float _Progress;
            float _RandomIntensity;
            float _Gap;
            fixed4 _GlowColor;

            float random(float2 uv) {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453123);
            }

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 1. グリッドの計算
                float2 gv = i.uv * _GridSize.xy;
                float2 id = floor(gv);
                float2 fuv = frac(gv);

                // 2. 個別の動き
                float rand = random(id);
                float individualProgress = saturate(_Progress * (1.0 + _RandomIntensity) - (rand * _RandomIntensity));
                float yOffset = pow(individualProgress, 2.0) * 1.5;

                // 3. サンプリング位置の固定（チップが絵として独立する）
                float2 sampledUV = (id + 0.5) / _GridSize.xy; // 各セルの中心値をサンプル
                // 注意: 厳密にはテクスチャが歪まないよう工夫が必要ですが、
                // トランジションとしては「セルの中心の色」を拾うのが最もドット絵・チップ感が出ます。
                
                // もし元の絵を維持したまま動かしたい場合はこちら:
                float2 originalUV = i.uv;
                originalUV.y -= yOffset;

                // 4. マスク処理 (0 = 隙間, 1 = チップ)
                float2 boundary = step(_Gap, fuv) * step(fuv, 1.0 - _Gap);
                float mask = boundary.x * boundary.y;

                // 5. 色の取得
                fixed4 col = tex2D(_MainTex, originalUV);

                // 6. アルファとエフェクト
                // 画面外判定
                float alphaLimit = step(0, originalUV.y) * step(originalUV.y, 1.0);
                
                // チップ自体の透明度
                col.a *= mask * alphaLimit * (1.0 - individualProgress);

                // 7. 発光（縁の演出）
                float edge = (1.0 - mask) * step(0.01, _Gap); // Gapが0の時は出さない
                fixed4 glow = _GlowColor * edge * col.a * 2.0;

                fixed4 finalCol = col + glow;
                return finalCol;
            }
            ENDCG
        }
    }
}