using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Safe : MonoBehaviour
{
    [SerializeField] GameFlags flag;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] GameObject trigger;
    [SerializeField] GameObject numpad;
    private bool abierto;
    
    // Start is called before the first frame update
    void Start()
    {
        abierto = false;   
    }

    // Update is called once per frame
    void Update()
    {
        if (!abierto && GameEventManager.Instance.HasFlag(flag))
        {
            Abrir();
        }
    }

    private void Abrir()
    {
        abierto = true;
        spriteRenderer.enabled = false;
        trigger.SetActive(true);
        numpad.SetActive(false);
    }
}
