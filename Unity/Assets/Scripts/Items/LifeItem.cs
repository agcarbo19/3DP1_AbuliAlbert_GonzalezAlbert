using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeItem : Item
{
    public int m_LifePoints;
    public override void Pick()
    {
        Debug.Log("Picked life item!");
        m_GameController.m_Player.AddLife(m_LifePoints);
        Destroy();
    }
}
