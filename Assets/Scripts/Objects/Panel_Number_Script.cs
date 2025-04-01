using TMPro;
using UnityEngine;

public class Panel_Number_Script : MonoBehaviour
{
    public TextMeshProUGUI inputDisplay;
    private string currentInput = "";
    private Mostrar_panel_codigo scriptDelCubo;
    private GameObject cuboActual;




 
    public void AddDigit(string digit)
    {
        currentInput += digit;
        inputDisplay.text = currentInput;
    }

    public void ClearInput()
    {
        currentInput = "";
        inputDisplay.text = "";
    }
    public void SetCubo(GameObject cubo)
    {
        cuboActual = cubo;
        scriptDelCubo = cubo.GetComponent<Mostrar_panel_codigo>();
    }
    public void SubmitInput()
    {
        Debug.Log("Número ingresado: " + currentInput);

        if (int.TryParse(currentInput, out int numeroIngresado))
        {
            if (numeroIngresado == scriptDelCubo.Numero_Necesario_para_Abrir)
            {
                Debug.Log("¡Código correcto! Puerta desbloqueada.");
                cuboActual.SetActive(false); // Desactivar el cubo correcto
                this.gameObject.SetActive(false); // Oculta el panel
            }
            else
            {
                Debug.Log("Código incorrecto.");
            }
        }
        else
        {
            Debug.Log("Entrada inválida.");
        }

        ClearInput();
    }
}
