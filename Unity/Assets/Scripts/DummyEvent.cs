using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DummyEvent : MonoBehaviour
{
    public UIEventTrigger m_eventTrigger;
    public FPSController m_player;
    public GameObject m_dummies;
    public GameObject m_uiNextLevel;

    public float m_score;
    public float m_eventTime;
    public float m_eventTimeRemaining;
    public bool m_isCountingDown;

    void Start()
    {
        m_dummies.SetActive(false);
        m_uiNextLevel.SetActive(false);
        m_eventTime = 30f;
        m_eventTimeRemaining = m_eventTime;
    }

    void Update()
    {
        if (m_eventTrigger.m_isActive == false)
        {
            m_dummies.SetActive(true);
            StartCoroutine("DummyChallenge");

            m_eventTimeRemaining -= 1 * Time.deltaTime;

            m_player.m_TextScore.text = "Score: " + m_score;
            m_player.m_TextDummyTime.text = "Time left: " + m_eventTimeRemaining;

            if (m_score >= 1000)
            {
                m_uiNextLevel.SetActive(true);

                if (m_eventTimeRemaining <= 0)
                {
                    SceneManager.LoadScene(1);
                }
            }

        }
    }

    IEnumerator DummyChallenge()
    {
        yield return new WaitForSeconds(30);
        m_dummies.SetActive(false);
    }
}