using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldItem : Item
{
    public int m_ShieldPoints;
    public override void Pick()
    {
        Debug.Log("Picked Shield item!");
        m_GameController.m_Player.AddShield(m_ShieldPoints);
        Destroy();
    }
}

