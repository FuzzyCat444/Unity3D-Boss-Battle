using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public GameObject eye;
    public GameObject iris;

    int stage;
    bool canTakeDamage;
    IEnumerator stageCoroutine;
    
    public Factory factory;
    public GameObject cameraObject;
    public GameObject player;

    Health health;
    SpriteRenderer spriteRenderer;
    SpriteRenderer eyeSpriteRenderer;
    SpriteRenderer irisSpriteRenderer;

    void Awake()
    {
        stage = 0;
        canTakeDamage = false;
        stageCoroutine = null;
    }

    void Start()
    {
        health = GetComponent<Health>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        eyeSpriteRenderer = eye.GetComponent<SpriteRenderer>();
        irisSpriteRenderer = iris.GetComponent<SpriteRenderer>();

        StartOver();
    }

    public void StartOver()
    {
        StopAllCoroutines();
        stage = 0;
        canTakeDamage = false;
        stageCoroutine = null;
        transform.position = new Vector2(0.0f, InitializeGame.WindowWidth);
        health.health = health.maxHealth;
        factory.StopAudio(factory.music);
        iris.transform.localPosition = Vector2.zero;
        GameObject[] plasma = GameObject.FindGameObjectsWithTag("plasma");
        for (int i = 0; i < plasma.Length; i++)
        {
            Destroy(plasma[i]);
        }
        Vector3 camPos = cameraObject.transform.position;
        camPos.x = 0.0f;
        camPos.y = 0.0f;
        cameraObject.transform.position = camPos;
        StartCoroutine(Intro());
    }

    IEnumerator Intro()
    {
        yield return new WaitForSeconds(2.0f);
        factory.PlayAudio(factory.rumble);
        yield return factory.ShakeCoroutine(cameraObject, 5.0f, 4.0f);
        yield return Together.Do(this, 
            factory.ShakeCoroutine(cameraObject, 1.0f, 4.0f),
            factory.MoveToCoroutine(gameObject, Vector2.zero, 1.0f)
        );
        factory.PlayAudio(factory.crash);
        yield return factory.ShakeCoroutine(cameraObject, 1.0f, 15.0f, true);
        yield return new WaitForSeconds(1.5f);
        stage = 1;
        factory.PlayAudio(factory.music, true);
    }

    IEnumerator Stage1()
    {
        while (true)
        {
            yield return Stage1Attack1();
            yield return Stage1Attack2();
            yield return Stage1Attack3();
            yield return Stage1Attack4();
        }
    }
    
    IEnumerator Stage1Attack1()
    {
        yield return factory.MoveToCoroutine(gameObject, Vector2.zero, 
            1.0f, true);
        float angle = 45.0f;
        float angleIncr = 85.0f;
        int numFireAngles = 4;
        float firePeriod = 0.1f;
        float plasmaSpeed = 300.0f;
        float attackDuration = 10.0f;
        float t = 0.0f;
        float t2 = 0.0f;
        while (t < attackDuration) {
            while (t2 > firePeriod)
            {
                for (int i = 0; i < numFireAngles; i++)
                {
                    float fireAngle = (angle + i * 360.0f / numFireAngles) *
                        Mathf.Deg2Rad;
                    factory.CreateBossPlasma(gameObject.transform.position,
                        new Vector2(Mathf.Cos(fireAngle), Mathf.Sin(fireAngle)),
                        plasmaSpeed, 1);
                }
                t2 -= firePeriod;
            }

            angle += angleIncr * Time.deltaTime;
            t += Time.deltaTime;
            t2 += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator Stage1Attack2()
    {
        yield return factory.MoveToCoroutine(gameObject, 
            new Vector2(-300, 300), 1.0f, true);
        Vector2[] path = factory.bossStage1Attack2Path;
        float pathL = Bezier.LoopLength(path);
        float moveDuration = 5.0f;
        yield return Together.Do(this,
            InOrder.Do(this,
                factory.FollowPathCoroutine(
                    path, pathL - 1.0f, 0.0f, moveDuration, gameObject
                ),
                factory.FollowPathCoroutine(
                    path, 0.0f, pathL - 1.0f, moveDuration, gameObject
                )
            ),
            Stage1Attack2_1(2.0f * moveDuration)
        );
    }

    IEnumerator Stage1Attack2_1(float duration)
    {
        float t = 0.0f;
        float t2 = 0.0f;
        float firePeriod = 0.5f;
        while (t < duration)
        {
            if (t2 > firePeriod) {
                while (t2 > firePeriod)
                {
                    factory.CreateBossPlasma(transform.position, 
                        new Vector2(0.0f, 1.0f), 500.0f, 3);
                    factory.CreateBossPlasma(transform.position,
                        new Vector2(0.0f, -1.0f), 500.0f, 3);
                    t2 -= firePeriod;
                }
                firePeriod = Random.Range(0.3f, 1.0f);
            }
            
            t += Time.deltaTime;
            t2 += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator Stage1Attack3()
    {
        yield return factory.MoveToCoroutine(gameObject, 
            new Vector2(-300.0f, 300.0f), 1.0f, true);
        Vector2[] path = factory.bossStage1Attack3Path;
        float pathL = Bezier.LoopLength(path);
        float duration = 3.5f;
        for (int i = 0; i < 4; i++)
        {
            bool even = i % 4 < 2;
            yield return Together.Do(this,
                factory.FollowPathCoroutine(path, even ? 0.0f : pathL, 
                    even ? pathL : 0.0f, duration, gameObject),
                Stage1Attack3_1(duration * 0.8f)
            );
        }
    }

    IEnumerator Stage1Attack3_1(float duration)
    {
        float t = 0.0f;
        float t2 = 0.0f;
        float firePeriod = 0.05f;
        float plasmaSpeed = 1200.0f;
        while (t < duration)
        {
            Vector2 dir1 = -transform.position;
            Vector2 dir2 = Quaternion.AngleAxis(30.0f, Vector3.forward) * dir1;
            Vector2 dir3 = Quaternion.AngleAxis(-30.0f, Vector3.forward) * dir1;
            Vector2 dir4 = Quaternion.AngleAxis(60.0f, Vector3.forward) * dir1;
            Vector2 dir5 = Quaternion.AngleAxis(-60.0f, Vector3.forward) * dir1;
            while (t2 >= firePeriod)
            {
                factory.CreateBossPlasma(gameObject.transform.position,
                    dir1, plasmaSpeed, 1);
                factory.CreateBossPlasma(gameObject.transform.position,
                    dir2, plasmaSpeed, 1);
                factory.CreateBossPlasma(gameObject.transform.position,
                    dir3, plasmaSpeed, 1);
                factory.CreateBossPlasma(gameObject.transform.position,
                    dir4, plasmaSpeed, 1);
                factory.CreateBossPlasma(gameObject.transform.position,
                    dir5, plasmaSpeed, 1);
                t2 -= firePeriod;
            }
            t += Time.deltaTime;
            t2 += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator Stage1Attack4()
    {
        yield return factory.MoveToCoroutine(gameObject,
            new Vector2(-300, 300), 1.0f, true);
        Vector2[] path = factory.bossStage1Attack4Path;
        float pathL = Bezier.LoopLength(path);
        float moveDuration = 5.0f;
        yield return Together.Do(this,
            InOrder.Do(this,
                factory.FollowPathCoroutine(
                    path, 0.0f, pathL - 1.0f, moveDuration, gameObject
                ),
                factory.FollowPathCoroutine(
                    path, pathL - 1.0f, 0.0f, moveDuration, gameObject
                )
            ),
            Stage1Attack4_1(2.0f * moveDuration)
        );
    }

    IEnumerator Stage1Attack4_1(float duration)
    {
        float t = 0.0f;
        float t2 = 0.0f;
        float firePeriod = 0.5f;
        while (t < duration)
        {
            if (t2 > firePeriod)
            {
                while (t2 > firePeriod)
                {
                    factory.CreateBossPlasma(transform.position,
                        new Vector2(1.0f, 0.0f), 500.0f, 3);
                    factory.CreateBossPlasma(transform.position,
                        new Vector2(-1.0f, 0.0f), 500.0f, 3);
                    t2 -= firePeriod;
                }
                firePeriod = Random.Range(0.3f, 1.0f);
            }

            t += Time.deltaTime;
            t2 += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator Stage2()
    {
        while (true)
        {
            yield return Stage2Attack1();
            yield return Stage2Attack2();
            yield return Stage2Attack3();
            yield return Stage2Attack4();
        }
    }

    IEnumerator Stage2Attack1()
    {
        yield return factory.MoveToCoroutine(gameObject, Vector2.zero, 
            1.0f, true);
        Vector2[] path = factory.bossStage2Attack1Path;
        float pathL = Bezier.LoopLength(path);
        float duration = 5.0f;
        for (int i = 0; i < 2; i++)
        {
            yield return Together.Do(this,
                factory.FollowPathCoroutine(path, 9.0f, pathL + 9.0f,
                    duration, gameObject),
                Stage2Attack1_1(duration)
            );
        }
    }

    IEnumerator Stage2Attack1_1(float duration)
    {
        float firePeriod = 0.75f;
        int shots = 8;

        float t = 0.0f;
        float t2 = 0.0f;
        while (t < duration)
        {
            while (t2 >= firePeriod)
            {
                for (int i = 0; i < shots; i++)
                {
                    float angle = 360.0f / shots * i * Mathf.Deg2Rad;
                    factory.CreateBossPlasma(gameObject.transform.position,
                        new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)),
                        500.0f, 2);
                }
                t2 -= firePeriod;
            }

            t += Time.deltaTime;
            t2 += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator Stage2Attack2()
    {
        yield return factory.MoveToCoroutine(gameObject, 
            new Vector2(-300.0f, 300.0f), 1.0f, true);
        Vector2[] path = factory.bossStage2Attack2Path;
        float pathL = Bezier.LoopLength(path);
        float duration = 15.0f;
        for (int i = 0; i < 2; i++)
        {
            yield return Together.Do(this,
                factory.FollowPathCoroutine(path, 0.0f, pathL, duration,
                    gameObject),
                Stage2Attack2_1(duration)
            );
        }
    }

    IEnumerator Stage2Attack2_1(float duration)
    {
        float t = 0.0f;
        float t2 = 0.0f;
        float firePeriod = 0.3f;
        while (t < duration)
        {
            while (t2 > firePeriod)
            {
                factory.CreateBossPlasma(transform.position,
                    player.transform.position - transform.position,
                    800.0f, 1);
                t2 -= firePeriod;
            }

            t += Time.deltaTime;
            t2 += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator Stage2Attack3()
    {
        float fireDuration = 3.0f;
        float firePeriod = 0.02f;
        for (int i = 0; i < 5; i++)
        {
            Vector2 randomPos = new Vector2(
                Random.Range(-300.0f, 300.0f),
                Random.Range(-300.0f, 300.0f));
            yield return factory.MoveToCoroutine(gameObject, randomPos, 0.75f);
            float t = 0.0f;
            float t2 = 0.0f;
            StartCoroutine(factory.ShakeCoroutine(gameObject, 
                fireDuration, 3.0f));
            while (t < fireDuration)
            {
                while (t2 >= firePeriod)
                {
                    float angle = Random.Range(0.0f, 360.0f) * 
                        Mathf.Deg2Rad;
                    Vector2 dir = new Vector2(
                        Mathf.Cos(angle), Mathf.Sin(angle));
                    factory.CreateBossPlasma(transform.position, dir,
                        450.0f, 2);
                    t2 -= firePeriod;
                }
                t += Time.deltaTime;
                t2 += Time.deltaTime;
                yield return null;
            }
        }
    }

    IEnumerator Stage2Attack4()
    {
        Vector2[] path1 = factory.bossStage2Attack4_1Path;
        float pathL1 = Bezier.LoopLength(path1);
        Vector2[] path2 = factory.bossStage2Attack4_2Path;
        float pathL2 = Bezier.LoopLength(path2);
        float duration = 10.0f;
        yield return InOrder.Do(this,
            factory.MoveToCoroutine(gameObject,
                new Vector2(-285.56f, -300.81f), 1.0f, true),
            Together.Do(this,
                factory.FollowPathCoroutine(path1, 0.0f, 5.0f * pathL1, 
                    duration, gameObject),
                Stage2Attack4_1(duration)
            ),
            factory.MoveToCoroutine(gameObject,
                new Vector2(285.56f, -300.81f), 1.0f, true),
            Together.Do(this,
                factory.FollowPathCoroutine(path2, 0.0f, 5.0f * pathL2, 
                    duration, gameObject),
                Stage2Attack4_1(duration)
            )
        );
    }

    IEnumerator Stage2Attack4_1(float duration)
    {
        float firePeriod = 0.2f;
        float plasmaSpeed = 1200.0f;
        int plasmaSize = 3;

        float t = 0.0f;
        float t2 = 0.0f;
        while (t < duration)
        {
            while (t2 >= firePeriod)
            {
                factory.CreateBossPlasma(gameObject.transform.position,
                    new Vector2(-1.0f, -1.0f),
                    plasmaSpeed, plasmaSize);
                factory.CreateBossPlasma(gameObject.transform.position,
                    new Vector2(1.0f, -1.0f),
                    plasmaSpeed, plasmaSize);
                factory.CreateBossPlasma(gameObject.transform.position,
                    new Vector2(1.0f, 1.0f),
                    plasmaSpeed, plasmaSize);
                factory.CreateBossPlasma(gameObject.transform.position,
                    new Vector2(-1.0f, 1.0f),
                    plasmaSpeed, plasmaSize);
                t2 -= firePeriod;
            }

            t += Time.deltaTime;
            t2 += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator Stage3()
    {
        while (true)
        {
            yield return Stage3Attack1();
            yield return Stage3Attack2();
            yield return Stage3Attack3();
            yield return Stage3Attack4();
        }
    }

    IEnumerator Stage3Attack1()
    {
        yield return factory.MoveToCoroutine(gameObject,
            new Vector2(-300.0f, -300.0f), 1.0f, true);
        float duration = 15.0f;
        float firePeriod = 1.5f;
        float gapWidth = 12.0f;
        int numAngles = 130;
        float plasmaSpeed = 150.0f;
        int plasmaSize = 1;
        float t = 0.0f;
        float t2 = firePeriod;
        while (t < duration)
        {
            while (t2 >= firePeriod)
            {
                float angle = -30.0f;
                float angleInc = 150.0f / numAngles;
                float gapAngle = Random.Range(0.0f, 90.0f);
                for (int i = 0; i < numAngles; i++)
                {
                    if (angle < gapAngle - 0.5f * gapWidth ||
                        angle > gapAngle + 0.5f * gapWidth)
                    {
                        float rAngle = angle * Mathf.Deg2Rad;
                        factory.CreateBossPlasma(gameObject.transform.position,
                            new Vector2(Mathf.Cos(rAngle), Mathf.Sin(rAngle)),
                            plasmaSpeed, plasmaSize);
                    }
                    angle += angleInc;
                }
                t2 -= firePeriod;
            }
            t += Time.deltaTime;
            t2 += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(4.5f);
    }

    IEnumerator Stage3Attack2()
    {
        yield return factory.MoveToCoroutine(gameObject,
            new Vector2(-292.0f, -250.0f), 1.0f, true);
        float duration = 15.0f;
        Vector2[] path = factory.bossStage3Attack2Path;
        float pathL = Bezier.LoopLength(path);
        yield return Together.Do(this,
            factory.FollowPathCoroutine(path, 0.0f, 2.0f * pathL, duration, 
                gameObject),
            Stage3Attack2_1(duration)
        );
    }

    IEnumerator Stage3Attack2_1(float duration)
    {
        float firePeriod1 = 0.75f;
        float firePeriod2 = 0.01f;
        float fireDuration = 0.15f;
        float angle = 0.0f;
        float angleIncr = 2.0f;
        float plasmaSpeed = 500.0f;
        int plasmaSize = 1;
        float t = 0.0f;
        float t2 = 0.0f;
        float t3 = fireDuration;
        float t4 = 0.0f;
        while (t < duration)
        {
            while (t2 >= firePeriod1)
            {
                t3 = 0.0f;
                t4 = 0.0f;
                angle = Random.Range(0.0f, 360.0f);
                t2 -= firePeriod1;
            }
            if (t3 < fireDuration)
            {
                while (t4 >= firePeriod2)
                {
                    float rAngle = angle * Mathf.Deg2Rad;
                    Vector2 dir1 = new Vector2(
                        Mathf.Cos(rAngle), Mathf.Sin(rAngle));
                    Vector2 dir2 = -dir1;
                    factory.CreateBossPlasma(gameObject.transform.position,
                        dir1, plasmaSpeed, plasmaSize);
                    factory.CreateBossPlasma(gameObject.transform.position,
                        dir2, plasmaSpeed, plasmaSize);
                    angle += angleIncr;
                    t4 -= firePeriod2;
                }
            }
            t += Time.deltaTime;
            t2 += Time.deltaTime;
            t3 += Time.deltaTime;
            t4 += Time.deltaTime;
            yield return null;
        }
    }
    
    IEnumerator Stage3Attack3()
    {
        yield return factory.MoveToCoroutine(gameObject, Vector2.zero,
            1.0f, true);
        float angle = 45.0f;
        float angleIncr = -30.0f;
        int numFireAngles = 8;
        float firePeriod = 0.3f;
        float plasmaSpeed = 300.0f;
        float attackDuration = 10.0f;
        float t = 0.0f;
        float t2 = 0.0f;
        while (t < attackDuration)
        {
            while (t2 > firePeriod)
            {
                for (int i = 0; i < numFireAngles; i++)
                {
                    float fireAngle = (angle + i * 360.0f / numFireAngles) *
                        Mathf.Deg2Rad;
                    factory.CreateBossPlasma(gameObject.transform.position,
                        new Vector2(Mathf.Cos(fireAngle), Mathf.Sin(fireAngle)),
                        plasmaSpeed, 3);
                }
                t2 -= firePeriod;
            }

            angle += angleIncr * Time.deltaTime;
            t += Time.deltaTime;
            t2 += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(2.0f);
    }

    IEnumerator Stage3Attack4()
    {
        for (int i = 0; i < 20; i++)
        {
            Vector2 origPos = transform.position;
            Vector2 dir = player.transform.position - transform.position;
            Vector2 newPos = origPos + dir + dir.normalized * 200.0f;
            float duration = 1.1f;
            float t = 0.0f;
            float tInc = 1.0f / duration;
            while (t < 1.0f)
            {
                float a = 4.0f;
                float t2 = 3 * t * t - 2 * t * t * t;
                float t3 = a * t2 * t2 - (a - 1.0f) * t2;

                transform.position = (1.0f - t3) * origPos + t3 * newPos;

                t += Time.deltaTime * tInc;
                yield return null;
            }
            int numAngles = 4;
            float angle = Random.Range(0.0f, 360.0f);
            float angleInc = 360.0f / numAngles;
            for (int j = 0; j < numAngles; j++)
            {
                float rAngle = angle * Mathf.Deg2Rad;
                Vector2 fireDir = new Vector2(
                    Mathf.Cos(rAngle), Mathf.Sin(rAngle));


                factory.CreateBossPlasma(gameObject.transform.position,
                    fireDir, 100.0f, 2);

                angle += angleInc;
            }
        }
        yield return new WaitForSeconds(4.0f);
    }

    IEnumerator Win()
    {
        factory.PlayAudio(factory.crash);
        health.Deactivate();
        float duration = 6.0f;
        yield return Together.Do(this,
            Bleed(duration, 0.001f, true),
            Fade(duration)
        );
        gameObject.SetActive(false);
    }

    IEnumerator Fade(float duration)
    {
        float t = 0.0f;
        while (t < duration)
        {
            float fade = 1.0f - t / duration;
            Color color = spriteRenderer.color;
            color.a = fade;
            spriteRenderer.color = color;
            eyeSpriteRenderer.color = color;
            irisSpriteRenderer.color = color;

            factory.music.volume = fade;

            t += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator Bleed(float duration, float interval, bool taper = false)
    {
        StartCoroutine(
            factory.ShakeCoroutine(gameObject, duration, 3.0f, false));
        float t = 0.0f;
        float t2 = 0.0f;
        float splatterInterval = interval;
        while (t < duration)
        {
            while (t2 >= splatterInterval)
            {
                float angle = Random.Range(0.0f, 360.0f);
                float rAngle = angle * Mathf.Deg2Rad;
                Vector2 dir = new Vector2(
                    Mathf.Cos(rAngle), Mathf.Sin(rAngle));
                dir *= Random.Range(0.0f, 80.0f);
                factory.CreateBlood(gameObject, 
                    (Vector2)transform.position + dir);
                t2 -= splatterInterval;
            }
            if (taper)
                splatterInterval *= Mathf.Pow(3.0f, Time.deltaTime);
            t += Time.deltaTime;
            t2 += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator NextStage(bool died)
    {
        if (died)
            yield return Bleed(3.0f, 0.01f);
        canTakeDamage = true;
        yield return stageCoroutine;
    }


    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<PlayerPlasma>() != null)
        {
            Destroy(collider.gameObject);
            Health health = GetComponent<Health>();
            if (canTakeDamage)
            {
                health.health -= 2.0f;

                Vector2 dir = collider.gameObject.transform.position - 
                    transform.position;
                dir.Normalize();
                dir *= 64.0f;
                factory.CreateBlood(gameObject, 
                    (Vector2)transform.position + dir);
            }
        }
    }

    void Update()
    {
        if (stage > 0)
        {
            Vector2 eyeDir = player.transform.position - transform.position;
            eyeDir.Normalize();
            eyeDir *= 6.0f;
            iris.transform.localPosition = eyeDir;

            bool died = false;

            if (health.health == 0.0f)
            {
                StopAllCoroutines();
                stage++;
                stageCoroutine = null;
                Vector3 camPos = cameraObject.transform.position;
                camPos.x = 0.0f;
                camPos.y = 0.0f;
                cameraObject.transform.position = camPos;
                died = true;
                canTakeDamage = false;
            }

            if (stageCoroutine == null)
            {
                switch (stage)
                {
                    case 1:
                        stageCoroutine = Stage1();
                        break;
                    case 2:
                        stageCoroutine = Stage2();
                        break;
                    case 3:
                        stageCoroutine = Stage3();
                        break;
                    case 4:
                        stageCoroutine = Win();
                        break;
                }
                if (stageCoroutine != null)
                {
                    health.health = health.maxHealth;
                    StartCoroutine(NextStage(died));
                }
            }
        }
    }

    public void Init(GameObject camera, GameObject player)
    {
        this.cameraObject = camera;
        this.player = player;
    }
}
