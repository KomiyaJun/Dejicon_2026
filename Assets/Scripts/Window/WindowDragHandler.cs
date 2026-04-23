using UnityEngine;
using UnityEngine.EventSystems;

//スクリプトをアタッチしたオブジェクトをドラッグした際に、targetRectを移動させるクラス
//クリックしたスクリプトに設定したtargetRectは、SetAsKastSibling()の処理を通し手前に来ます。
//担当者 : 小宮

public class WindowDragHandler : MonoBehaviour,  IDragHandler, IPointerDownHandler
{
    [SerializeField] private RectTransform targetRect;

    private void Awake()
    {
        // もし未設定なら、自分自身の親などを自動で割り当てる（保険）
        if (targetRect == null)
        {
            targetRect = GetComponentInParent<WindowBase>()?.GetComponent<RectTransform>();
        }
    }

    // クリックした瞬間に呼ばれる
    public void OnPointerDown(PointerEventData eventData)
    {
        // ウィンドウを最前面に持ってくる
        if (targetRect != null)
        {
            targetRect.SetAsLastSibling();
        }
    }

    // ドラッグ中に呼ばれる
    public void OnDrag(PointerEventData eventData)
    {
        if (targetRect == null) return;

        // eventData.delta は前フレームからのマウス移動量
        // これをanchoredPositionに加算することで、マウスについてくるようになる
        targetRect.anchoredPosition += eventData.delta;
    }
}
