using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    public FPSController m_Player;
    public WeaponController m_Weapon;
    public List<DroneEnemy> m_Enemies;
    public Transform m_DestroyObjects;
    public TextMeshProUGUI m_TextAmmo;
    public TextMeshProUGUI m_TextLife;
    public TextMeshProUGUI m_TextShield;

    private void Start()
    {
    }

    private void Update()
    {
        m_TextAmmo.text = m_Weapon.GetBullets().ToString();
        m_TextLife.text = m_Player.GetLife().ToString();
        m_TextShield.text = m_Player.GetShield().ToString();
    }

    //public void RestartGame()
    //{
    //    m_Player.Respawn(m_RespawnPoint);
    //    //m_Enemies.Respawn();
    //}
}
