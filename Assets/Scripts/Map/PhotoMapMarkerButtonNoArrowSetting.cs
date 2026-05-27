using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PhotoMapMarkerButtonNoArrowSetting : MonoBehaviour
{
    [Header("この写真ボタン")]
    [SerializeField] private Button photoButton;

    [Header("この写真に対応するID")]
    [SerializeField] private string markerId = "Photo_01";

    [Header("マップ上の位置 左下(0,0) 右上(1,1)")]
    [SerializeField] private Vector2 normalizedPosition = new Vector2(0.5f, 0.5f);

    [Header("同じ写真をもう一度押したら消す")]
    [SerializeField] private bool removeOnSecondClick = false;

    // 既存のMapWindowMarkerManagerに渡すための固定値
    // 矢印画像が右向きの場合、0度は右向き
    private const float FixedArrowRotationZ = 0f;

    private void Reset()
    {
        photoButton = GetComponent<Button>();
    }

    private void Awake()
    {
        if (photoButton == null)
        {
            photoButton = GetComponent<Button>();
        }
    }

    private void OnEnable()
    {
        photoButton.onClick.AddListener(OnClickPhoto);
    }

    private void OnDisable()
    {
        photoButton.onClick.RemoveListener(OnClickPhoto);
    }

    private void OnClickPhoto()
    {
        MapWindowMarkerManager markerManager = MapWindowMarkerManager.ActiveInstance;

        if (markerManager == null)
        {
            markerManager = FindFirstObjectByType<MapWindowMarkerManager>();
        }

        if (markerManager == null)
        {
            Debug.LogWarning("Scene上に MapWindowMarkerManager が見つかりません。先にMapWindowを開いてください。");
            return;
        }

        if (removeOnSecondClick && markerManager.HasMarker(markerId))
        {
            markerManager.RemoveMarker(markerId);
            return;
        }

        markerManager.AddOrUpdateMarker(markerId, normalizedPosition, FixedArrowRotationZ);
    }
}