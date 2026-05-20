using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapWindowMarkerManager : MonoBehaviour
{
    public static MapWindowMarkerManager ActiveInstance { get; private set; }

    [Header("マーカーを表示するレイヤー")]
    [SerializeField] private RectTransform markerLayer;

    [Header("ピンと矢印の画像")]
    [SerializeField] private Sprite pinSprite;
    [SerializeField] private Sprite arrowSprite;

    [Header("ピンと矢印の表示サイズ")]
    [SerializeField] private Vector2 pinSize = new Vector2(36f, 36f);
    [SerializeField] private Vector2 arrowSize = new Vector2(42f, 42f);

    [Header("矢印をピンから離す距離")]
    [SerializeField] private float arrowDistance = 32f;

    private readonly Dictionary<string, MarkerPair> markers = new Dictionary<string, MarkerPair>();

    private class MarkerPair
    {
        public RectTransform root;
        public RectTransform pin;
        public RectTransform arrow;
    }

    private void Awake()
    {
        if (!gameObject.scene.IsValid())
        {
            Debug.LogError("MapWindowMarkerManager が Project内のPrefab Asset上で実行されています。Scene上のMapWindowで実行してください。");
            return;
        }

        ActiveInstance = this;
    }

    private void OnEnable()
    {
        if (gameObject.scene.IsValid())
        {
            ActiveInstance = this;
        }
    }

    private void OnDestroy()
    {
        if (ActiveInstance == this)
        {
            ActiveInstance = null;
        }
    }

    public void AddOrUpdateMarker(string markerId, Vector2 normalizedPosition, float arrowRotationZ)
    {
        if (markerLayer == null)
        {
            Debug.LogError("MarkerLayer が設定されていません。MapWindow Prefab内の MarkerLayer を登録してください。");
            return;
        }

        if (!markerLayer.gameObject.scene.IsValid())
        {
            Debug.LogError("MarkerLayer に Project内のPrefab Asset が設定されています。Hierarchy上に生成された MapWindow の MarkerLayer を使ってください。");
            return;
        }

        if (string.IsNullOrEmpty(markerId))
        {
            Debug.LogWarning("Marker ID が空です。");
            return;
        }

        normalizedPosition.x = Mathf.Clamp01(normalizedPosition.x);
        normalizedPosition.y = Mathf.Clamp01(normalizedPosition.y);

        MarkerPair markerPair;

        if (markers.ContainsKey(markerId))
        {
            markerPair = markers[markerId];
        }
        else
        {
            markerPair = CreateMarkerPair(markerId);
            markers.Add(markerId, markerPair);
        }

        markerPair.root.anchorMin = normalizedPosition;
        markerPair.root.anchorMax = normalizedPosition;
        markerPair.root.anchoredPosition = Vector2.zero;

        markerPair.pin.anchoredPosition = Vector2.zero;

        Vector2 arrowDirection = Quaternion.Euler(0f, 0f, arrowRotationZ) * Vector2.right;

        markerPair.arrow.anchoredPosition = arrowDirection * arrowDistance;
        markerPair.arrow.localRotation = Quaternion.Euler(0f, 0f, arrowRotationZ);

        markerPair.root.gameObject.SetActive(true);
        markerPair.root.SetAsLastSibling();
    }

    public bool HasMarker(string markerId)
    {
        return markers.ContainsKey(markerId);
    }

    public void RemoveMarker(string markerId)
    {
        if (!markers.ContainsKey(markerId)) return;

        Destroy(markers[markerId].root.gameObject);
        markers.Remove(markerId);
    }

    public void ClearAllMarkers()
    {
        foreach (MarkerPair markerPair in markers.Values)
        {
            if (markerPair.root != null)
            {
                Destroy(markerPair.root.gameObject);
            }
        }

        markers.Clear();
    }

    private MarkerPair CreateMarkerPair(string markerId)
    {
        GameObject rootObject = new GameObject("MarkerPair_" + markerId, typeof(RectTransform));
        rootObject.transform.SetParent(markerLayer, false);

        RectTransform rootRect = rootObject.GetComponent<RectTransform>();
        rootRect.pivot = new Vector2(0.5f, 0.5f);
        rootRect.sizeDelta = Vector2.zero;
        rootRect.localScale = Vector3.one;

        RectTransform pinRect = CreateUIImage("Pin", pinSprite, pinSize, rootRect);
        RectTransform arrowRect = CreateUIImage("Arrow", arrowSprite, arrowSize, rootRect);

        return new MarkerPair
        {
            root = rootRect,
            pin = pinRect,
            arrow = arrowRect
        };
    }

    private RectTransform CreateUIImage(string objectName, Sprite sprite, Vector2 size, RectTransform parent)
    {
        GameObject imageObject = new GameObject(objectName, typeof(RectTransform), typeof(Image));
        imageObject.transform.SetParent(parent, false);

        RectTransform rect = imageObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = Vector2.zero;
        rect.localScale = Vector3.one;

        Image image = imageObject.GetComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = true;
        image.raycastTarget = false;

        return rect;
    }
}