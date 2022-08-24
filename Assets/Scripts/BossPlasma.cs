using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPlasma : MonoBehaviour
{
    public int size;
    public Vector2 velocity;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition += (Vector3)(Time.deltaTime * velocity);
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Camera>() != null)
        {
            Destroy(gameObject);
        }
    }
}
