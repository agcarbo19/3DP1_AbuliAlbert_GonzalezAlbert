using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DroneEnemy : MonoBehaviour
{

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
    public FPSController m_Player;
    public List<Transform> m_Waypoints;
    int m_CurrentWaypointId = 0;

    public Transform m_Eyes;
    public float m_MaxDistanceToRaycast = 15f;
    public LayerMask m_SightLayerMask;

    private void Awake()
    {
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        SetIdleState();
    }


    void Update()
    {
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
        //Falta millorar!
        if (!m_NavMeshAgent.hasPath && m_NavMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
            MoveToNextPatrolPosition();
        //if (HearsPlayer())
        //{
        //    SetAlertState();
        //}
        //if (SeesPlayer)
        //{
        //    SetChaseState();
        //}
    }
    void UpdateAlertState()
    {

    }
    void UpdateChaseState()
    {

    }
    void UpdateAttackState()
    {

    }
    void UpdateHitState()
    {

    }
    void UpdateDieState()
    {

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
        MoveToNextPatrolPosition();
    }
    void SetAlertState()
    {
        m_State = TState.ALERT;
        m_CurrentTime = 0f;
    }
    void SetChaseState()
    {
        m_State = TState.CHASE;
        m_CurrentTime = 0f;
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
        Vector3 l_Direction = m_Player.transform.position - transform.position; //Vector Enemy-Player
        float l_DistanceToPlayer = l_Direction.magnitude; //Distancia
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
        Vector3 l_Direction = m_Player.transform.position - transform.position;
        l_Direction.Normalize();
        bool l_IsOnCone = Vector3.Dot(transform.forward, l_Direction) >= Mathf.Cos(m_ConeAngle * Mathf.Deg2Rad * 0.5f);

        Ray l_Ray = new Ray(m_Eyes.position, l_Direction);
        if (l_IsOnCone && Physics.Raycast(l_Ray, m_MaxDistanceToRaycast, m_SightLayerMask))
        {
            return false;
        }

        return true;

        //LayerMask yes to all menos al player y enemigos.
    }

    private bool HearsPlayer()
    {
        float l_DistanceToPlayer = Vector3.Distance(m_Player.transform.position, transform.position);
        return l_DistanceToPlayer < m_MaxDistanceToAlert;
    }

}
