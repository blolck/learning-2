using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;

    public int maxHealth = 5;
    public GameObject projectilePrefab;
    

    public int health { get { return currentHealth; } }
    int currentHealth;

    Animator ani;
    Rigidbody2D rb;
    public Vector2 lookDirection = new Vector2(1, 0);

    float horizontal;
    float vertical;

    bool isInvincible; //默认初始化为false
    float invincibleTimer; //恢复到可受伤状态之前，剩下的无敌状态时间。无敌时间计时器
    public float timeInvincible = 2.0f; //无敌时间

    bool isCoolDown;
    public float timeCoolDown = 0.5f;
    float CoolDownTimer;

    private AudioSource audioSource;
    public AudioClip throwSound;
    public AudioClip hitSound;
  

    void Start()
    {
        ani = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);
        //观察方向
        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        ani.SetFloat("Look X", lookDirection.x);
        ani.SetFloat("Look Y", lookDirection.y);
        ani.SetFloat("Speed", move.magnitude);
        //无敌判断
        if (isInvincible)
        {
            Debug.Log($"进入无敌状态，持续 {timeInvincible} 秒");
            invincibleTimer -= Time.deltaTime; //无敌时间开始
            if (invincibleTimer < 0)
                isInvincible = false;//无敌时间结束  
        }
        //冷却判断
        if (isCoolDown)
        {
            CoolDownTimer -= Time.deltaTime;  //Time.deltatime不能在按下攻击键的时候使用，只会减少按下去的那一帧        
            if (CoolDownTimer <= 0) //重置状态
            {
                isCoolDown = false;
                Debug.Log("冷却结束，可再次攻击");
            }
        }
        //射击触发
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (!isCoolDown)
            {
                Debug.Log($"进入攻击冷却状态，持续 {timeCoolDown} 秒");
                Launch();
                PlaySound(throwSound);
                CoolDownTimer = timeCoolDown;
                isCoolDown = true; // 标记为冷却中
            }
            else
            {
                Debug.Log($"冷却剩余：{CoolDownTimer:F1} 秒"); // 冷却中按J键的提示
            }
           
        }
        //对话触发
        if (Input.GetKeyDown(KeyCode.F))
        {
            RaycastHit2D hit = Physics2D.Raycast(rb.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                { character.DisplayDialog(); }           
            }
        }
    }
    //人物移动
    void FixedUpdate()

    {
        Vector2 position = rb.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rb.MovePosition(position);
      
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            ani.SetTrigger("Hit");           
            if (isInvincible)
            { return; }
            else
            {
                isInvincible = true;
                invincibleTimer = timeInvincible;
                audioSource.PlayOneShot(hitSound);
            }
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    public void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rb.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>(); //引用Projectile脚本
        projectile.Launch(lookDirection, 300);

        ani.SetTrigger("Launch");
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}

