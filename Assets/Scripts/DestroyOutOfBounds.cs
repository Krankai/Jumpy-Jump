using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOutOfBounds : MonoBehaviour
{
    [SerializeField]
    float offsetX = 10f;

    [SerializeField]
    float minRangeY = -10f;

    float rangeX;
    PlayerController playerControllerScript;

    // Start is called before the first frame update
    void Start()
    {
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        rangeX = playerControllerScript.RangeX + offsetX;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < -rangeX || transform.position.x > rangeX)
        {
            gameObject.SetActive(false);
        }
        else if (transform.position.y < minRangeY)
        {
            gameObject.SetActive(false);
        }
    }
}
