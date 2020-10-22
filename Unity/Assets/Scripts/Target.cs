using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float m_Health = 50.0f;
    public ParticleSystem m_DeadExposion;

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
        if (m_DeadExposion != null)
        {
            GameObject.Instantiate(m_DeadExposion, new Vector3(transform.position.x, 2f, transform.position.z), Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
