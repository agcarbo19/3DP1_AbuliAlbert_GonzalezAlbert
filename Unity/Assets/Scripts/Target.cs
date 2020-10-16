using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float m_Health = 50.0f;
    
    public void TakeDamage(float damage)
    {
        m_Health -= damage;

        if (m_Health <= 0f)
        {
            Dead();
        }

    }

    private void Dead()
    {
        Destroy(gameObject);
    }
}
