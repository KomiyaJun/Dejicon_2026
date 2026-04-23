using UnityEngine;
using Cysharp.Threading.Tasks;

//ウィンドウを開く時と閉じる時、なんらかの特殊な処理を追加したい場合用のabstractクラス
//開いたときにローディング処理をしたい。閉じるときに何らかの演出を入れたいといった場合に利用
//利用したい場合、クラスを新規で作成し WindowBaseを継承 OnOpen,OnCloseをそれぞれ下記の形でoverrideしてください。
//下記は元の処理にDebug.Logを追加しただけのもの。

//担当者 : 小宮
public class WindowA : WindowBase
{
    protected override async UniTask OnOpen()
    {
        await base.OnOpen();
        Debug.Log("OnOpen");
    }
    protected override async UniTask OnClose()
    {
        await base.OnClose();
        Debug.Log("OnClose");
    }
}
