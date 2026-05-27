using UnityEngine;

/// <summary>
/// プロローグのデータを保持する ScriptableObject。
/// 背景画像は1枚のみ。セリフは行ごとに配列で管理する。
/// Project上で右クリック → Create → Prologue → SlideData から作成できる。
/// </summary>
[CreateAssetMenu(fileName = "SlideData", menuName = "Prologue/SlideData")]
public class PrologueSlideData : ScriptableObject
{
    [Header("背景画像（プロローグ全体で1枚）")]
    [Tooltip("プロローグ開始時に表示する背景画像")]
    public Sprite backgroundImage;

    [Header("セリフ一覧")]
    [Tooltip("1要素 = 1クリックで表示される1行分のセリフ")]
    public string[] lines;

    [Header("テキスト設定")]
    [Tooltip("文字送り速度（文字/秒）。0以下なら即時全表示。")]
    public float textSpeed = 30f;

    [Header("BGM（任意）")]
    [Tooltip("プロローグで再生するBGM")]
    public AudioClip bgm;
}