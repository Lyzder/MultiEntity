using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class Hitbox : MonoBehaviour
{
    public GameObject owner;
    public AudioClip hitSfx;
    private List<IDamageable> ignoreObjects;

    private void Awake()
    {
        ignoreObjects = new List<IDamageable>();
    }

    private void OnEnable()
    {
        ignoreObjects.Clear();
    }

    private void OnDisable()
    {
        ignoreObjects.Clear();
    }

    private void OnTriggerStay(Collider other)
    {
        // Avoid hitting the owner (player) of the hitbox
        if (other.gameObject == owner) return;

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && !damageable.IsInvulnerable() && !ignoreObjects.Contains(damageable))
        {
            damageable.TakeDamage();
            AudioManager.Instance.PlaySFX(hitSfx);
            ignoreObjects.Add(damageable);
            Debug.Log("Hit: " + other.name);
        }
    }
}
