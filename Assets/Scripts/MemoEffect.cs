using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

public class MemoEffect : MonoBehaviour
{
    [Header("テキスト演出用")]
    [SerializeField] private Image slashImage;      // ギザギザ斜線のImage
    [SerializeField] private List<CanvasGroup> textGroups = new List<CanvasGroup>();

    [Header("写真（フラッシュ）演出用")]
    [SerializeField] private Image faceImage;       // 顔写真のImage
    [SerializeField] private Image flashImage;      // フラッシュ用のImage（Maskの子にする）

    /// <summary>
    /// すでに解放済みの状態でウィンドウが開かれた時の初期化
    /// </summary>
    public void SetUnlockedState()
    {
        if (slashImage != null) slashImage.gameObject.SetActive(false);
        foreach (var group in textGroups) if (group != null) group.alpha = 1f;

        if (faceImage != null) faceImage.gameObject.SetActive(true);
        if (flashImage != null) flashImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// 出現アニメーション全体を再生
    /// </summary>
    public async UniTaskVoid PlayAnimationAsync(CancellationToken ct)
    {
        // --- 1. 初期状態のセットアップ ---
        gameObject.SetActive(true);

        if (slashImage != null)
        {
            slashImage.gameObject.SetActive(true);
            slashImage.fillAmount = 0f;
            slashImage.color = Color.white;
        }
        foreach (var group in textGroups) if (group != null) group.alpha = 0f;

        // 写真演出の初期化：最初は写真を隠し、フラッシュ画像を「完全な白」で重ねる
        if (faceImage != null) faceImage.gameObject.SetActive(false);
        if (flashImage != null)
        {
            flashImage.gameObject.SetActive(true);
            flashImage.color = Color.white; // 始まりは白
        }

        // --- 2. 斜線をギザギザに書く（0.3秒） ---
        if (slashImage != null)
        {
            await slashImage.DOFillAmount(1f, 0.3f)
                .SetEase(Ease.OutQuad)
                .WithCancellation(ct);
        }

        // --- 3. 【改良】シャッターの色遷移（白 → グレー → 黒） ---
        if (flashImage != null)
        {
            // ① 白からグレーへ（0.1秒かけて変化）
            await flashImage.DOColor(new Color(0.4f, 0.4f, 0.4f, 1f), 0.3f)
                .SetEase(Ease.Linear)
                .WithCancellation(ct);

            // ② グレーから黒へ（0.05秒で一気に暗転）
            await flashImage.DOColor(Color.black, 0.15f)
                .SetEase(Ease.InQuad)
                .WithCancellation(ct);

            // 黒になった瞬間に、背後の「顔写真」をアクティブにする
            if (faceImage != null) faceImage.gameObject.SetActive(true);
        }

        // --- 4. 斜線消去 ＆ フラッシュ（黒）消去 ＆ 文章フェードイン（同時並行） ---
        var fadeTasks = new List<UniTask>();

        // 斜線のフェードアウト
        if (slashImage != null)
        {
            fadeTasks.Add(slashImage.DOFade(0f, 0.4f).SetEase(Ease.InQuad).WithCancellation(ct));
        }

        // フラッシュ画像（黒）のフェードアウト ＝ 写真が浮かび上がる
        if (flashImage != null)
        {
            fadeTasks.Add(flashImage.DOFade(0f, 0.5f).SetEase(Ease.OutQuad).WithCancellation(ct));
        }

        // すべてのテキストグループのフェードイン
        foreach (var group in textGroups)
        {
            if (group != null)
            {
                fadeTasks.Add(group.DOFade(1f, 0.5f).SetEase(Ease.OutCubic).WithCancellation(ct));
            }
        }

        // すべてのフェード完了を待つ
        await UniTask.WhenAll(fadeTasks);

        // --- 5. 後片付け ---
        if (slashImage != null) slashImage.gameObject.SetActive(false);
        if (flashImage != null) flashImage.gameObject.SetActive(false);
    }
}