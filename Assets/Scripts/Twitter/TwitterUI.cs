using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Xのような投稿一覧UIをUnity上に表示するためのサンプルスクリプトです。
/// 
/// 特徴:
/// ・1スクリプトで完結
/// ・ScrollRectを自動生成
/// ・投稿本文と画像を表示可能
/// ・Inspectorから投稿データを設定可能
/// ・既存CanvasがなければCanvasも自動生成
/// 
/// 使い方:
/// 1. 空のGameObjectを作成
/// 2. このスクリプトをアタッチ
/// 3. InspectorのPostsに投稿内容を追加
/// 4. 再生すると投稿一覧UIが表示されます
/// </summary>
public class TwitterUI : MonoBehaviour
{
    /// <summary>
    /// 1件分の投稿データです。
    /// Inspectorから編集できます。
    /// </summary>
    [Serializable]
    public class PostData
    {
        [Header("投稿者名")]
        public string userName = "User";

        [Header("投稿本文")]
        [TextArea(2, 6)]
        public string postText = "これは投稿本文です。";

        [Header("投稿画像")]
        public Sprite postImage;

        [Header("アバター色")]
        public Color avatarColor = new Color(0.1f, 0.45f, 1f);
    }

    [Header("表示する投稿一覧")]
    [SerializeField]
    private List<PostData> posts = new List<PostData>();

    [Header("起動時にサンプル投稿を生成する")]
    [SerializeField]
    private bool createSamplePostsOnStart = true;

    [Header("投稿カードの最大横幅")]
    [SerializeField]
    private float maxPostWidth = 680f;

    [Header("投稿同士の間隔")]
    [SerializeField]
    private float postSpacing = 16f;

    [Header("背景色")]
    [SerializeField]
    private Color backgroundColor = new Color(0.03f, 0.03f, 0.035f, 1f);

    [Header("投稿カード背景色")]
    [SerializeField]
    private Color postCardColor = new Color(0.12f, 0.12f, 0.14f, 1f);

    [Header("本文テキスト色")]
    [SerializeField]
    private Color textColor = Color.white;

    [Header("ユーザー名テキスト色")]
    [SerializeField]
    private Color userNameColor = new Color(0.9f, 0.9f, 0.95f, 1f);

    private Canvas canvas;
    private ScrollRect scrollRect;
    private RectTransform contentRoot;
    private Font defaultFont;

    private void Start()
    {
        // Unity標準フォントを取得します。
        // TextコンポーネントにはFontが必要です。
        defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        if (defaultFont == null)
        {
            defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        // Inspectorに投稿がない場合、必要に応じてサンプルを作成します。
        if (posts.Count == 0 && createSamplePostsOnStart)
        {
            CreateSamplePosts();
        }

        // UI全体を生成します。
        BuildUI();

        // 投稿一覧を生成します。
        RefreshFeed();
    }

    /// <summary>
    /// サンプル投稿を作成します。
    /// 実運用ではInspectorからpostsを設定するか、AddPost()を呼び出してください。
    /// </summary>
    private void CreateSamplePosts()
    {
        posts.Add(new PostData
        {
            userName = "DigiCon",
            postText = "Unity上でXのような投稿一覧UIを表示するサンプルです。スクロールで過去投稿を見ることができます。",
            avatarColor = new Color(0.1f, 0.45f, 1f)
        });

        posts.Add(new PostData
        {
            userName = "Unity User",
            postText = "画像が設定されていない投稿は、本文だけのカードとして表示されます。",
            avatarColor = new Color(0.2f, 0.8f, 0.45f)
        });

        posts.Add(new PostData
        {
            userName = "Creator",
            postText = "InspectorのPostsに要素を追加して、Post ImageへSpriteを設定すると画像付き投稿になります。",
            avatarColor = new Color(1f, 0.55f, 0.2f)
        });

        posts.Add(new PostData
        {
            userName = "Past Post",
            postText = "投稿数が増えると縦方向にスクロールできます。古い投稿を下に並べる想定です。",
            avatarColor = new Color(0.65f, 0.35f, 1f)
        });
    }

    /// <summary>
    /// Canvas、ScrollRect、Viewport、Contentを自動生成します。
    /// 既存のCanvasがシーンにあればそれを使用し、なければ新規作成します。
    /// </summary>
    private void BuildUI()
    {
        canvas = FindObjectOfType<Canvas>();

        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("Canvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();
        }

        // EventSystemがないとスクロール操作やUIクリックが正しく動かないため生成します。
        CreateEventSystemIfNeeded();

        // 画面全体の背景
        GameObject rootObject = CreateUIObject("XLikeFeedRoot", canvas.transform);
        RectTransform rootRect = rootObject.GetComponent<RectTransform>();
        StretchToParent(rootRect);

        Image background = rootObject.AddComponent<Image>();
        background.color = backgroundColor;

        // ScrollRect本体
        GameObject scrollObject = CreateUIObject("Scroll View", rootObject.transform);
        RectTransform scrollRectTransform = scrollObject.GetComponent<RectTransform>();
        StretchToParent(scrollRectTransform);

        scrollRect = scrollObject.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Elastic;
        scrollRect.inertia = true;
        scrollRect.scrollSensitivity = 35f;

        // Viewport
        GameObject viewportObject = CreateUIObject("Viewport", scrollObject.transform);
        RectTransform viewportRect = viewportObject.GetComponent<RectTransform>();
        StretchToParent(viewportRect);

        Image viewportImage = viewportObject.AddComponent<Image>();
        viewportImage.color = new Color(1f, 1f, 1f, 0.01f);

        Mask viewportMask = viewportObject.AddComponent<Mask>();
        viewportMask.showMaskGraphic = false;

        // Content
        GameObject contentObject = CreateUIObject("Content", viewportObject.transform);
        contentRoot = contentObject.GetComponent<RectTransform>();

        contentRoot.anchorMin = new Vector2(0.5f, 1f);
        contentRoot.anchorMax = new Vector2(0.5f, 1f);
        contentRoot.pivot = new Vector2(0.5f, 1f);
        contentRoot.anchoredPosition = Vector2.zero;
        contentRoot.sizeDelta = new Vector2(maxPostWidth, 0f);

        VerticalLayoutGroup contentLayout = contentObject.AddComponent<VerticalLayoutGroup>();
        contentLayout.childAlignment = TextAnchor.UpperCenter;
        contentLayout.spacing = postSpacing;
        contentLayout.padding = new RectOffset(24, 24, 24, 24);
        contentLayout.childControlWidth = true;
        contentLayout.childControlHeight = true;
        contentLayout.childForceExpandWidth = true;
        contentLayout.childForceExpandHeight = false;

        ContentSizeFitter contentSizeFitter = contentObject.AddComponent<ContentSizeFitter>();
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRect.viewport = viewportRect;
        scrollRect.content = contentRoot;
    }

    /// <summary>
    /// 投稿一覧を再生成します。
    /// postsの内容を変更したあと、このメソッドを呼べば表示を更新できます。
    /// </summary>
    public void RefreshFeed()
    {
        if (contentRoot == null)
        {
            return;
        }

        // 既存の投稿UIを削除します。
        for (int i = contentRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(contentRoot.GetChild(i).gameObject);
        }

        // 投稿データからUIを生成します。
        for (int i = 0; i < posts.Count; i++)
        {
            CreatePostCard(posts[i], i);
        }

        // 次フレームでレイアウトを更新して、スクロール位置を一番上に戻します。
        Canvas.ForceUpdateCanvases();

        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }
    }

    /// <summary>
    /// 外部スクリプトから投稿を追加したい場合に使えます。
    /// 例:
    /// xLikePostFeedUI.AddPost("User", "本文", sprite);
    /// </summary>
    public void AddPost(string userName, string postText, Sprite postImage = null)
    {
        PostData newPost = new PostData
        {
            userName = userName,
            postText = postText,
            postImage = postImage,
            avatarColor = UnityEngine.Random.ColorHSV(0f, 1f, 0.5f, 0.9f, 0.8f, 1f)
        };

        // 新しい投稿を先頭に追加します。
        posts.Insert(0, newPost);

        RefreshFeed();
    }

    /// <summary>
    /// 1件分の投稿カードを作成します。
    /// </summary>
    private void CreatePostCard(PostData post, int index)
    {
        GameObject cardObject = CreateUIObject("Post Card " + index, contentRoot);
        RectTransform cardRect = cardObject.GetComponent<RectTransform>();

        LayoutElement cardLayoutElement = cardObject.AddComponent<LayoutElement>();
        cardLayoutElement.preferredWidth = maxPostWidth;
        cardLayoutElement.flexibleWidth = 1f;

        Image cardImage = cardObject.AddComponent<Image>();
        cardImage.color = postCardColor;

        VerticalLayoutGroup cardLayout = cardObject.AddComponent<VerticalLayoutGroup>();
        cardLayout.padding = new RectOffset(20, 20, 18, 18);
        cardLayout.spacing = 12f;
        cardLayout.childAlignment = TextAnchor.UpperLeft;
        cardLayout.childControlWidth = true;
        cardLayout.childControlHeight = true;
        cardLayout.childForceExpandWidth = true;
        cardLayout.childForceExpandHeight = false;

        ContentSizeFitter cardSizeFitter = cardObject.AddComponent<ContentSizeFitter>();
        cardSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // 上部エリア: アバター + ユーザー名
        GameObject headerObject = CreateUIObject("Header", cardObject.transform);
        HorizontalLayoutGroup headerLayout = headerObject.AddComponent<HorizontalLayoutGroup>();
        headerLayout.spacing = 12f;
        headerLayout.childAlignment = TextAnchor.MiddleLeft;
        headerLayout.childControlWidth = true;
        headerLayout.childControlHeight = true;
        headerLayout.childForceExpandWidth = false;
        headerLayout.childForceExpandHeight = false;

        LayoutElement headerLayoutElement = headerObject.AddComponent<LayoutElement>();
        headerLayoutElement.preferredHeight = 48f;

        // アバター
        GameObject avatarObject = CreateUIObject("Avatar", headerObject.transform);
        Image avatarImage = avatarObject.AddComponent<Image>();
        avatarImage.color = post.avatarColor;

        RectTransform avatarRect = avatarObject.GetComponent<RectTransform>();
        avatarRect.sizeDelta = new Vector2(44f, 44f);

        LayoutElement avatarLayoutElement = avatarObject.AddComponent<LayoutElement>();
        avatarLayoutElement.preferredWidth = 44f;
        avatarLayoutElement.preferredHeight = 44f;

        // ユーザー名
        GameObject userNameObject = CreateUIObject("User Name", headerObject.transform);
        Text userNameText = userNameObject.AddComponent<Text>();
        userNameText.text = string.IsNullOrEmpty(post.userName) ? "User" : post.userName;
        userNameText.font = defaultFont;
        userNameText.fontSize = 24;
        userNameText.fontStyle = FontStyle.Bold;
        userNameText.color = userNameColor;
        userNameText.alignment = TextAnchor.MiddleLeft;

        LayoutElement userNameLayoutElement = userNameObject.AddComponent<LayoutElement>();
        userNameLayoutElement.flexibleWidth = 1f;
        userNameLayoutElement.preferredHeight = 44f;

        // 本文
        GameObject bodyObject = CreateUIObject("Post Text", cardObject.transform);
        Text bodyText = bodyObject.AddComponent<Text>();
        bodyText.text = post.postText;
        bodyText.font = defaultFont;
        bodyText.fontSize = 22;
        bodyText.color = textColor;
        bodyText.alignment = TextAnchor.UpperLeft;
        bodyText.horizontalOverflow = HorizontalWrapMode.Wrap;
        bodyText.verticalOverflow = VerticalWrapMode.Overflow;
        bodyText.lineSpacing = 1.15f;

        ContentSizeFitter bodySizeFitter = bodyObject.AddComponent<ContentSizeFitter>();
        bodySizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // 画像が設定されている場合のみ画像UIを作成します。
        if (post.postImage != null)
        {
            GameObject imageObject = CreateUIObject("Post Image", cardObject.transform);
            Image postImage = imageObject.AddComponent<Image>();
            postImage.sprite = post.postImage;
            postImage.color = Color.white;
            postImage.preserveAspect = true;

            RectTransform imageRect = imageObject.GetComponent<RectTransform>();

            // 画像の比率に合わせて高さを決めます。
            float spriteWidth = post.postImage.rect.width;
            float spriteHeight = post.postImage.rect.height;
            float aspect = spriteHeight / Mathf.Max(spriteWidth, 1f);

            float imageWidth = maxPostWidth - 80f;
            float imageHeight = Mathf.Clamp(imageWidth * aspect, 180f, 520f);

            imageRect.sizeDelta = new Vector2(imageWidth, imageHeight);

            LayoutElement imageLayoutElement = imageObject.AddComponent<LayoutElement>();
            imageLayoutElement.preferredWidth = imageWidth;
            imageLayoutElement.preferredHeight = imageHeight;
        }

        // 下部の薄い区切り線
        GameObject lineObject = CreateUIObject("Divider", cardObject.transform);
        Image lineImage = lineObject.AddComponent<Image>();
        lineImage.color = new Color(1f, 1f, 1f, 0.08f);

        LayoutElement lineLayoutElement = lineObject.AddComponent<LayoutElement>();
        lineLayoutElement.preferredHeight = 1f;

        cardRect.localScale = Vector3.one;
    }

    /// <summary>
    /// UI用GameObjectを作成し、RectTransformを設定します。
    /// </summary>
    private GameObject CreateUIObject(string objectName, Transform parent)
    {
        GameObject uiObject = new GameObject(objectName);
        uiObject.transform.SetParent(parent, false);

        RectTransform rectTransform = uiObject.AddComponent<RectTransform>();
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.anchoredPosition = Vector2.zero;

        return uiObject;
    }

    /// <summary>
    /// RectTransformを親いっぱいに広げます。
    /// </summary>
    private void StretchToParent(RectTransform rectTransform)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;
    }

    /// <summary>
    /// EventSystemがシーンに存在しない場合に作成します。
    /// ScrollRectをマウスやタッチで操作するために必要です。
    /// </summary>
    private void CreateEventSystemIfNeeded()
    {
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystemObject.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
    }
}