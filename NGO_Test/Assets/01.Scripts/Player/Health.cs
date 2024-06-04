using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private TextMeshPro HealthText;

    [SerializeField] private int maxHealth = 100;
    private NetworkVariable<int> currentHealth;
    private PlayerMovement moveCompo;

    private void Awake()
    {
        currentHealth = new NetworkVariable<int>();
        moveCompo = GetComponent<PlayerMovement>();
    }

    public override void OnNetworkSpawn()
    {
        currentHealth.OnValueChanged += HandleTakeDamage;
        if (IsServer)
            currentHealth.Value = maxHealth;
        HandleTakeDamage(0, maxHealth);
    }

    private void HandleTakeDamage(int previousValue, int newValue)
    {
        HealthText.text = newValue.ToString();
    }

    public void ApplyDamage(int damage)
    {
        currentHealth.Value -= damage;

        if (currentHealth.Value <= 0)
            Destroy(gameObject);
    }

    public void ApplyDamage(int damage, Vector3 direction, float force)
    {
        currentHealth.Value -= damage;

        if (currentHealth.Value <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            ClientRpcParams rpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { OwnerClientId } }
            };
            moveCompo.KnockbackClientRpc(direction, force, rpcParams);
        }
    }
}
