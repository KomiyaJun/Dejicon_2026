using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PhotoMapPinButton : MonoBehaviour
{
    [Header("この写真ボタン")]
    [SerializeField] private Button photoButton;

    [Header("この写真に対応するID")]
    [SerializeField] private string markerId = "Photo_01";

    [Header("マップ上の位置 左下(0,0) 右上(1,1)")]
    [SerializeField] private Vector2 normalizedPosition = new Vector2(0.5f, 0.5f);

    [Header("同じ写真をもう一度押したら消す")]
    [SerializeField] private bool removeOnSecondClick = false;

    // 既存のMapWindowMarkerManagerに渡すためだけの固定値
    private const float DummyArrowRotationZ = 0f;

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

        // 既存のMapWindowMarkerManagerでピン＋矢印を生成
        markerManager.AddOrUpdateMarker(markerId, normalizedPosition, DummyArrowRotationZ);

        // 生成された矢印だけ非表示にする
        HideArrow(markerManager);
    }

    private void HideArrow(MapWindowMarkerManager markerManager)
    {
        string markerObjectName = "MarkerPair_" + markerId;

        RectTransform[] rectTransforms = markerManager.GetComponentsInChildren<RectTransform>(true);

        foreach (RectTransform rect in rectTransforms)
        {
            if (rect.name != markerObjectName) continue;

            Transform arrow = rect.Find("Arrow");

            if (arrow != null)
            {
                arrow.gameObject.SetActive(false);
            }

            return;
        }

        Debug.LogWarning(markerObjectName + " が見つかりませんでした。");
    }
}