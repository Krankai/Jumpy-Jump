using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowFromSide : MonoBehaviour
{
    public MoveDirection throwingDirection = MoveDirection.Right;

    [SerializeField]
    float minThrowingForce = 100f;

    [SerializeField]
    float maxThrowingForce = 200f;

    Rigidbody rb;
    Vector3 vector3Right = Vector3.right;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Fire signal to start throwing object
    public void StartThrowing()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        rb.AddForce(vector3Right * (int)throwingDirection * RandomizeForce(), ForceMode.Impulse);
    }

    // Randomize throwing force
    float RandomizeForce()
    {
        return Random.Range(minThrowingForce, maxThrowingForce);
    }
}
