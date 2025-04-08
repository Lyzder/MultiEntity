using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NumberpadObject : InteractableBase
{
    [SerializeField] GameObject panel;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameFlags flag;
    [SerializeField] Collider trigger;
    public string code;
    public int codeLength;
    public AudioClip tecla;
    private string inputCode;
    private PlayerController player;

    // Start is called before the first frame update
    void Awake()
    {
        player = null;
        inputCode =  string.Empty;
        text.text = string.Empty;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact(PlayerController player)
    {
        OpenPanel();
        this.player = player;
        player.StartReading(this);
    }

    public void NumberInput(string input)
    {
        AudioManager.Instance.PlaySFX(tecla);
        if (inputCode.Length < codeLength)
        {
            inputCode += input;
            UpdateText();
        }
    }

    public void DeleteInput()
    {
        AudioManager.Instance.PlaySFX(tecla);
        if (inputCode.Length > 0)
        {
            inputCode = inputCode.Remove(inputCode.Length - 1);
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
        AudioManager.Instance.PlaySFX(tecla);
        if (code == inputCode)
        {
            //TODO
            GameEventManager.Instance.SetFlag(flag);
            player.StopReading();
            player.ObjectOutOfRange(this);
            DeactivateTrigger();
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

    private void DeactivateTrigger()
    {
        trigger.enabled = false;
    }
}
