using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    protected GameController m_GameController;
    void Start()
    {
        m_GameController = GameObject.FindObjectOfType<GameController>();  
    }

    public virtual void Pick()
    {
        Debug.LogError("This method must not be called!");
    }

    protected void Destroy()
    {
        GameObject.Destroy(gameObject);
    }
}
