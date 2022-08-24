using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeGame : MonoBehaviour
{
    public static readonly int WindowWidth = 700;
    public static readonly int WindowSize = WindowWidth / 2;
    
    void Awake()
    {
        Bezier.Window bezierWindow =
            new Bezier.Window(0.0f, 100.0f, 100.0f, -100.0f,
                -WindowSize, -WindowSize, WindowWidth, WindowWidth);

        Factory factory = GameObject.Find("Factory").GetComponent<Factory>();
        factory.Init(bezierWindow);

        GameObject camera = factory.CreateCamera(WindowWidth);
        GameObject background = factory.CreateBackground();
        GameObject boss = factory.CreateBoss();
        GameObject player = factory.CreatePlayer();

        Boss bossBoss = boss.GetComponent<Boss>();
        bossBoss.Init(camera, player);

        Player playerPlayer = player.GetComponent<Player>();
        playerPlayer.Init(camera, boss);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}
