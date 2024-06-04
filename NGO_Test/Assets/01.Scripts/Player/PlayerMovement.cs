using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpPower = 6f;

    private Rigidbody2D rigid;

    [SerializeField] private float jumpDelay = 0.2f;
    [SerializeField] private float groundDetectRange = 1.1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform visualTrm;
    private float jumpTimeCounter = 0;
    private bool canMove = true;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!IsOwner || !canMove) return;

        float x = Input.GetAxisRaw("Horizontal");

        Vector2 velocity = new Vector2(x * speed, rigid.velocity.y);
        rigid.velocity = velocity;

        if (x == 1)
            visualTrm.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        else if (x == -1)
            visualTrm.rotation = Quaternion.Euler(new Vector3(0, 180, 0));

        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        if (jumpTimeCounter < jumpDelay)
            jumpTimeCounter += Time.deltaTime;
    }

    private void Jump()
    {
        if (CanJump())
            rigid.AddForce(Vector3.up * jumpPower, ForceMode2D.Impulse);
    }

    private bool CanJump()
    {
        bool canJump = false;
        if (jumpTimeCounter >= jumpDelay && IsGround())
        {
            jumpTimeCounter = 0;
            canJump = true;
        }

        return canJump;
    }

    [ClientRpc]
    public void KnockbackClientRpc(Vector3 direction, float force, ClientRpcParams rpcParams = default)
    {
        StartCoroutine(OnKnockback(direction, force));
    }

    private IEnumerator OnKnockback(Vector3 direction, float force)
    {
        canMove = false;
        rigid.AddForce(direction * force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(1f);
        canMove = true;
    }

    private bool IsGround() => Physics2D.Raycast(transform.position, Vector3.down, groundDetectRange, groundLayer);

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * groundDetectRange);
        Gizmos.color = Color.white;
    }
#endif
}
