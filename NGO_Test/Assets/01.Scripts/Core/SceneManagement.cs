using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneEnum
{
    Bootstrap = 0,
    JoinScene,
    GameScene,
}

public class SceneManagement : MonoBehaviour
{
    public static SceneManagement Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        Instance = this;
    }

    public void LoadScene(SceneEnum scene)
    {
        SceneManager.LoadScene((int)scene);
    }
}
