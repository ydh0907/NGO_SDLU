using Unity.Netcode;
using UnityEngine;

public class PlayerGun : NetworkBehaviour
{
    [SerializeField] private Bullet clientBullet;
    [SerializeField] private Bullet serverBullet;
    [SerializeField] private Transform gun;
    [SerializeField] private Transform firePos;

    private new CapsuleCollider2D collider;

    private void Awake()
    {
        collider = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        if (IsOwner)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Fire();
            }
            gun.right = GetMouseDirection();
        }
    }

    private void Fire()
    {
        Bullet bullet = Instantiate(clientBullet, firePos.position, gun.rotation);
        bullet.Init(collider);
        if (IsOwner)
            FireServerRpc();
    }

    [ServerRpc]
    private void FireServerRpc()
    {
        Bullet bullet = Instantiate(serverBullet, firePos.position, gun.rotation);
        bullet.Init(collider);
        FireClientRpc();
    }

    [ClientRpc]
    private void FireClientRpc()
    {
        if (!IsOwner)
            Fire();
    }

    private Vector2 GetMouseDirection()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePos - (Vector2)transform.position;
        direction.Normalize();
        return direction;
    }
}
