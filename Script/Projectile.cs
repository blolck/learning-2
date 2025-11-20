using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class Projectile : MonoBehaviour
{

    public Rigidbody2D rb3;
    bool broken;
    
    void Awake()
    {
       rb3 = GetComponent<Rigidbody2D>();   
    }

    void Update()
    { 
        if (transform.position.magnitude > 50.0f)
        {
            Destroy(gameObject);
        }
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        EnemyController ec = other.collider.GetComponent<EnemyController>();
        if (ec != null)
        {
            ec.Fix();
        }
        Debug.Log("齿轮与" + other.gameObject + "发生了碰撞");
        Destroy(gameObject);
    }

    public void Launch(Vector2 direction, float force)
    
    {
        rb3.AddForce(direction * force);
    }



}
