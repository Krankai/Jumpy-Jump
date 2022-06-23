using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    public bool damageOnExplode = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnApplyDamage(collision.gameObject);
        }
    }

    public void OnApplyDamage(GameObject playerTarget)
    {
        playerTarget.GetComponent<PlayerController>().OnHit();
    }
}
