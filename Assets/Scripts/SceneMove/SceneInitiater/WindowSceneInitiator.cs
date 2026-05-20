using UnityEngine;

public class WindowSceneInitiator : BaseSceneInitiator
{
    public override void PrepareSceneData()
    {
        Debug.Log("window用のシナリオテキストを読み込み中...");
    }

    public override void StartScene()
    {
        Debug.Log("windowパート開始！文字送りや立ち絵を表示します。");
    }
}
