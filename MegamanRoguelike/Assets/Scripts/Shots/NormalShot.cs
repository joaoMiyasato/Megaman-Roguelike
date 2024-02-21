using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalShot : MonoBehaviour
{
    public int damage;
    public float speed;
    public bool rightDirection;
    Rigidbody2D rb;

    public float destructionTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        destructionTime -= Time.deltaTime;

        if(destructionTime < 0 )
        {
            Destroy(this.gameObject);
        }
    }
    private void FixedUpdate()
    {
        if(rightDirection)
        {
            rb.velocity = Vector2.right * speed;
        }
        else
        {
            rb.velocity = Vector2.left * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision != null)
        {
            if (collision.gameObject.layer == 8)
            {
                if(collision.gameObject.tag == "DamageBox")
                {
                    collision.gameObject.GetComponentInParent<scr_BaseEntityStats>().TakeDamage(damage, false);
                    speed = 0;
                    Destroy(this.gameObject.GetComponent<SpriteRenderer>());
                }
            }
        }
    }
}
