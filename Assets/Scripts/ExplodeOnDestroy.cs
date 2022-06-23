using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeOnDestroy : MonoBehaviour
{
    public ParticleSystem explosionParticle;

    [SerializeField]
    float explosionPower = 3000f;

    [SerializeField]
    float explosionRadius = 5f;

    private DamagePlayer damageScript;

    // Start is called before the first frame update
    void Start()
    {
        damageScript = GetComponent<DamagePlayer>();
    }

    public void OnExplode()
    {
        Instantiate(explosionParticle, transform.position, Quaternion.identity);
        ExplodeForce();
    }

    void ExplodeForce()
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
        foreach (Collider hit in colliders)
        {
            // Apply damage if player
            if (hit.gameObject.CompareTag("Player"))
            {
                if (damageScript != null && damageScript.damageOnExplode)
                {
                    damageScript.OnApplyDamage(hit.gameObject);
                }
            }
            else
            {
                // Apply explosion force to nearby 'enemies'
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(explosionPower, explosionPos, explosionRadius);
                }
            }
        }
    }
}
