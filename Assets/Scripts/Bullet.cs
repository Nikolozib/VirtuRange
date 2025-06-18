using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 200f;

    void Start()
    {
        Destroy(gameObject, 3f); // Destroy bullet after 3 seconds to avoid clutter
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Bullet hit: " + collision.gameObject.name);

        TargetSpawn target = collision.gameObject.GetComponent<TargetSpawn>();
        if (target != null)
        {
            target.Hit();
        }

        // Destroy the bullet on collision except when hitting ground or player
        if (collision.gameObject.tag != "Ground" && collision.gameObject.tag != "Player")
        {
            Destroy(gameObject);
        }
    }
}