using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEventTrigger : MonoBehaviour
{
    public GameObject m_uiObject;
    public bool m_isActive;

    void Start()
    {
        m_uiObject.SetActive(false);
        m_isActive = true;
    }

    private void OnTriggerEnter(Collider m_player)
    {
        if (m_player.gameObject.tag == "Player")
        {
            m_uiObject.SetActive(true);
            StartCoroutine("WaitForSec");
            Debug.Log("Waitforsec");

            new WaitForSeconds(5);
        }
    }

    IEnumerator WaitForSec()
    {
        yield return new WaitForSeconds(5);
        m_uiObject.SetActive(false);
        m_isActive = false;
    }
}