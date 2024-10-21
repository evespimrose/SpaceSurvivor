using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 10;

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float duration = 3f;

    void Start()
    {
        Destroy(gameObject,duration);
    }
    void Update()
    {
        Move(Vector2.up);
    }

    public void Move(Vector2 dir)
    {
        transform.Translate(dir * moveSpeed * Time.deltaTime);
    }

    public void SetDamage(float damageAmount)
    {
        damage = damageAmount;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            PacketManager.Instance.SendMessage(new PlayerMessage(gameObject, collision.gameObject, damage));
            Destroy(gameObject);
        }
    }
}
