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
        m_Enemies.Add(FindObjectOfType<DroneEnemy>());
    }
    private void Update()
    {
        m_TextAmmo.text = m_Weapon.GetBullets().ToString();
        m_TextLife.text = m_Player.GetLife().ToString();
        m_TextShield.text = m_Player.GetShield().ToString();
    }

    public IEnumerator RestartGame(Transform RespawnPoint)
    {
        m_Player.transform.position = RespawnPoint.position;
        yield return new WaitForSeconds(.5f);
        m_Player.RePatchPlayer();
        //for(int i = 0; i < m_Enemies.Count; i++)
        //{
        //    m_Enemies[i].Respawn();
        //}
    }
}
