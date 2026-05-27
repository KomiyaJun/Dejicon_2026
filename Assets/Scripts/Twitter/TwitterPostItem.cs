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
    [SerializeField] private Button postImageButton;
    [SerializeField] private Image imageMemoFrame;
    [SerializeField] private Color memoColor = new Color(0.25f, 0.8f, 1f, 1f);

    private string imageMemoKey;

    private int likeCount;
    private bool isLiked;

    public void SetUp(string userName, string postText, Sprite image, Sprite avatarIcon, Color avatarColor, int startLikeCount, string postTime, string imageMemoKey)
    {
        this.imageMemoKey = imageMemoKey;

        if (avatarImage != null)
        {
            if (avatarIcon != null)
            {
                avatarImage.sprite = avatarIcon;
                avatarImage.color = Color.white;
            }
            else
            {
                avatarImage.sprite = null;
                avatarImage.color = avatarColor;
            }
        }

        if (userNameText != null)
        {
            userNameText.text = string.IsNullOrEmpty(userName) ? "Unknown User" : userName;
        }

        if (timeText != null)
        {
            timeText.text = string.IsNullOrEmpty(postTime)? "" : "" + postTime;
        }

        if (bodyText != null)
        {
            bodyText.text = postText.Replace("\\n", "\n");
        }

        if (postImage != null)
        {
            if (image == null)
            {
                postImage.gameObject.SetActive(false);
            }
            else
            {
                postImage.gameObject.SetActive(true);
                postImage.sprite = image;
                postImage.preserveAspect = true;
            }
        }

        if (postImageButton != null)
        {
            postImageButton.onClick.RemoveAllListeners();

            if (image != null && !string.IsNullOrEmpty(imageMemoKey))
            {
                postImageButton.onClick.AddListener(OnPostImageClicked);
            }
        }

        if (imageMemoFrame != null)
        {
            imageMemoFrame.gameObject.SetActive(image != null && !string.IsNullOrEmpty(imageMemoKey));
            imageMemoFrame.color = memoColor;
        }

        likeCount = startLikeCount;
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
            string key = linkID.Replace("memo_", "");
            ActivateMemo(key);
        }
    }

    private void OnPostImageClicked()
    {
        ActivateMemo(imageMemoKey);
    }

    private void ActivateMemo(string key)
    {
        if (string.IsNullOrEmpty(key)) return;

        if (memoWindowData == null)
        {
            Debug.LogWarning("memoWindowData が設定されていません");
            return;
        }

        if (WindowService.Instance == null)
        {
            Debug.LogWarning("WindowService.Instance が見つかりません");
            return;
        }

        // メモウィンドウがまだ表示されていない時だけ開く
        if (!WindowService.Instance.IsWindowVisible(memoWindowData))
        {
            WindowService.Instance.OpenWindow(memoWindowData);
        }

        StartCoroutine(ActivateMemoAfterOpen(key));
    }

    private IEnumerator ActivateMemoAfterOpen(string key)
    {
        float timeout = 3f;
        float timer = 0f;

        while (WindowMemo.Instance == null && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (WindowMemo.Instance == null)
        {
            Debug.LogWarning("WindowMemo.Instance が見つかりませんでした");
            yield break;
        }

        WindowMemo.Instance.ActivateContent(key);
        Debug.Log($"{key} をメモに保存しました");
    }



}