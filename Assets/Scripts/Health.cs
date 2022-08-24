using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 10.0f;
    public float health = 10.0f;
    public Vector2 offset = Vector2.zero;

    GameObject healthBar;
    GameObject healthBarFill;

    // Start is called before the first frame update
    void Awake()
    {
        healthBar = new GameObject();
        healthBar.name = "Health Bar";
        healthBar.transform.parent = gameObject.transform;
        healthBar.AddComponent<SpriteRenderer>();
        SpriteRenderer healthBarRenderer = 
            healthBar.GetComponent<SpriteRenderer>();
        healthBarRenderer.sprite = 
            Resources.Load<Sprite>("Sprites/UI/HealthBar");
        healthBarRenderer.sortingLayerName = "UI";
        healthBarRenderer.drawMode = SpriteDrawMode.Sliced;
        healthBarRenderer.sortingOrder = 0;

        healthBarFill = new GameObject();
        healthBarFill.name = "Health Bar Fill";
        healthBarFill.transform.parent = gameObject.transform;
        healthBarFill.AddComponent<SpriteRenderer>();
        SpriteRenderer healthBarFillRenderer = 
            healthBarFill.GetComponent<SpriteRenderer>();
        healthBarFillRenderer.sprite = 
            Resources.Load<Sprite>("Sprites/UI/HealthBarFill");
        healthBarFillRenderer.sortingLayerName = "UI";
        healthBarFillRenderer.drawMode = SpriteDrawMode.Sliced;
        healthBarFillRenderer.sortingOrder = 1;
    }

    public void Activate()
    {
        healthBar.SetActive(true);
        healthBarFill.SetActive(true);
    }

    public void Deactivate()
    {
        healthBar.SetActive(false);
        healthBarFill.SetActive(false);
    }

    void OnDestroy()
    {
        Destroy(healthBar);
        Destroy(healthBarFill);
    }

    // Update is called once per frame
    void Update()
    {
        if (health < 0)
            health = 0;

        int healthBarWidth = (int) (Mathf.Log(maxHealth) * 50.0f);
        int healthBarHeight = 10;
        Vector2Int healthBarSize = 
            new Vector2Int(healthBarWidth, healthBarHeight);
        
        SpriteRenderer healthBarRenderer =
            healthBar.GetComponent<SpriteRenderer>();
        Sprite healthBarSprite = healthBarRenderer.sprite;
        healthBarRenderer.size = healthBarSize;
        healthBar.transform.localPosition = offset - healthBarSize / 2;

        SpriteRenderer healthBarFillRenderer =
            healthBarFill.GetComponent<SpriteRenderer>();
        Vector2Int healthBarFillSize = new Vector2Int(
            (int) (healthBarWidth - healthBarSprite.border.x -
            healthBarSprite.border.z),
            (int) (healthBarHeight - healthBarSprite.border.y -
            healthBarSprite.border.w)
        );
        healthBarFill.transform.localPosition = offset - healthBarFillSize / 2;
        healthBarFillSize.x = (int)(healthBarFillSize.x * health / maxHealth);
        healthBarFillRenderer.size = healthBarFillSize;
    }
}
