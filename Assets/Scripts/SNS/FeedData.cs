using UnityEngine;

[CreateAssetMenu(fileName = "FeedData", menuName = "SNS/Feed Data")]
public class FeedData : ScriptableObject
{
    public PostData[] posts;
}