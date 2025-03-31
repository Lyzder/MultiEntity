using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Mostrar_mensaje_notas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI mensaje;
    public GameObject Canva_del_mensaje;
    private void Start()
    {
        if (mensaje != null)
        {
            mensaje.gameObject.SetActive(false); // Ocultamos el mensaje al inicio
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && mensaje != null)
        {   //Canva_del_mensaje.SetActive(true);
            mensaje.gameObject.SetActive(true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && mensaje != null)
        {
            mensaje.gameObject.SetActive(false);
           // Canva_del_mensaje.SetActive(false);
        }
    }
}
