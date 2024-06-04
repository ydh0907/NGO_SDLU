using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private async void Start()
    {
        await AuthManager.Instance.Authorization();
        SceneManagement.Instance.LoadScene(SceneEnum.JoinScene);
    }
}
