using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TwitterPostItem : MonoBehaviour
{
    [SerializeField] private Image avatarImage;
    [SerializeField] private TextMeshProUGUI userNameText;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private Image postImage;
    [SerializeField] private TextMeshProUGUI timeText;

    [Header("‚¢‚¢‚ËUI")]
    [SerializeField] private Button likeButton;
    [SerializeField] private Image likeIcon;
    [SerializeField] private TextMeshProUGUI likeCountText;

    [SerializeField] private Color normalLikeColor = new Color(0.45f, 0.5f, 0.55f, 1f);
    [SerializeField] private Color likedColor = new Color(1f, 0.1f, 0.35f, 1f);

    private int likeCount;
    private bool isLiked;

    public void SetUp(string userName, string postText, Sprite image, Sprite avatarIcon, Color avatarColor, int startLikeCount, string postTime)
    {
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
            likeIcon.color = isLiked ? likedColor : normalLikeColor;
        }

        if (likeCountText != null)
        {
            likeCountText.text = likeCount.ToString();
            likeCountText.color = isLiked ? likedColor : normalLikeColor;
        }
    }
}