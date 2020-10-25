using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyTarget : MonoBehaviour
{
    public DummyEvent m_event;

    float m_respawnTime = 10.0f;
    bool m_isRoutine = false;

    public bool m_isHit = false;

    void Update()
    {
        if (m_isHit == true)
        {
            if (m_isRoutine == false)
            {
                gameObject.GetComponent<Animation>().Play("dummy_down");
                m_event.m_score += 150;

                StartCoroutine(RespawnTime());
                m_isRoutine = true;
            }
        }
    }

    private IEnumerator RespawnTime()
    {
        yield return new WaitForSeconds(m_respawnTime);

        gameObject.GetComponent<Animation>().Play("dummy_up");

        m_isHit = false;
        m_isRoutine = false;
    }
}