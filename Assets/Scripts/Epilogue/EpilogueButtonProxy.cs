using UnityEngine;

/// <summary>
/// プレハブ内のボタンからシーン上の EpilogueTransitionController を呼び出すためのスクリプト
/// </summary>
public class EpilogueButtonProxy : MonoBehaviour
{
    // ボタンの OnClick イベントからこれを呼び出します
    public void CallTransition()
    {
        if (EpilogueTransitionController.Instance != null)
        {
            EpilogueTransitionController.Instance.StartTransition();
        }
        else
        {
            Debug.LogWarning("シーン内に EpilogueTransitionController が見つかりません。Setup Pre-Epilogue UI が実行されているか確認してください。");
        }
    }
}
