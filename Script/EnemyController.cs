using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;


public class EnemyController : MonoBehaviour
{
    public float speed = 3.0f;
    public bool vertical;
    public float changeTime = 3.0f;
    float timer;

    Rigidbody2D rb2;
    Animator ani;
    int direction = 1;

    bool broken = true;
    public ParticleSystem smokeEffect;

    AudioSource audioSource;
    public AudioClip fixSound;
    void Start()
    {
        rb2= GetComponent<Rigidbody2D>();
        timer = changeTime;
        ani = GetComponent<Animator>();
        
        audioSource = GetComponent<AudioSource>();  
    }
    void Update()
    {
        if (!broken)
        {
            return;
        }

        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction; //折返，改变方向
            timer = changeTime;
        }
    }
    public void FixedUpdate()
    {
        if (!broken)
        {
            return; //broken之后就不再执行运动代码
        }

        Vector2 position = rb2.position;
        if (vertical)
        {
            position.y = position.y + Time.deltaTime * speed * direction; ;
            ani.SetFloat("MoveX", 0);
            ani.SetFloat("MoveY", direction);
        }
        else
        {
            position.x = position.x + Time.deltaTime * speed * direction; ;
            ani.SetFloat("MoveX", direction);
            ani.SetFloat("MoveY", 0);
        }


        rb2.MovePosition(position);
    }
    public void Fix()
    {
        ani.SetTrigger("Fixed");
        broken = false;
        audioSource.PlayOneShot(fixSound);
        smokeEffect.Stop();
        rb2.simulated = false;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        RubyController player = collision.gameObject.GetComponent<RubyController>();
        if (player != null)
        {
            player.ChangeHealth(-1);

            Debug.Log(player.health);
        }
    }
}


