using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    [SerializeField] private float destroyTime = 2f;

    private void Awake()
    {
        Destroy(gameObject, destroyTime);
    }
}
