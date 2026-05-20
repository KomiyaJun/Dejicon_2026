using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMoveTester : MonoBehaviour
{
    [SerializeField] private string sceneName = "WindowTest";

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SceneTransitionManager.Instance.LoadScene(sceneName);
        }
    }

    private void TestA()
    {
        Debug.Log("DebugA");
        SceneManager.LoadScene("WindowTest");
    }
    private void TestB()
    {
        Debug.Log("DebugB");
    }
}
