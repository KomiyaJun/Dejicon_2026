using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapPinData))]
public class MapPinDataEditor : Editor
{
    private int selectedPinIndex = 0;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty mapTexProp = serializedObject.FindProperty("previewMapTexture");
        EditorGUILayout.PropertyField(mapTexProp, new GUIContent("プレビュー用マップ画像"));

        SerializedProperty pinsProp = serializedObject.FindProperty("pins");
        EditorGUILayout.PropertyField(pinsProp, true);

        Texture2D mapTex = (Texture2D)mapTexProp.objectReferenceValue;

        if (mapTex != null && pinsProp.arraySize > 0)
        {
            GUILayout.Space(20);
            GUILayout.Label("ビジュアルピン配置エディター", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("マップ画像をクリックすると、選択中のピンの座標が自動で更新されます。", MessageType.Info);

            // 編集するピンを選択するポップアップ
            string[] pinNames = new string[pinsProp.arraySize];
            for (int i = 0; i < pinsProp.arraySize; i++)
            {
                var pinProp = pinsProp.GetArrayElementAtIndex(i);
                string id = pinProp.FindPropertyRelative("pinId").stringValue;
                pinNames[i] = string.IsNullOrEmpty(id) ? $"Pin {i}" : id;
            }

            selectedPinIndex = EditorGUILayout.Popup("編集するピンを選択", selectedPinIndex, pinNames);
            if (selectedPinIndex >= pinsProp.arraySize)
            {
                selectedPinIndex = pinsProp.arraySize - 1;
            }

            if (selectedPinIndex >= 0)
            {
                GUILayout.Space(10);
                // 画像のアスペクト比に合わせて描画枠を確保
                float aspect = (float)mapTex.width / mapTex.height;
                Rect rect = GUILayoutUtility.GetAspectRect(aspect);
                GUI.DrawTexture(rect, mapTex);

                Event e = Event.current;
                
                // クリック判定
                if (e.type == EventType.MouseDown && e.button == 0 && rect.Contains(e.mousePosition))
                {
                    Vector2 localClickPos = e.mousePosition - new Vector2(rect.x, rect.y);
                    
                    // X は左から右へ 0.0 ~ 1.0
                    float normX = localClickPos.x / rect.width;
                    // Y は下から上へ 0.0 ~ 1.0 (GUI座標は上が0なので反転する)
                    float normY = 1f - (localClickPos.y / rect.height);

                    var selectedPinProp = pinsProp.GetArrayElementAtIndex(selectedPinIndex);
                    selectedPinProp.FindPropertyRelative("normalizedPosition").vector2Value = new Vector2(normX, normY);
                    
                    GUI.changed = true;
                    e.Use(); // イベントを消費
                }

                // 登録されているすべてのピンを描画
                for (int i = 0; i < pinsProp.arraySize; i++)
                {
                    var pinProp = pinsProp.GetArrayElementAtIndex(i);
                    Vector2 pos = pinProp.FindPropertyRelative("normalizedPosition").vector2Value;
                    string id = pinProp.FindPropertyRelative("pinId").stringValue;

                    // UI上の描画座標に変換
                    Vector2 drawPos = new Vector2(rect.x + rect.width * pos.x, rect.y + rect.height * (1f - pos.y));

                    // 選択中のピンは目立たせる
                    Color pinColor = (i == selectedPinIndex) ? Color.red : new Color(1f, 0.5f, 0f, 0.7f);
                    float size = (i == selectedPinIndex) ? 12f : 8f;

                    EditorGUI.DrawRect(new Rect(drawPos.x - size/2, drawPos.y - size/2, size, size), pinColor);
                    
                    if (i == selectedPinIndex)
                    {
                        GUI.Label(new Rect(drawPos.x + 10, drawPos.y - 10, 150, 20), $"<color=red><b>{id}</b></color>", new GUIStyle() { richText = true });
                    }
                    else
                    {
                        GUI.Label(new Rect(drawPos.x + 8, drawPos.y - 8, 100, 20), $"<color=orange>{id}</color>", new GUIStyle() { richText = true });
                    }
                }
            }
        }
        else if (mapTex == null)
        {
            GUILayout.Space(10);
            EditorGUILayout.HelpBox("プレビュー用マップ画像をセットすると、ビジュアルエディターが利用できます。", MessageType.Warning);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
