using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Vector2 velocity;
    Vector2 acceleration;

    float shootTimer;
    float damageTimer;
    float damageTimerMax;
    float damageTimer2;

    Health health;
    SpriteRenderer spriteRenderer;
    GameObject boss;
    GameObject cameraObject;
    public Factory factory;

    void Awake()
    {
        shootTimer = 0.0f;
        damageTimer = 0.0f;
        damageTimerMax = 0.1f;
    }

    void Start()
    {
        health = GetComponent<Health>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<Boss>() != null)
        {
            if (damageTimer == 0.0f)
            {
                health.health -= 0.5f;
                damageTimer = damageTimerMax;
                factory.PlayAudio(factory.hurt);
                StartCoroutine(
                    factory.ShakeCoroutine(cameraObject, 0.1f, 7.0f));
            }
        }
        BossPlasma plasma = collider.gameObject.GetComponent<BossPlasma>();
        if (plasma != null && damageTimer2 == 0.0f)
        {
            float damage = 0.05f;
            if (plasma.size == 2)
                damage *= 4.0f;
            else if (plasma.size == 3)
                damage *= 8.0f;
            health.health -= damage;
            damageTimer2 = damageTimerMax;
            factory.PlayAudio(factory.hurt);

            Vector3 camPos = cameraObject.transform.position;
            camPos.x = 0.0f;
            camPos.y = 0.0f;
            cameraObject.transform.position = camPos;
            StartCoroutine(
                    factory.ShakeCoroutine(cameraObject, 0.1f, 7.0f));
        }
    }

    void Update()
    {
        damageTimer -= Time.deltaTime;
        if (damageTimer < 0.0f)
            damageTimer = 0.0f; 
        damageTimer2 -= Time.deltaTime;
        if (damageTimer2 < 0.0f)
            damageTimer2 = 0.0f;
        Color color = spriteRenderer.color;
        float v = Mathf.Max(damageTimer, damageTimer2) / damageTimerMax;
        color.g = 1.0f - v;
        color.b = 1.0f - v;
        spriteRenderer.color = color;

        if (health.health == 0.0f)
        {
            health.health = health.maxHealth;
            transform.localPosition = new Vector3(0.0f, -250.0f, 0.0f);
            Boss bossBoss = boss.GetComponent<Boss>();
            bossBoss.StartOver();
        }

        acceleration.Set(0.0f, 0.0f);

        const float friction = 1050.0f;
        Vector2 frictionAccel = velocity;
        frictionAccel.Normalize();
        frictionAccel.Scale(new Vector2(-friction, -friction));
        if (friction * Time.deltaTime > velocity.magnitude)
        {
            velocity.Set(0.0f, 0.0f);
            frictionAccel.Set(0.0f, 0.0f);
        }
        acceleration += frictionAccel;
        
        const float move = 3500.0f;
        Vector2 moveAccel = new Vector2(
            move * Input.GetAxisRaw("Horizontal"),
            move * Input.GetAxisRaw("Vertical"));
        acceleration += moveAccel;
        

        velocity += Time.deltaTime * acceleration;
        transform.localPosition += (Vector3)(Time.deltaTime * velocity);

        Camera cameraCamera = cameraObject.GetComponent<Camera>();
        Vector2 bottomLeft = cameraCamera.ScreenToWorldPoint(
            new Vector2(0.0f, 0.0f));
        Vector2 topRight = cameraCamera.ScreenToWorldPoint(
            new Vector2(Screen.width, Screen.height));
        
        if (transform.position.x < bottomLeft.x)
        {
            transform.position = new Vector3(
                bottomLeft.x, transform.position.y);
            velocity.x = 0.0f;
        }
        else if (transform.position.x > topRight.x)
        {
            transform.position = new Vector3(
                topRight.x, transform.position.y);
            velocity.x = 0.0f;
        }
        if (transform.position.y < bottomLeft.y)
        {
            transform.position = new Vector3(
                transform.position.x, bottomLeft.y);
            velocity.y = 0.0f;
        }
        else if (transform.position.y > topRight.y)
        {
            transform.position = new Vector3(
                transform.position.x, topRight.y);
            velocity.y = 0.0f;
        }

        const float shootDelay = 0.1f;
        const float plasmaSpeed = 1050.0f;
        if (Input.GetButtonDown("Fire1"))
        {
            shootTimer = 0.0f;
        }
        if (Input.GetButton("Fire1"))
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0.0f)
            {
                shootTimer = shootDelay;
                Vector2 position = (Vector2)cameraCamera.ScreenToWorldPoint(
                    (Vector3)Input.mousePosition);
                Vector2 direction = position - (Vector2)transform.position;

                factory.CreatePlayerPlasma(transform.position, direction, 
                    plasmaSpeed);
                factory.PlayAudio(factory.fire);
            }
        }
    }

    public void Init(GameObject camera, GameObject boss)
    {
        this.cameraObject = camera;
        this.boss = boss;
    }
}
