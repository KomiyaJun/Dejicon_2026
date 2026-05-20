using UnityEngine;

public class TestSceneInitiator : BaseSceneInitiator
{
    public override void PrepareSceneData()
    {
        Debug.Log("ノベル用のシナリオテキストを読み込み中...");
    }

    public override void StartScene()
    {
        Debug.Log("ノベルパート開始！文字送りや立ち絵を表示します。");
    }
}
