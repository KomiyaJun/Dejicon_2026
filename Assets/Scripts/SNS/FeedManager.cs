using UnityEngine;

public class FeedManager : MonoBehaviour
{
    [Header("データ")]
    [SerializeField] private FeedData feedData;

    [Header("Prefabと配置先")]
    [SerializeField] private PostItemView postItemPrefab;
    [SerializeField] private Transform contentParent; // ScrollViewのContent

    void Start()
    {
        GenerateFeed();
    }

    void GenerateFeed()
    {
        // 既存の子を全削除（リセット対応）
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        if (feedData == null || feedData.posts == null) return;

        foreach (PostData post in feedData.posts)
        {
            // Prefabをインスタンス化してContentへ追加
            PostItemView item = Instantiate(postItemPrefab, contentParent);

            // ScriptableObjectのデータをUIにバインド
            item.Bind(post);
        }
    }
}