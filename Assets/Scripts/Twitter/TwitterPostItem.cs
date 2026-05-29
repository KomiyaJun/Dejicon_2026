using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Threading;

public class TwitterPostItem : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image avatarImage;
    [SerializeField] private TextMeshProUGUI userNameText;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private Image postImage;
    [SerializeField] private TextMeshProUGUI timeText;

    [Header("いいねUI")]
    [SerializeField] private Button likeButton;
    [SerializeField] private Image likeIcon;
    [SerializeField] private TextMeshProUGUI likeCountText;

    [SerializeField] private Sprite normalHeartSprite;
    [SerializeField] private Sprite likedHeartSprite;

    [SerializeField] private Color normalLikeColor = new Color(0.45f, 0.5f, 0.55f, 1f);
    [SerializeField] private Color likedColor = new Color(1f, 0.1f, 0.35f, 1f);

    [Header("メモ連携")]
    [SerializeField] private WindowData memoWindowData;
    [SerializeField] private WindowData mapWindowData;
    [SerializeField] private WindowData defaultWindowData;
    [SerializeField] private float waitTimeout = 5.0f;
    [SerializeField] private Button postImageButton;
    [SerializeField] private Image imageMemoFrame;
    [SerializeField] private Color memoColor = new Color(0.25f, 0.8f, 1f, 1f);

    private string imageMemoKey;

    private int likeCount;
    private bool isLiked;

    public void Bind(global::PostData data)
    {
        if (data == null) return;

        this.imageMemoKey = data.photoLinkID;

        if (avatarImage != null)
        {
            if (data.accountIcon != null)
            {
                avatarImage.sprite = data.accountIcon;
                avatarImage.color = Color.white;
            }
            else
            {
                avatarImage.sprite = null;
                // アバターがない場合はPostDataの指定色にする（Twitterの既存挙動維持）
                avatarImage.color = data.avatarColor;
            }
        }

        if (userNameText != null)
        {
            userNameText.text = string.IsNullOrEmpty(data.accountName) ? "Unknown User" : data.accountName;
        }

        if (timeText != null)
        {
            timeText.text = string.IsNullOrEmpty(data.timeAgo)? "" : "" + data.timeAgo;
        }

        if (bodyText != null)
        {
            bodyText.text = (data.caption ?? "").Replace("\\n", "\n");
        }

        if (postImage != null)
        {
            if (data.postPhoto == null)
            {
                postImage.gameObject.SetActive(false);
            }
            else
            {
                postImage.gameObject.SetActive(true);
                postImage.sprite = data.postPhoto;
                postImage.preserveAspect = true;
            }
        }

        if (postImageButton != null)
        {
            postImageButton.onClick.RemoveAllListeners();

            if (data.postPhoto != null && !string.IsNullOrEmpty(imageMemoKey))
            {
                postImageButton.onClick.AddListener(OnPostImageClicked);
            }
        }

        if (imageMemoFrame != null)
        {
            imageMemoFrame.gameObject.SetActive(data.postPhoto != null && !string.IsNullOrEmpty(imageMemoKey));
            imageMemoFrame.color = memoColor;
        }

        likeCount = data.likeCount;
        isLiked = false;

        if (likeButton != null)
        {
            likeButton.onClick.RemoveAllListeners();
            likeButton.onClick.AddListener(OnLikeButtonClicked);
        }

        UpdateLikeUI();
    }

    private void OnLikeButtonClicked()
    {
        if (isLiked)
        {
            isLiked = false;
            likeCount--;
        }
        else
        {
            isLiked = true;
            likeCount++;
        }

        if (likeCount < 0)
        {
            likeCount = 0;
        }

        UpdateLikeUI();
    }

    private void UpdateLikeUI()
    {
        if (likeIcon != null)
        {
            likeIcon.sprite = isLiked ? likedHeartSprite : normalHeartSprite;
            likeIcon.color = isLiked ? likedColor : normalLikeColor;
        }

        if (likeCountText != null)
        {
            likeCountText.text = likeCount.ToString();
            likeCountText.color = isLiked ? likedColor : normalLikeColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (bodyText == null) return;

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(
            bodyText,
            eventData.position,
            eventData.pressEventCamera
        );

        if (linkIndex == -1) return;

        TMP_LinkInfo linkInfo = bodyText.textInfo.linkInfo[linkIndex];
        string linkID = linkInfo.GetLinkID();

        if (linkID.StartsWith("memo_"))
        {
            // 元のロジックに代わり汎用リンク処理へ
            OnLinkClicked(linkID);
        }
        else
        {
            // memo_ 以外のもの（map_ 等）が来た場合も通す
            OnLinkClicked(linkID);
        }
    }

    private void OnPostImageClicked()
    {
        OnLinkClicked(imageMemoKey);
    }

    private Transform GetWindowParent()
    {
        GameObject obj = GameObject.Find("Window_Parent");
        if (obj != null) return obj.transform;
        Debug.LogWarning("[TwitterPostItem] Window_Parent が見つかりません");
        return null;
    }

    private void OpenWindow(WindowData data)
    {
        if (data == null) return;

        Transform parent = GetWindowParent();

        if (WindowCache.Instance != null)
        {
            WindowCache.Instance.OpenWindow(data, parent);
        }
        else if (WindowService.Instance != null)
        {
            WindowService.Instance.OpenWindow(data);
        }
    }

    private void OpenWindowAndActivate(WindowData data, string key)
    {
        if (WindowCache.Instance != null && WindowCache.Instance.IsOpen(data))
        {
            WindowBase window = WindowCache.Instance.GetWindow(data);
            window?.transform.SetAsLastSibling();
            StartCoroutine(WaitForWindowMemoAndActivate(key));
            return;
        }

        OpenWindow(data);
        StartCoroutine(WaitForWindowMemoAndActivate(key));
    }

    private IEnumerator WaitForWindowMemoAndActivate(string key)
    {
        float elapsed = 0f;
        while (WindowMemo.Instance == null && elapsed < waitTimeout)
        {
            yield return null;
            elapsed += Time.unscaledDeltaTime;
        }

        if (WindowMemo.Instance == null)
        {
            Debug.LogWarning($"[TwitterPostItem] WindowMemo.Instance が {waitTimeout}秒 経っても取得できませんでした。キー: {key}");
            yield break;
        }

        WindowMemo.Instance.ActivateContent(key);
        Debug.Log($"[TwitterPostItem] {key} を活性化しました");
    }

    private void OnLinkClicked(string id)
    {
        if (string.IsNullOrEmpty(id)) return;

        if (id.Contains("+"))
        {
            string[] parts = id.Split('+');
            foreach (string part in parts)
            {
                string trimmed = part.Trim();
                if (!string.IsNullOrEmpty(trimmed))
                    OnLinkClicked(trimmed);
            }
            return;
        }

        if (id.StartsWith("memo_"))
        {
            string key = id.Replace("memo_", "");
            OpenWindowAndActivate(memoWindowData, key);
        }
        else if (id.StartsWith("map_"))
        {
            string key = id.Replace("map_", "");
            MapPinDatabase.Instance?.RevealPin(key);
            OpenWindow(mapWindowData);
            StartCoroutine(SwitchMapAfterWindowOpens(key));
        }
        else
        {
            OpenWindow(defaultWindowData);
        }
    }

    private IEnumerator SwitchMapAfterWindowOpens(string key)
    {
        if (key != "kouku" && key != "nokou" && key != "toggle")
            yield break;

        float elapsed = 0f;
        while (MapKoukuController.ActiveInstance == null && elapsed < waitTimeout)
        {
            yield return null;
            elapsed += Time.unscaledDeltaTime;
        }

        if (MapKoukuController.ActiveInstance == null)
        {
            Debug.LogWarning("[TwitterPostItem] MapKoukuController.ActiveInstance が見つかりません");
            yield break;
        }

        switch (key)
        {
            case "kouku":
                MapKoukuController.ActiveInstance.ShowKoukuOn();
                break;
            case "nokou":
                MapKoukuController.ActiveInstance.ShowKoukuOff();
                break;
            case "toggle":
                MapKoukuController.ActiveInstance.ToggleKouku();
                break;
        }
    }
}