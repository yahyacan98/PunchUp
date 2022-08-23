﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public bool facingRight;

    public List<Loot> Loots;
    public int BaseDamage = 10;
    private int Damage;

    public int maxHealth = 100;
    public int currentHealth;

    public Transform PunchPoint;
    public float punchRange = 0.5f;

    public Transform KickPoint;
    public float kickRange = 0.5f;

    private SoundManager soundmanager;
    public HealthBar healthBar;
    public GameObject HealthBarObject;
    private HitText HitText;
    private GameObject HitsObject;

    private AudioSource AudioSource;
    public bool isDead = false;
    bool isKnockedUp;
    public bool isFrozen = false;

    public float speed;
    public float chaseDistance;
    public float stopDistanceX;
    public float stopDistanceY;
    public bool isAttacking = false;
    public bool tookDamage = false;
    public bool isWalking = false;

    public GameObject target;

    public bool targetClose = false;

    public LayerMask playerLayer;

    Animator animator;

    public float targetDistanceX;
    public float targetDistanceY;

    public float AttackSpeedTime = 2;

    public LevelManager levelManager;
    public LevelEnemyChecker levelEnemyChecker;

    public AudioSource TargetAudioSource;


    public int EXP;

    public int EnemyCount;


    private void Awake()
    {
        levelEnemyChecker = GameObject.Find("LevelEnemyChecker").GetComponent<LevelEnemyChecker>();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();

        HealthBarObject = Instantiate(Resources.Load("Prefabs/HealthBar")) as GameObject;
        //HealthBarObject.transform.parent = GameObject.Find("EnemyHealthbars").transform;
        HealthBarObject.transform.localScale = new Vector3(1, 1, 1);
        healthBar = HealthBarObject.GetComponent<HealthBar>();
    }
    void Start()
    {
        Damage = BaseDamage + levelManager.Level; // Level'a göre Düşman Damage
        AttackSpeedTime = 2 - (float)levelManager.Level / 30; // Level'a göre Düşman attack hızı
        maxHealth = 100 + levelManager.Level * 5; // Level'a göre Düşman canı
        healthBar.SetMaxHealth(maxHealth);


        AudioSource = GetComponent<AudioSource>();
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        target = GameObject.Find("Fighter");
        TargetAudioSource = target.GetComponent<PlayerCombat>().AudioSource;
        AudioSource = GetComponent<AudioSource>();
        soundmanager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        stopDistanceX = Random.Range(stopDistanceX - 0.3f, stopDistanceX + 0.3f);
        stopDistanceY = Random.Range(stopDistanceY - 0.1f, stopDistanceY + 0.1f);

    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        tookDamage = true;
        isAttacking = false;
        float pushPower = (float)damage / 200 * 3;

        if(target.GetComponent<PlayerCombat>().lastDamageIsPunch)
        {
            pushPower /= 2;
        }
        

        hits(damage);

        if (facingRight && !isKnockedUp)
        {
            transform.position = new Vector2(transform.position.x - pushPower, transform.position.y);
        }
        if (!facingRight && !isKnockedUp)
        {
            transform.position = new Vector2(transform.position.x + pushPower, transform.position.y);
        }

        AudioSource.clip = target.GetComponent<PlayerCombat>().currentAttackSound;
        AudioSource.Play();

        if(damage > maxHealth/3)
        {
            KnockUp();
        }
        if (!isKnockedUp)
            animator.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();

        }
        healthBar.SetHealth(currentHealth);


    }

    public void KnockUp()
    {

        if (!isKnockedUp)
        {
            animator.SetTrigger("KnockDown");
            isKnockedUp = true;
        }



    }

    public void LootDrop()
    {
        float playerLuck = GameObject.Find("Fighter").GetComponent<Skills>().luckRatio;
      



        foreach (var loot in Loots)
        {
            float dice = Random.Range(0, 100);

            if (dice >= 100 - (loot.DropRate + loot.DropRate * playerLuck / 10))
            {
                if (loot.ID == 1)
                {
                    Debug.Log("Loot düştü : " + loot.name);
                    Instantiate(Resources.Load("Prefabs/Loots/Watermelon Loot"), transform.position, Quaternion.identity);
                }

                else if (loot.ID == 2)
                {
                    Debug.Log("Loot düştü : " + loot.name);
                    Instantiate(Resources.Load("Prefabs/Loots/Gold Loot"), new Vector3(transform.position.x + 1, transform.position.y, transform.position.z), Quaternion.identity);
                }

                else if (loot.ID == 3)
                {
                    Debug.Log("Loot düştü : " + loot.name);
                    Instantiate(Resources.Load("Prefabs/Loots/GoldPack Loot"), new Vector3(transform.position.x + 1, transform.position.y, transform.position.z), Quaternion.identity);
                }

                target.GetComponent<AudioSource>().PlayOneShot(loot.DropSound);
            }
        }

    }

    public void Destroy()
    {
        target.GetComponent<PlayerCombat>().GainExp(EXP);
        levelEnemyChecker.EnemyDied();
        LootDrop();
        Destroy(HealthBarObject);
        Destroy(HitsObject);
        Destroy(gameObject);
    }


    void Die()
    {
        isDead = true;

        animator.SetBool("IsDead", isDead);

    }
    // Update is called once per frame
    void Update()
    {
        healthBar.transform.position = new Vector2(transform.position.x, transform.position.y + 3.2f);

        if (transform.position.x < target.transform.position.x && !facingRight)
        {
            Flip();
        }

        else if (transform.position.x > target.transform.position.x && facingRight)
        {
            Flip();
        }

        if (!isFrozen)
        {
            if (isDead) // Bazen bug oluyor ölse de ölme animasyonuna girmiyor bu onu engellemek için
            {
                Die();
            }


            targetDistanceX = Mathf.Abs(transform.position.x - target.transform.position.x);
            targetDistanceY = Mathf.Abs(transform.position.y - target.transform.position.y);

            if (!isDead && !isKnockedUp && !tookDamage)
            {

                if (targetDistanceX < chaseDistance && targetDistanceX > stopDistanceX || (targetDistanceY < chaseDistance && targetDistanceY > stopDistanceY && !isKnockedUp && targetDistanceX < chaseDistance && targetDistanceX > stopDistanceX))
                    ChasePlayer();
                else
                    StopChasePlayer();

                if (targetDistanceX < stopDistanceX && targetDistanceY < stopDistanceY)
                    targetClose = true;
            }
            else
            {
                //Bi şey yapma
            }
        }
        else
        {
            if(Vector2.Distance(transform.position,target.transform.position) > 5f)
            {
                ChasePlayer();
            }
            else
            {
                StopChasePlayer();
            }
        }
    }


    

    private void PunchDamageAgent()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(PunchPoint.position, kickRange, playerLayer);
        if (hitEnemies.Length > 0)
        {

            TargetAudioSource.clip = soundmanager.Punch2;
            TargetAudioSource.Play();
        }
        foreach (Collider2D player in hitEnemies)
        {
            if (!player.GetComponent<PlayerCombat>().isDead)
            {
                player.GetComponent<PlayerCombat>().TakeDamage(Damage, gameObject);
            }
        }
    }


    private void Attack1()
    {
        animator.SetTrigger("Attack1");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(PunchPoint.position, kickRange, playerLayer);
        if (hitEnemies.Length > 0)
        {
            TargetAudioSource.clip = soundmanager.Punch2;
            TargetAudioSource.Play();
        }
        foreach (Collider2D player in hitEnemies)
        {
            if (!player.GetComponent<PlayerCombat>().isDead)
            {
                player.GetComponent<PlayerCombat>().TakeDamage(Damage, gameObject);
            }
        }
    }

    private void Attack2()
    {
        animator.SetTrigger("Attack2");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(KickPoint.position, kickRange, playerLayer);
        if (hitEnemies.Length > 0)
        {
            TargetAudioSource.clip = soundmanager.Kick2;
            TargetAudioSource.Play();
        }
        foreach (Collider2D player in hitEnemies)
        {
            if (!player.GetComponent<PlayerCombat>().isDead)
            {
                player.GetComponent<PlayerCombat>().TakeDamage(Damage, gameObject);
            }
        }
    }

    private void PistolSound()
    {
        AudioSource.clip = soundmanager.GunShot;
        AudioSource.Play();
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(PunchPoint.position, kickRange, playerLayer);

        if (hitEnemies.Length > 0)
        {

            TargetAudioSource.clip = soundmanager.GunShot;
            TargetAudioSource.Play();


        }

        foreach (Collider2D player in hitEnemies)
        {
            if (!player.GetComponent<PlayerCombat>().isDead)
            {
                player.GetComponent<PlayerCombat>().TakeDamage(Damage * 3, gameObject);

            }
        }

    }
    private void Attack3()
    {
        animator.SetTrigger("Attack3");
        if(gameObject.name != "Agent(Clone)")
        {

        
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(PunchPoint.position, kickRange, playerLayer);
        if (hitEnemies.Length > 0)
        {
            
                AudioSource.clip = soundmanager.Punch1;
                AudioSource.Play();
            
            
        }
        foreach (Collider2D player in hitEnemies)
        {
            if (!player.GetComponent<PlayerCombat>().isDead)
            {
                player.GetComponent<PlayerCombat>().TakeDamage(Damage * 2, gameObject);

            }
        }
        }
    }

    private IEnumerator AttackPlayer()
    {


        isAttacking = true;
        yield return new WaitForSeconds(AttackSpeedTime);

        if (targetClose && !tookDamage && !isFrozen)
        {


            int AttackNo = Random.Range(0, 5);
            switch (AttackNo)
            {
                case 0:
                    Attack1();

                    break;

                case 1:
                    Attack2();

                    break;

                case 2:
                    Attack1();
                    break;

                case 3:
                    Attack2();

                    break;

                case 4:
                    Attack3();
                    break;

                default:
                    break;


            }
        }
        else
        {
            isAttacking = false;
        }


        tookDamage = false;
        StopAllCoroutines();
    }

    private void StopChasePlayer()
    {
        isWalking = false;
        animator.SetBool("IsWalking", isWalking);

        if (!isAttacking)
            StartCoroutine(AttackPlayer());
    }

    public void AlertObservers(string message)
    {
        if (message == "AttackEnded")
        {
            isAttacking = false;
        }

        if (message == "StandUp")
        {
            isKnockedUp = false;
        }

        if (message == "HitEnded")
        {
            tookDamage = false;
        }

        if (message == "Punch1")
        {
            PunchDamageAgent();
        }

        if (message == "Punch2")
        {
            PunchDamageAgent();
        }

    }

    private void ChasePlayer()
    {
        targetClose = false;



        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

        isWalking = true;
        animator.SetBool("IsWalking", isWalking);
    }

   


    private void Flip()
    {

        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

    }

    public void hits(int damage)
    {
        HitsObject = Instantiate(Resources.Load("Prefabs/Hit")) as GameObject;
        HitsObject.transform.parent = GameObject.Find("EnemyHealthbars").transform;
        HitsObject.transform.localScale = new Vector3(1, 1, 1);
        HitsObject.transform.position = new Vector2(transform.position.x, transform.position.y + 4f);
        HitsObject.GetComponentInChildren<Text>().text = "-" + damage;

        HitText = HitsObject.GetComponent<HitText>();


    }


}
