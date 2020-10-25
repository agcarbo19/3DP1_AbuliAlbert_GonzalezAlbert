using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoItem : Item
{
    public int m_AddAmmoRounds;

    public override void Pick()
    {
        if (m_GameController.m_Player.GetAmmo() < m_GameController.m_Player.m_MaxAmmo)
        {
            int l_nAmmo = m_GameController.m_Player.m_MaxAmmo - m_GameController.m_Player.GetAmmo();
            if (l_nAmmo > m_AddAmmoRounds)
            {
                m_GameController.m_Player.AddAmmo(m_AddAmmoRounds);
            }
            else
            {
                m_GameController.m_Player.AddAmmo(l_nAmmo);
            }

            Destroy();
        }
    }
}

