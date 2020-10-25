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
    public AudioSource m_Music;

    private void Start()
    {
        m_Enemies.Add(FindObjectOfType<DroneEnemy>());
    }

    public IEnumerator RestartGame(Transform RespawnPoint)
    {
        m_Player.transform.position = RespawnPoint.position;
        yield return new WaitForSeconds(.5f);
        m_Player.RePatchPlayer();
        m_Music.Play();
        //for(int i = 0; i < m_Enemies.Count; i++)
        //{
        //    m_Enemies[i].Respawn();
        //}
    }
}
