// FeedManager.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FeedManager : MonoBehaviour
{
    [Header("データ")]
    [SerializeField] private FeedData feedData;

    [Header("Prefabと配置先")]
    [SerializeField] private PostItemView postItemPrefab;
    [SerializeField] private Transform contentParent;

    private ScrollRect scrollRect;

    void Start()
    {
        // 自分自身のScrollRectを自動取得する
        if (scrollRect == null)
            scrollRect = GetComponent<ScrollRect>();

        GenerateFeed();
    }

    void GenerateFeed()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        if (feedData == null || feedData.posts == null) return;

        foreach (PostData post in feedData.posts)
        {
            PostItemView item = Instantiate(postItemPrefab, contentParent);
            item.Bind(post);
        }

        // 1フレーム待ってからスクロール位置をリセットする
        StartCoroutine(ResetScrollPosition());
    }

    // 1フレーム待ってからスクロール位置を一番上に戻すコルーチン
    private IEnumerator ResetScrollPosition()
    {
        // レイアウトの再計算が終わるまで待つ
        yield return null;

        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1f;
    }
}