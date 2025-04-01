using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mostrar_panel_codigo: MonoBehaviour
{
    [SerializeField] private GameObject Panel_de_Codigo;
    public int Numero_Necesario_para_Abrir = 1994;
    void Awake()
    {
        Panel_de_Codigo.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Panel_de_Codigo.SetActive(true);

            // ?? Obtener el script del panel y pasarle este cubo
            Panel_Number_Script panelScript = Panel_de_Codigo.GetComponent<Panel_Number_Script>();
            if (panelScript != null)
            {
                panelScript.SetCubo(this.gameObject); // le pasamos el cubo
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Panel_de_Codigo.SetActive(false);
        }
    }
}

