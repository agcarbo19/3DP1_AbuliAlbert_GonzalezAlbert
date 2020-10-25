using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DroneEnemy : MonoBehaviour
{
    #region Parameters
    enum TState
    {
        IDLE,
        PATROL,
        ALERT,
        CHASE,
        ATTACK,
        HIT,
        DIE
    }
    public Transform m_Target;
    NavMeshAgent m_NavMeshAgent;
    TState m_State;
    private float m_CurrentTime = 0f;
    public float m_IdleTime = 3f;
    public float m_MaxDistanceToAlert = 5f;
    public float m_MinDistanceToAttack = 3f;
    public float m_MaxDistanceToAttack = 7f;
    public float m_MaxDistanceToPatrol = 15f;
    public float m_ConeAngle = 60f;
    public float m_LerpAttackRotation = 0.6f;
    public float m_Timer = 0;
    public float m_FireRate = 1.0f;
    public float m_NextTimeToFire = 0.0f;
    public int m_Damage = 20;
    public ParticleSystem m_WeaponFlash;
    public GameController m_GameController;
    public List<Transform> m_Waypoints;
    private Animation m_animation;
    public AnimationClip m_Hit;
    int m_CurrentWaypointId = 0;
    public float m_Health = 50.0f;
    public ParticleSystem m_DeadExposion;
    public Transform m_Eyes;
    private Vector3 m_InitialPos;
    public LayerMask m_SightLayerMask;
    public GameObject m_Drops;
    #endregion
    private void Awake()
    {
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_animation = GetComponent<Animation>();
        m_GameController = FindObjectOfType<GameController>();
    }
    void Start()
    {
        SetIdleState();
        m_InitialPos = transform.position;
    }
    void Update()
    {
        #region Gizmo Cono Vision
        float l_Angle = (m_ConeAngle / 2) * Mathf.Deg2Rad;
        Vector3 l_dirRight = (transform.forward * Mathf.Cos(l_Angle) + transform.right * Mathf.Sin(l_Angle)).normalized;
        Vector3 l_dirLeft = (transform.forward * Mathf.Cos(l_Angle) - transform.right * Mathf.Sin(l_Angle)).normalized;
        Debug.DrawRay(m_Eyes.transform.position, l_dirRight * m_MaxDistanceToPatrol, Color.red);
        Debug.DrawRay(m_Eyes.transform.position, l_dirLeft * m_MaxDistanceToPatrol, Color.red);
        #endregion

        if (m_Health <= 0f)
        {
            SetDieState();
        }

        m_CurrentTime += Time.deltaTime;
        switch (m_State)
        {
            case TState.IDLE:
                UpdateIdleState();
                break;
            case TState.PATROL:
                UpdatePatrolState();
                break;
            case TState.ALERT:
                UpdateAlertState();
                break;
            case TState.CHASE:
                UpdateChaseState();
                break;
            case TState.ATTACK:
                UpdateAttackState();
                break;
            case TState.HIT:
                UpdateHitState();
                break;
            case TState.DIE:
                UpdateDieState();
                break;
        }
    }

    #region Update States
    void UpdateIdleState()
    {
        if (m_CurrentTime >= m_IdleTime)
            SetPatrolState();
    }
    void UpdatePatrolState()
    {
        if (!m_NavMeshAgent.hasPath && m_NavMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
            MoveToNextPatrolPosition();
        if (HearsPlayer())
        {
            SetAlertState();
        }
        if (SeesPlayer())
        {
            SetChaseState();
        }

    }
    void UpdateAlertState()
    {
        m_Timer += Time.deltaTime;
        LookingForPlayer();
        if (SeesPlayer())
        {
            SetChaseState();
        }
        if (m_Timer >= 30f)
        {
            m_Timer = 0;
            SetPatrolState();
        }
    }
    void UpdateChaseState()
    {
        if (Distance2Player() <= m_MaxDistanceToAttack)
        {
            SetAttackState();
        }
        if (!SeesPlayer() || Distance2Player() > m_MaxDistanceToPatrol)
        {
            SetAlertState();
        }
    }
    void UpdateAttackState()
    {
        if (Time.time >= m_NextTimeToFire)
        {
            m_NextTimeToFire = Time.time + 1f / m_FireRate;
            if (SeesPlayer())
                Shoot();
        }

        if (SeesPlayer() && Distance2Player() > m_MaxDistanceToAttack)
            SetChaseState();

        if (!SeesPlayer())
            SetAlertState();

    }
    void UpdateHitState()
    {
        m_animation.clip = m_Hit;
        m_animation.Play();

        if (!SeesPlayer())
        {
            SetAlertState();
        }
        else
        {
            SetChaseState();
        }

    }
    void UpdateDieState()
    {
        if (Random.Range(0, 3) == 2)
        {
            if (m_Drops != null)
                GameObject.Instantiate(m_Drops, new Vector3(transform.position.x, 0.5f, transform.position.z), Quaternion.identity);
        }
        if (m_DeadExposion != null)
        {
            GameObject.Instantiate(m_DeadExposion, new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), Quaternion.identity);
        }
        Destroy(gameObject);
    }
    #endregion

    #region Set States
    void SetIdleState()
    {
        m_State = TState.IDLE;
        m_NavMeshAgent.isStopped = true;
        m_CurrentTime = 0f;
    }
    void SetPatrolState()
    {
        m_State = TState.PATROL;
        m_CurrentTime = 0f;
        GetComponentInChildren<Light>().color = Color.white;
        MoveToNextPatrolPosition();
    }
    void SetAlertState()
    {
        m_State = TState.ALERT;
        m_CurrentTime = 0f;
        GetComponentInChildren<Light>().color = Color.yellow;
    }
    void SetChaseState()
    {
        m_State = TState.CHASE;
        m_CurrentTime = 0f;
        GetComponentInChildren<Light>().color = Color.red;
        SetNextChasePosition();
    }
    void SetAttackState()
    {
        m_State = TState.ATTACK;
        m_CurrentTime = 0f;
    }
    void SetHitState()
    {
        m_State = TState.HIT;
        m_CurrentTime = 0f;
    }
    void SetDieState()
    {
        m_State = TState.DIE;
        m_CurrentTime = 0f;
    }

    #endregion

    void SetNextChasePosition()
    {
        Vector3 l_Direction = m_GameController.m_Player.transform.position - transform.position; //Vector Enemy-Player
        float l_DistanceToPlayer = Distance2Player(); //Distancia
        float l_MovementDistance = l_DistanceToPlayer - m_MinDistanceToAttack;

        //No normalizamos el vector porque és una opcion costosa de calcular.
        l_Direction /= l_DistanceToPlayer;

        Vector3 l_ChasePosition = transform.position + l_Direction * l_MovementDistance;

        m_NavMeshAgent.SetDestination(l_ChasePosition);
        m_NavMeshAgent.isStopped = false;
    }
    void MoveToNextPatrolPosition()
    {
        m_NavMeshAgent.isStopped = false;
        m_NavMeshAgent.SetDestination(m_Waypoints[m_CurrentWaypointId].position);
        ++m_CurrentWaypointId;
        if (m_CurrentWaypointId >= m_Waypoints.Count)
        {
            m_CurrentWaypointId = 0;
        }
    }
    private bool SeesPlayer()
    {
        Vector3 l_Direction = (m_GameController.m_Player.transform.position + Vector3.up * 1.6f) - transform.position;
        float l_DistanceToPlayer = l_Direction.magnitude;
        l_Direction /= l_DistanceToPlayer;
        Debug.DrawRay(m_Eyes.position, l_Direction * m_MaxDistanceToPatrol, Color.blue);
        bool l_IsOnCone = Vector3.Dot(transform.forward, l_Direction) >= Mathf.Cos(m_ConeAngle * Mathf.Deg2Rad * 0.5f);
        Ray l_Ray = new Ray(m_Eyes.position, l_Direction);

        if (l_DistanceToPlayer > m_MaxDistanceToPatrol)
            return false;

        if (l_IsOnCone && !Physics.Raycast(l_Ray, l_DistanceToPlayer, m_SightLayerMask))
        {
            return true;
        }

        return false;
    }
    private bool HearsPlayer()
    {
        float l_DistanceToPlayer = Vector3.Distance(m_GameController.m_Player.transform.position, transform.position);
        return l_DistanceToPlayer < m_MaxDistanceToAlert;
    }
    private float Distance2Player()
    {
        Vector3 l_Direction = m_GameController.m_Player.transform.position - transform.position; //Vector Enemy-Player
        return l_Direction.magnitude; //Distancia
    }
    private void LookingForPlayer()
    {
        transform.Rotate(new Vector3(0f, 360f, 0f), 30f * Time.deltaTime);
    }
    private void Shoot()
    {
        m_WeaponFlash.Play();
        Vector3 l_Direction = m_GameController.m_Player.transform.position - transform.position;
        l_Direction.Normalize();
        Ray l_Ray = new Ray(m_Eyes.position, l_Direction);
        RaycastHit l_RaycastHit;

        if (Physics.Raycast(l_Ray, out l_RaycastHit, m_MaxDistanceToAttack))
        {
            FPSController target = l_RaycastHit.transform.GetComponentInParent<FPSController>();
            if (target != null)
            {
                target.HurtingPlayer(m_Damage);
            }
        }

    }
    public void TakeDamage(float damage)
    {
        m_Health -= damage;
        SetHitState();

    }
    public void Respawn()
    {
        transform.position = m_InitialPos;
    }
}
