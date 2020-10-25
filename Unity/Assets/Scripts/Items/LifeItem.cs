using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeItem : Item
{
    public int m_LifePoints;

    public override void Pick()
    {
        if (m_GameController.m_Player.GetLife() < m_GameController.m_Player.m_MaxLife)
        {
            m_GameController.m_Player.m_ItemSound.Play();
            int l_nLife = m_GameController.m_Player.m_MaxLife - m_GameController.m_Player.GetLife();
            if (l_nLife > m_LifePoints)
            {
                m_GameController.m_Player.AddLife(m_LifePoints);
            }
            else
            {
                m_GameController.m_Player.AddLife(l_nLife);
            }
            Destroy();
        }
    }
}
