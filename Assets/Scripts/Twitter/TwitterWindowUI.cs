using UnityEngine;
using System;
using System.Collections.Generic;


public class TwitterWindowUI : MonoBehaviour
{
    [Serializable]
    public class PostData
    {
        [Header("投稿者名")]
        public string userName = "User";
        [Header("投稿内容")]
        [TextArea(3, 10)]
        public string postText = "これは投稿本文です。";
        [Header("投稿画像")]
        public Sprite postImage;
        [Header("アバターアイコン")]
        public Sprite avatarIcon;
        [Header("アバターの色")]
        public Color avatarColor = new Color(0.1f, 0.45f, 1f);
        [Header("いいね数")]
        public int likeCount;
        [Header("投稿日時")]
        public string postTime = "7時間";
    }

    [Header("投稿用content")]
    [SerializeField] private Transform contentRoot;
    [Header("投稿１件分のPrefab")]
    [SerializeField] private TwitterPostItem postPrefab;
    [Header("表示する投稿一覧")]
    [SerializeField] private List<PostData> posts = new List<PostData>();


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

        // Inspector上のPostsで最後に追加したものを一番上に表示する
        for (int i = posts.Count - 1; i >= 0; i--)
        {
            PostData post = posts[i];

            TwitterPostItem item = Instantiate(postPrefab, contentRoot);
            item.SetUp(post.userName, post.postText, post.postImage, post.avatarIcon, post.avatarColor, post.likeCount, post.postTime);
        }

    }

    public void AddPost(String userName, string postText, Sprite postImage = null, Sprite avatarIcon = null)
    {
        PostData newPost = new PostData
        {
            userName = userName,
            postText = postText,
            postImage = postImage,
            avatarIcon = avatarIcon,
            avatarColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value)
        };
        posts.Add(newPost);
        RefreshFeed();
    }

   
}
