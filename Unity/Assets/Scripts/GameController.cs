using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    public FPSController m_Player;
    public Transform m_DestroyObjects;
    public TextMeshProUGUI m_TextAmmo;

    private void Start()
    {

    }

    private void Update()
    {
        m_TextAmmo.text = m_Player.GetAmmo().ToString();
    }

}
