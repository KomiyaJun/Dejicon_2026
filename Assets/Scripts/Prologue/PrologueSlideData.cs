using UnityEngine;

/// <summary>
/// 1枚のスライドデータを保持する ScriptableObject。
/// Project上で右クリック → Create → Prologue → SlideData から作成できる。
/// </summary>
[CreateAssetMenu(fileName = "SlideData", menuName = "Prologue/SlideData")]
public class PrologueSlideData : ScriptableObject
{
    [Header("表示内容")]
    [Tooltip("スライドに表示する背景画像")]
    public Sprite image;

    [Tooltip("スライドに表示するテキスト（改行は\\nで記述）")]
    [TextArea(3, 6)]
    public string text;

    [Header("タイミング")]
    [Tooltip("テキスト送り速度（文字/秒）。0以下なら即時全表示。")]
    public float textSpeed = 30f;

    [Tooltip("テキスト全表示後、次へ自動進む場合の待機秒数。0以下なら自動進行しない。")]
    public float autoAdvanceDelay = 0f;

    [Header("BGM（任意）")]
    [Tooltip("このスライドで再生するBGM。nullなら前のスライドのBGMを継続。")]
    public AudioClip bgm;
}
