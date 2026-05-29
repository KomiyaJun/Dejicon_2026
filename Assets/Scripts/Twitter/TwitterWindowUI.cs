using UnityEngine;
using System;
using System.Collections.Generic;


public class TwitterWindowUI : MonoBehaviour
{
    [Header("データ")]
    [SerializeField] private FeedData feedData;

    [Header("投稿用content")]
    [SerializeField] private Transform contentRoot;
    
    [Header("投稿UIのPrefab")]
    [SerializeField] private TwitterPostItem postPrefab;


    private void Start()
    {
        RefreshFeed();
    }

    public void RefreshFeed()
    {
        if (contentRoot == null || postPrefab == null)
        {
            Debug.LogWarning("TwitterWindowUIの設定が不足しています");
            return;
        }

        for (int i = contentRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(contentRoot.GetChild(i).gameObject);
        }

        if (feedData == null || feedData.posts == null) return;

        // Twitterは新しい投稿が上にくるようにしたい場合、そのままループするか逆順ループするか
        // Instagram側はそのままループしていましたが、元のTwitterのコードに合わせて逆順に表示
        for (int i = feedData.posts.Length - 1; i >= 0; i--)
        {
            global::PostData post = feedData.posts[i];
            if (post == null) continue;

            TwitterPostItem item = Instantiate(postPrefab, contentRoot);
            item.Bind(post);
        }
    }
}
