using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TwitterPostItem : MonoBehaviour
{
    [SerializeField] private Image avatarImage;
    [SerializeField] private TextMeshProUGUI userNameText;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private Image postImage;

    public void SetUp(string userName, string postText, Sprite image, Color avatarColor)
    {
        if (avatarImage != null)
        {
            avatarImage.color = avatarColor;
        }

        if (userNameText != null)
        {
            userNameText.text = string.IsNullOrEmpty(userName) ? "Unknown User" : userName;
        }
        if (bodyText != null)
        {
            bodyText.text = postText;
        }

        if ( postImage != null)
        {
            if (image == null)
            {
                postImage.gameObject.SetActive(false);
            }
            else
            {
                postImage.gameObject.SetActive(true);
                postImage.sprite = image;
            }
        }   
    }
}
