using UnityEngine;

/// <summary>
/// マップ上のピン情報をまとめた ScriptableObject。
/// pinId をキーに座標・向き・ラベルを一元管理する。
/// </summary>
[CreateAssetMenu(fileName = "MapPinData", menuName = "Map/MapPinData")]
public class MapPinData : ScriptableObject
{
    [Header("エディタープレビュー用マップ画像")]
    public Texture2D previewMapTexture;

    [System.Serializable]
    public class PinEntry
    {
        [Header("ピンを識別する ID")]
        public string pinId;

        [Header("マップ上の位置 (0,0)=左下 (1,1)=右上")]
        public Vector2 normalizedPosition = new Vector2(0.5f, 0.5f);

        [Header("矢印の表示/非表示")]
        public bool showArrow = true;

        [Header("矢印の向き (度)")]
        public float arrowRotationZ = 0f;

        [Header("表示ラベル (任意)")]
        public string label;
    }

    [Header("ピン一覧")]
    public PinEntry[] pins;

    /// <summary>
    /// pinId に一致する PinEntry を返す。見つからない場合は null。
    /// </summary>
    public PinEntry FindPin(string pinId)
    {
        if (pins == null || string.IsNullOrEmpty(pinId)) return null;

        foreach (var entry in pins)
        {
            if (entry.pinId == pinId)
                return entry;
        }

        Debug.LogWarning($"[MapPinData] pinId \"{pinId}\" が見つかりません。MapPinData に登録されているか確認してください。");
        return null;
    }
}
