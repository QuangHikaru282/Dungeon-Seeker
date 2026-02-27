using UnityEngine;

public class IceProjectile : MonoBehaviour
{
    public float speed = 7f;
    public float lifetime = 5f;
    public int damage = 10;

    private float dirX;

    public void Init(float directionX)
    {
        dirX = directionX > 0 ? 1f : -1f;
        // Flip sprite nếu bay trái
        if (dirX < 0)
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Chỉ di chuyển trục X
        transform.Translate(Vector2.right * dirX * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerStats>()?.TakeDamage(damage);
            other.GetComponent<FrozenEffect>()?.ApplyFreeze();
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}