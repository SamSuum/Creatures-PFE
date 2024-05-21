using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    void Update()
    {
        transform.Translate(Vector3.right * 10);
        Destroy(this.gameObject, 4f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
      
        if (collision.transform.CompareTag("Enemy"))
        {
            collision.transform.GetComponent<Enemy>().TakeDamage();
            Destroy(gameObject);
        }

    }
}
