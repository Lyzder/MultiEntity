using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PersonaTrigger : MonoBehaviour
{
    public enum PersonaId
    {
        Base = 0,
        Dep = 1,
        Int = 2
    }

    [SerializeField] private PersonaId personaId;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ChangePersona(PlayerController player)
    {
        int id = (int)personaId;
        if (player.personaActiva != id)
        {
            player.ChangePersona(id);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ChangePersona(other.GetComponent<PlayerController>());
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 2f);
    }
}
