using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Observer
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float hp = 50f;
    [SerializeField] private float maxHp;
    [SerializeField] private float damage = 20f;

    [SerializeField] private Transform targetDirection;
    private Renderer myrenderer;


    public Image hpBar;
    public float hpAmount { get { return hp / maxHp; } }
    public Transform targetTransform { get { return targetDirection; } }

    public delegate void EnemyDestroyed();
    public event EnemyDestroyed OnDestroyed;

    private void Awake()
    {
        PacketManager.Instance.AddObserver(this);
        myrenderer = transform.Find("Renderer") . GetComponent<Renderer>();
    }
    private void Start()
    {
        SetTargetDirection(GameObject.Find("Player").transform);
        
        maxHp = hp;
        OnDestroyed += () => { PacketManager.Instance.RemoveObserver(this); };
    }

    private void Update()
    {
        Move();
        DisplayHPToUI();
        
    }

    private void DisplayHPToUI()
    {
        hpBar.fillAmount = hpAmount;
    }

    private void Move()
    {
        Vector2 moveDir = (targetDirection.position - transform.position).normalized;
        
        transform.Translate(moveDir * moveSpeed * Time.deltaTime);
        myrenderer.transform.up = -moveDir;
    }

    public void SetTargetDirection(Transform target)
    {
        targetDirection = target;
    }

    public void TakeDamage(float damageAmount)
    {
        hp -= damageAmount;

        if (hp <= 0)
        {
            OnDeath();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PacketManager.Instance.SendMessage(new PlayerMessage(gameObject, collision.gameObject, damage));
        }
    }

    public override void OnNotify(IEventMessage message)
    {
        if (message is PlayerMessage playerMessage && playerMessage.Target == gameObject)
        {
            TakeDamage(playerMessage.Damage);
        }
    }

    private void OnDeath()
    {
        EnemyKilledMessage killMessage = new EnemyKilledMessage(gameObject);

        PacketManager.Instance.SendMessage(killMessage);

        OnDestroyed?.Invoke();

        Destroy(gameObject);
    }
}
