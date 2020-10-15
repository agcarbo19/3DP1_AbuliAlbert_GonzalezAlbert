using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoItem : Item
{
    public int m_AmmoRounds;
    public override void Pick()
    {
        Debug.Log("Picked Ammo item!");
        m_GameController.m_Player.AddAmmo(m_AmmoRounds);
        Destroy();
    }
}

