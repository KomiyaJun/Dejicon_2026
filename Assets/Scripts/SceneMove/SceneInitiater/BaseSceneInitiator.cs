using UnityEngine;

// すべてのシーン初期化スクリプトの「型」となる抽象クラス
public abstract class BaseSceneInitiator : MonoBehaviour
{
    // 画面が見えない裏側でやりたい処理をここに書く（子クラスで上書きする）
    public abstract void PrepareSceneData();

    // フェードアウトが終わって画面が見えた時の処理をここに書く（子クラスで上書きする）
    public abstract void StartScene();
}