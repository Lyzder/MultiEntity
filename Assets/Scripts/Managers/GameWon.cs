using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWon : MonoBehaviour
{
    [SerializeField] GameObject panel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        FinishTrigger.PlayerWon += ShowPanel;
    }

    private void OnDisable()
    {
        FinishTrigger.PlayerWon -= ShowPanel;
    }

    private void ShowPanel()
    {
        panel.SetActive(true);
    }

    public void HidePanel()
    {
        panel.SetActive(false);
    }
}
