using UnityEngine;
using System;
using System.Collections.Generic;


public class TwitterWindowUI : MonoBehaviour
{
    [Serializable]
    public class PostData
    {
        [Header("“ŠeÒ–¼")]
        public string userName = "User";
        [Header("“Še“à—e")]
        public string postText = "‚±‚ê‚Í“Še–{•¶‚Å‚·B";
        [Header("“Še‰æ‘œ")]
        public Sprite postImage;
        [Header("ƒAƒoƒ^[‚ÌF")]
        public Color avatarColor = new Color(0.1f, 0.45f, 1f);
    }

    [Header("“Še—pcontent")]
    [SerializeField] private Transform contentRoot;
    [Header("“Še‚PŒ•ª‚ÌPrefab")]
    [SerializeField] private TwitterPostItem postPrefab;
    [Header("•\¦‚·‚é“Šeˆê——")]
    [SerializeField] private List<PostData> posts = new List<PostData>();


    private void Start()
    {
        RefreshFeed();
    }

    public void RefreshFeed()
    {
        if (contentRoot == null || postPrefab == null)
        {
            Debug.LogWarning("TwitterWindowUI‚Ìİ’è‚ª•s‘«‚µ‚Ä‚¢‚Ü‚·");
            return;
        }

        for (int i = contentRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(contentRoot.GetChild(i).gameObject);
        }

        // Inspectorã‚ÌPosts‚ÅÅŒã‚É’Ç‰Á‚µ‚½‚à‚Ì‚ğˆê”Ôã‚É•\¦‚·‚é
        for (int i = posts.Count - 1; i >= 0; i--)
        {
            PostData post = posts[i];

            TwitterPostItem item = Instantiate(postPrefab, contentRoot);
            item.SetUp(post.userName, post.postText, post.postImage, post.avatarColor);
        }

    }

    public void AddPost(String userName, string postText, Sprite postImage = null)
    {
        PostData newPost = new PostData
        {
            userName = userName,
            postText = postText,
            postImage = postImage,
            avatarColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value)
        };
        posts.Add(newPost);
        RefreshFeed();
    }

   
}
