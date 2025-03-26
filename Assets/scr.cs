using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.PlayMusic(AudioManager.Instance.GeneralMusic);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
