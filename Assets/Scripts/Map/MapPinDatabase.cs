using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// セッション中に SNS 経由で表示されたピンの状態を保持するシングルトン。
/// マップウィンドウが再び開いたとき、ピンを自動で復元する。
/// </summary>
public class MapPinDatabase : MonoBehaviour
{
    public static MapPinDatabase Instance { get; private set; }

    [Header("ピンのマスターデータ")]
    [SerializeField] private MapPinData pinData;

    /// <summary>セッション中に公開されたピン ID のセット</summary>
    private readonly HashSet<string> revealedPins = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // ─────────────────────────────────────────────
    // 公開 API
    // ─────────────────────────────────────────────

    /// <summary>
    /// ピンを公開する。マップウィンドウが開いていれば即座に表示、
    /// 閉じていれば次回開いたときに復元される。
    /// </summary>
    public void RevealPin(string pinId)
    {
        if (string.IsNullOrEmpty(pinId)) return;

        revealedPins.Add(pinId);
        Debug.Log($"[MapPinDatabase] ピン公開: {pinId}");

        // すでにマーカーマネージャが存在する場合は即座に表示
        if (MapWindowMarkerManager.ActiveInstance != null)
        {
            ApplyPin(pinId, MapWindowMarkerManager.ActiveInstance);
        }
    }

    /// <summary>
    /// マップウィンドウが開いたときに呼ばれ、公開済みの全ピンを復元する。
    /// </summary>
    public void RestoreRevealedPins(MapWindowMarkerManager markerManager)
    {
        if (markerManager == null) return;

        foreach (string pinId in revealedPins)
        {
            ApplyPin(pinId, markerManager);
        }
    }

    /// <summary>
    /// pinId の PinEntry を取得する。見つからなければ null。
    /// </summary>
    public MapPinData.PinEntry GetPin(string pinId)
    {
        if (pinData == null)
        {
            Debug.LogWarning("[MapPinDatabase] pinData が設定されていません。ManagerMap の Inspector で MapPinData を登録してください。");
            return null;
        }

        return pinData.FindPin(pinId);
    }

    // ─────────────────────────────────────────────
    // 内部処理
    // ─────────────────────────────────────────────

    private void ApplyPin(string pinId, MapWindowMarkerManager markerManager)
    {
        var entry = GetPin(pinId);
        if (entry == null) return;

        markerManager.AddOrUpdateMarker(pinId, entry.normalizedPosition, entry.arrowRotationZ, entry.showArrow);
    }
}
