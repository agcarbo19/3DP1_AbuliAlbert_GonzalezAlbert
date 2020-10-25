using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldItem : Item
{
    public int m_ShieldPoints;
    public override void Pick()
    {
        if (m_GameController.m_Player.GetShield() < m_GameController.m_Player.m_MaxShield)
        {
            int l_nShield = m_GameController.m_Player.m_MaxShield - m_GameController.m_Player.GetShield();
            if (l_nShield > m_ShieldPoints)
            {
                m_GameController.m_Player.AddShield(m_ShieldPoints);
            }
            else
            {
                m_GameController.m_Player.AddShield(l_nShield);
            }
            Destroy();
        }
    }
}

