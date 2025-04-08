using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeAgroTrigger : MonoBehaviour
{
    [SerializeField] Boss owner;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            owner.StopAgro();
        }
    }
}
