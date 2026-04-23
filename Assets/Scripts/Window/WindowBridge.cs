using UnityEngine;

// タスクバーのボタンからウィンドウベースの関数にアクセスするためのクラス
// 担当者 : 小宮
public class WindowBridge : MonoBehaviour
{
    [SerializeField] private WindowData data;
    [SerializeField] private GameObject parentObject;
    private WindowBase instantiatedWindow;

    private bool IsWindowVisible => instantiatedWindow != null && instantiatedWindow.gameObject.activeSelf;
    
    //閉じていたら開く、開いていたら閉じる
    public void OnButtonClicked()
    {
        if (!IsWindowVisible)
        {
            OnOpen();
        }
        else
        {
            OnClose();
        }
    }

    // ウィンドウを開く処理
    private void OnOpen()
    {
        if (instantiatedWindow == null)    //最初の呼び出しの場合
        {
            GameObject obj = Instantiate(data.prefab, parentObject.transform);

            instantiatedWindow = obj.GetComponent<WindowBase>();

            instantiatedWindow.SetUpWindow(data);
            if (instantiatedWindow == null)
            {
                Debug.LogError($"{data.prefab.name}にWindowBaseがアタッチされていません");
                return;
            }
        }
        instantiatedWindow?.Open();  //開く
    }


    // ウィンドウを閉じる処理
    public void OnClose()
    {
        instantiatedWindow?.Close();
    }

}
