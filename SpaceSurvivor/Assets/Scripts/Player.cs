using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Player : Observer
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float hp = 100f;
    [SerializeField] private float fireRate = 0.3f;
    [SerializeField] private float experience = 0f;
    [SerializeField] private int level = 1;
    [SerializeField] private int killScore = 0;

    private float baseExperiencePerKill = 5f;
    private float experienceGainRate = 1f;
    private float timeSinceStart = 0f;
    private bool isGuided = false;

    public TMP_Text killScoreText;

    public GameObject projectilePrefab;
    public Transform firePoint;
    private Transform closestEnemy;
    private Coroutine autoFireCoroutine = null;

    private void Awake()
    {
        PacketManager.Instance.AddObserver(this);
    }

    void Update()
    {
        Move();
        Fire();

        AutoMode();

        timeSinceStart += Time.deltaTime;

        killScoreText.text = "Kills: " + killScore;

        closestEnemy = SeekClosestEnemy();

    }

    private Transform SeekClosestEnemy()
    {
        List<GameObject> list = PacketManager.Instance.Enemies;

        Transform closestEnemy = null;

        float closestDistance = float.MaxValue;

        foreach (GameObject go in list)
        {

            float curruntDistance = Vector2.Distance(transform.position, go.transform.position);

            if (curruntDistance < closestDistance)
            {
                closestDistance = curruntDistance;
                closestEnemy = go.transform;
            }

        }
        return closestEnemy;
    }
    private void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector2 dir = new Vector2(x, y).normalized;
        transform.Translate(dir * moveSpeed * Time.deltaTime);
    }

    public void Fire()
    {
        if (Input.GetButtonDown("Fire1") && !isGuided)
        {
            FireProjectileTowardsMouse();
        }
    }
    private void AutoMode()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            isGuided = !isGuided;
            

            if (isGuided && autoFireCoroutine == null)
            {
                Debug.Log($"is Guided : {isGuided}\nauto fire coroutine is null : {autoFireCoroutine == null}");
                autoFireCoroutine = StartCoroutine(AutoFire());
            }
            else if (!isGuided && autoFireCoroutine != null)
            {
                StopCoroutine(autoFireCoroutine);
                autoFireCoroutine = null;
            }
        }
    }

    private void FireProjectileTowardsMouse()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectile.transform.up = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position;

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.SetDamage(damage);
    }

    private IEnumerator AutoFire()
    {
        while (true)
        {
            yield return new WaitUntil(() => closestEnemy != null);
            print($"close enemy is not null : {closestEnemy != null}");
            yield return new WaitForSeconds(fireRate);

            Debug.Log("Fire");
            FireProjectileAtClosestEnemy();
        }
    }

    private void FireProjectileAtClosestEnemy()
    {
        if (closestEnemy == null) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectile.transform.up = closestEnemy.position - transform.position;

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.SetDamage(damage);
    }

    private void GainExperience(float exp)
    {
        experience += exp;

        // Check for level-up
        float experienceRequired = level * 10;
        if (experience >= experienceRequired)
        {
            LevelUp();
            experience -= experienceRequired;
        }
    }

    private void LevelUp()
    {
        level++;
        IncreaseDamage(3f);
        print("Level Up! New Level: " + level + ", New Damage: " + damage);
    }

    private void IncreaseDamage(float dmg)
    {
        damage += dmg;
    }

    public void TakeDamage(float damageAmount)
    {
        hp -= damageAmount;

        if (hp <= 0)
        {
            print("�÷��̾� ����!");
        }
    }

    public override void OnNotify(IEventMessage message)
    {
        print(message is ClosestEnemyMessage);

        if (message is PlayerMessage playerMessage && playerMessage.Target == gameObject)
        {
            TakeDamage(playerMessage.Damage);
        }
        else if (message is ClosestEnemyMessage closestEnemyMessage)
        {
            closestEnemy = closestEnemyMessage.Sender.transform;
        }
        else if (message is EnemyKilledMessage)
        {
            GainExperience(Mathf.Round(baseExperiencePerKill + (timeSinceStart / 5f) * experienceGainRate * 10) / 10f);
            killScore++;
        }
    }
}
