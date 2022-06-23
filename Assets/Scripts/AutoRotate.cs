using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    [SerializeField]
    float speed = 400f;

    readonly Vector3 vector3Right = Vector3.right;

    // Update is called once per frame
    void Update()
    {
        SelfRotate();
    }

    void SelfRotate()
    {
        transform.Rotate(vector3Right * speed * Time.deltaTime);
    }

}
