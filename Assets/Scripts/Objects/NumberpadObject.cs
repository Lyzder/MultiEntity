using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NumberpadObject : InteractableBase
{
    [SerializeField] GameObject panel;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameFlags flag;
    public string code;
    public int codeLength;
    private string inputCode;
    private PlayerController player;

    // Start is called before the first frame update
    void Awake()
    {
        player = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact(PlayerController player)
    {
        OpenPanel();
        player.StartReading(this);
    }

    public void NumberInput(string input)
    {
        if (inputCode.Length < codeLength)
        {
            inputCode += input;
            UpdateText();
        }
    }

    public void DeleteInput()
    {
        if (inputCode.Length > 0)
        {
            inputCode.Remove(inputCode.Length - 1);
            UpdateText();
        }
    }

    public void ResetInput()
    {
        inputCode = string.Empty;
    }

    private void UpdateText()
    {
        text.text = inputCode;
    }

    public void CheckCode()
    {
        if (code == inputCode)
        {
            //TODO
            GameEventManager.Instance.SetFlag(flag);
        }
    }

    public override void OpenPanel()
    {
        ResetInput();
        panel.SetActive(true);
    }

    public override void ClosePanel()
    {
        panel.SetActive(false);
    }
}
