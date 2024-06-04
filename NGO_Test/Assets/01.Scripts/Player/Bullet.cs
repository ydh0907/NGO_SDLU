using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private bool canHit = true;
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 5;
    [SerializeField] private float destroyTime = 2f;

    private Rigidbody2D rb;

    public void Init(Collider2D owner)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
        Physics2D.IgnoreCollision(owner, GetComponent<CircleCollider2D>());
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (canHit && other.TryGetComponent(out Health health))
        {
            Vector3 direction = (other.transform.position - transform.position).normalized;
            float force = 10f;

            health.ApplyDamage(damage, direction, force);
        }
        Destroy(gameObject);
    }
}
