using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerNivel : MonoBehaviour
{
    [SerializeField] GameFlags levelFlag;
    public static event Action<GameFlags> AdvanceLevel;

    // Start is called before the first frame update
    void Start()
    {
        if (GameEventManager.Instance.HasFlag(levelFlag))
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameEventManager.Instance.HasFlag(levelFlag))
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameEventManager.Instance.SetFlag(levelFlag);
            AdvanceLevel?.Invoke(levelFlag);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameEventManager.Instance.SetFlag(levelFlag);
            AdvanceLevel?.Invoke(levelFlag);
        }
    }
}
