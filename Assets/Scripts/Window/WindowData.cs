using UnityEngine;

//ウィンドウのデータ　ウィンドウの名前や色を管理しやすくする用
//担当者 : 小宮

[CreateAssetMenu(fileName = "NewWindowData", menuName = "Window/WindowData")]
public class WindowData : ScriptableObject
{
    [Header("ウィンドウの名前")]
    public string windowName;

    [Header("アプリアイコン")]
    public Sprite icon;

    [Header("ウィンドウプレファブ")]
    public GameObject prefab;

    [Header("ウィンドウカラー")]
    public Color windowColor;
}
