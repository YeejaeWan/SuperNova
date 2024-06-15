using UnityEngine;

public class SkillBullet : MonoBehaviour
{
    public float speed = 5f;
    public float duration = 3f;
    private Vector2 direction;
    private float timer = 0f;

    public void Init(Vector2 direction, float duration)
    {
        this.direction = direction;
        this.duration = duration;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        timer += Time.deltaTime;
        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().Stun();
            Destroy(gameObject);
        }
    }
}
