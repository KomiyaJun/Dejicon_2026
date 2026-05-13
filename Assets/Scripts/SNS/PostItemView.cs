// PostItemView.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PostItemView : MonoBehaviour
{
    [Header("アカウント部分")]
    [SerializeField] private Image accountIconImage;
    [SerializeField] private TextMeshProUGUI accountNameText;

    [Header("投稿部分")]
    [SerializeField] private Image postPhotoImage;
    [SerializeField] private TextMeshProUGUI captionText;

    [Header("エンゲージメント")]
    [SerializeField] private TextMeshProUGUI likeCountText;
    [SerializeField] private TextMeshProUGUI timeAgoText;
    [SerializeField] private Button likeButton;

    private int currentLikeCount;
    private bool isLiked = false;

    public void Bind(PostData data)
    {
        accountNameText.text = data.accountName;
        captionText.text = data.caption;
        timeAgoText.text = data.timeAgo;
        currentLikeCount = data.likeCount;

        UpdateLikeDisplay();

        if (data.accountIcon != null)
            accountIconImage.sprite = data.accountIcon;

        if (data.postPhoto != null)
            postPhotoImage.sprite = data.postPhoto;

        likeButton.onClick.RemoveAllListeners();
        likeButton.onClick.AddListener(OnLikeButtonClicked);


        //リンクの色付け
        captionText.text = data.caption
    .Replace("<link=", "<color=#4A90D9><link=")
    .Replace("</link>", "</link></color>");
    }

    private void OnLikeButtonClicked()
    {
        isLiked = !isLiked;
        currentLikeCount += isLiked ? 1 : -1;
        UpdateLikeDisplay();
    }

    private void UpdateLikeDisplay()
    {
        likeCountText.text = currentLikeCount.ToString("N0") + " Likes";

        ColorBlock colors = likeButton.colors;
        colors.normalColor = isLiked ? Color.red : Color.white;
        likeButton.colors = colors;
    }
}