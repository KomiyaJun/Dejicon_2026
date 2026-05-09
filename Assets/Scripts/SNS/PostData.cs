using UnityEngine;

[CreateAssetMenu(fileName = "PostData", menuName = "SNS/Post Data")]
public class PostData : ScriptableObject
{
    [Header("アカウント情報")]
    public string accountName;      // 例: "user_taro"
    public Sprite accountIcon;      // プロフィール画像

    [Header("投稿内容")]
    public Sprite postPhoto;        // 投稿写真
    [TextArea(2, 5)]
    public string caption;          // 投稿文章

    [Header("エンゲージメント")]
    public int likeCount;
    public string timeAgo;          // 例: "2時間前"
}