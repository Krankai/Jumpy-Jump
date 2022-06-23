using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnContact : MonoBehaviour
{
    [SerializeField]
    private string contactObjectTag = string.Empty;

    [SerializeField]
    private ContactType contactType = ContactType.Collision;

    [SerializeField]
    private bool haveDelay = false;

    [SerializeField]
    private float delayTime = 3.0f;

    private bool isTriggered = false;
    private ExplodeOnDestroy explosionScript;

    // Start is called before the first frame update
    void Start()
    {
        explosionScript = GetComponent<ExplodeOnDestroy>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isTriggered && contactType == ContactType.Collision && collision.gameObject.CompareTag(contactObjectTag))
        {
            StartCoroutine(DestroyRoutine());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered && contactType == ContactType.Trigger && other.gameObject.CompareTag(contactObjectTag))
        {
            StartCoroutine(DestroyRoutine());
        }
    }

    void CheckExplosion()
    {
        if (explosionScript != null)
        {
            explosionScript.OnExplode();
        }
    }

    IEnumerator DestroyRoutine()
    {
        // Set the flag
        isTriggered = true;

        // Set delay time if required
        if (haveDelay && delayTime > 0)
        {
            yield return new WaitForSeconds(delayTime);
        }

        // Explosion particle effect
        CheckExplosion();

        // Recycle enemy game object
        gameObject.SetActive(false);

        // Clear the flag
        isTriggered = false;
    }
}

public enum ContactType
{
    Collision,
    Trigger,
}
