using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFromSide : MonoBehaviour
{
    public MoveDirection movingDirection = MoveDirection.Right;
    public bool isFixedRotation = false;

    [SerializeField]
    float speed = 25.0f;

    bool isStartMoving = false;

    Rigidbody rb;
    Transform modelTransform;
    Vector3 originalModelScale;

    readonly Vector3 vector3Right = Vector3.right;

    private void Awake()
    {
        modelTransform = transform.GetChild(0);
        originalModelScale = modelTransform.localScale;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        MovePhysicsBody();
    }

    // Fire signal to start moving object
    public void StartMoving()
    {
        UpdateScale();
        isStartMoving = true;
    }

    // Move gameobject through its physics body instead of transformation
    void MovePhysicsBody()
    {
        if (isStartMoving)
        {
            rb.MovePosition(transform.position + vector3Right * (int)movingDirection * speed * Time.deltaTime);
        }
    }

    void UpdateScale()
    {
        modelTransform.localScale = (int)movingDirection * originalModelScale;
    }
}
