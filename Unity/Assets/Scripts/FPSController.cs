using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    #region Parameters
    private float m_Yaw;
    private float m_Pitch;
    public CharacterController m_CharacterController;
    public GameController m_GameController;

    [Header("Camera")]
    public Camera m_Camera;
    public Transform m_PitchController;
    public float m_MinPitch = -35.0f;
    public float m_MaxPitch = 105.0f;
    public float m_YawRotationalSpeed = 1.0f;
    public float m_PitchRotationalSpeed = 1.0f;

    [Header("Velocidades")]
    public float m_Speed = 2.0f;
    public float m_VerticalSpeed;
    public float m_JumpSpeed = 10.0f;
    public float m_RunSpeedMultiplaier = 1.5f;

    [Header("HUD")]
    public int m_Life = 100;
    public int m_Ammo = 100;
    public int m_Shield = 100;

    [Header("Bools")]
    public bool m_InvertVerticalAxis = true;
    public bool m_InvertHorizontalAxis = false;
    public bool m_OnGround = false;
    public bool m_AngleLocked = false;
    public bool m_AimLocked = true;
    public bool m_IsMoving = false;

    [Header("Input")]
    public KeyCode m_LeftKeyCode;
    public KeyCode m_RightKeyCode;
    public KeyCode m_UpKeyCode;
    public KeyCode m_DownKeyCode;
    public KeyCode m_JumpKey;
    public KeyCode m_RunKeyCode = KeyCode.LeftShift;
    public KeyCode m_DebugLockAngleKeyCode = KeyCode.I;
    public KeyCode m_DebugLockKeyCode = KeyCode.O;

    [Header("Jumping")]
    public bool isJumping;
    public float jumpTime;
    public float jumpTimeCounter;
    public float m_TimeSinceLastGround = 0.0f;
    public float m_JumpThresholdSinceLastGround = 0.2f;
    public float m_FallMultiplier = 1.01f;

    #endregion

    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        m_Yaw = transform.rotation.eulerAngles.y;
        m_Pitch = m_PitchController.rotation.eulerAngles.x;
        m_VerticalSpeed = 0.0f;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        #region Camera y dirección
        //Actualizar el Yaw y el Pitch
        float l_MouseAxisX = Input.GetAxis("Mouse X");
        float l_MouseAxisY = Input.GetAxis("Mouse Y");

        #region Invertir controles camera
        if (m_InvertHorizontalAxis)
        {
            l_MouseAxisX = -l_MouseAxisX;
        }
        if (m_InvertVerticalAxis)
        {
            l_MouseAxisY = -l_MouseAxisY;
        }
        #endregion

        #region Clavar la camera si no queremos que se mueva
        if (!m_AngleLocked)
        {
            m_Yaw = m_Yaw + l_MouseAxisX * m_YawRotationalSpeed * Time.deltaTime;
            m_Pitch = m_Pitch + l_MouseAxisY * m_PitchRotationalSpeed * Time.deltaTime;
            m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);
        }
        #endregion

        transform.rotation = Quaternion.Euler(0.0f, m_Yaw, 0.0f);
        m_PitchController.localRotation = Quaternion.Euler(m_Pitch, 0.0f, 0.0f);

        #region Sacar el mouse con la "O" para tocar el editor
#if UNITY_EDITOR //If para sacar el mouse con la "O" para tocar el editor
        if (Input.GetKeyDown(m_DebugLockAngleKeyCode))
            m_AngleLocked = !m_AngleLocked;
        if (Input.GetKeyDown(m_DebugLockKeyCode))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
            m_AimLocked = Cursor.lockState == CursorLockMode.Locked;
        }
#endif
        #endregion

        #endregion

        #region Movement

        Vector3 l_Movement = Vector3.zero;

        //para yaw 0 forward (0,0,1)
        //para yaw 90 forward (1,0,0)
        //para yaw 180 forward (0,0,-1)
        //para yaw 270 forward (-1,0,0)

        Vector3 l_Forward = new Vector3(Mathf.Sin(m_Yaw * Mathf.Deg2Rad), 0.0f, Mathf.Cos(m_Yaw * Mathf.Deg2Rad));
        Vector3 l_Right = new Vector3(Mathf.Sin((m_Yaw + 90.0f) * Mathf.Deg2Rad), 0.0f, Mathf.Cos((m_Yaw + 90.0f) * Mathf.Deg2Rad));

        l_Forward.y = 0.0f;
        l_Movement.Normalize();
        l_Right.y = 0.0f;
        l_Movement.Normalize();

        #region IF Inputs WASD
        if (Input.GetKey(m_RightKeyCode))
        {
            l_Movement = l_Right;
        }
        if (Input.GetKey(m_LeftKeyCode))
        {
            l_Movement = -l_Right;
        }
        if (Input.GetKey(m_UpKeyCode))
        {
            l_Movement += l_Forward;
        }
        if (Input.GetKey(m_DownKeyCode))
        {
            l_Movement += -l_Forward;
        }
        #endregion

        #region Salto
        //if (Input.GetKey(m_JumpKey) && m_TimeSinceLastGround < m_JumpThresholdSinceLastGround)
        //{
        //    m_VerticalSpeed = m_JumpSpeed;
        //}
        if (Input.GetKeyDown(m_JumpKey) && m_OnGround == true)
        {
            isJumping = true;
            jumpTimeCounter = jumpTime;
            m_VerticalSpeed = m_JumpSpeed;
        }

        if (Input.GetKey(m_JumpKey) && isJumping == true)
        {
            if (jumpTimeCounter > 0)
            {
                m_VerticalSpeed = m_JumpSpeed;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetKeyUp(m_JumpKey))
        {
            isJumping = false;
        }

        if (isJumping == false && m_OnGround == false)
        {
            if (m_VerticalSpeed < 0.0f)
            {
                m_VerticalSpeed *= m_FallMultiplier;
            }
        }

        #endregion

        //Normalizamos para arreglar la velocidad de diagonal
        l_Movement.Normalize();

        #region Bool IsMoving
        if ((l_Movement.x != 0f || l_Movement.z != 0f) && m_OnGround == true)
        {
            m_IsMoving = true;
        }
        else
        {
            m_IsMoving = false;
        }
        #endregion

        #region Correr-Sprint      
        //Controlar si corremos o no

        float l_Speed = m_Speed;
        if (Input.GetKey(m_RunKeyCode))
        {
            l_Speed *= m_RunSpeedMultiplaier;
        }
        #endregion

        //Movimento horizontal y vertical
        //---> v=v0+a*dt
        m_VerticalSpeed = m_VerticalSpeed + Physics.gravity.y * Time.deltaTime;
        //---> x=x0+v*dt; para el plano XZ (m_Speed)
        l_Movement = l_Movement * l_Speed * Time.deltaTime;
        //---> x=x0+v*dt; para el plano Y (m_VerticalSpeed)
        l_Movement.y = m_VerticalSpeed * Time.deltaTime;
        #endregion

        #region Collision
        //CollisionFags es una mascara que detectara si colisionamos des de algun lado
        CollisionFlags l_CollisionFlogs = m_CharacterController.Move(l_Movement);
        m_OnGround = (l_CollisionFlogs & CollisionFlags.CollidedBelow) != 0;

        m_TimeSinceLastGround += Time.deltaTime;
        if (m_OnGround)
        {
            m_TimeSinceLastGround = 0.0f;
        }

        //Si topamos con algo VerticalSpeed = 0
        if (m_OnGround || ((l_CollisionFlogs & CollisionFlags.CollidedAbove) != 0 && m_VerticalSpeed > 0.0f))
        {
            m_VerticalSpeed = 0.0f;
        }
        if (m_TimeSinceLastGround < m_JumpThresholdSinceLastGround)
            m_OnGround = true;
        #endregion

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            other.GetComponent<Item>().Pick();
        }
    }

    #region Item Functions
    public void AddLife(int LifePoints)
    {
        m_Life += LifePoints;
    }

    public void AddAmmo(int AmmoRounds)
    {
        m_Ammo += AmmoRounds;
    }

    public void RemoveAmmo()
    {
        m_Ammo--;
    }

    public int GetAmmo()
    {
        return m_Ammo;
    }

    public void AddShield(int ShieldPoints)
    {
        m_Shield += ShieldPoints;
    }
    #endregion
}
