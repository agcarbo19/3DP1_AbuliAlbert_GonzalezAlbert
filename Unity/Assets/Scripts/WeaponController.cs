using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public float m_Damage = 10.0f;
    public float m_FireRate = 15.0f;
    public float m_ImpactForce = 30.0f;
    public float m_NextTimeToFire = 0.0f;

    public Camera m_Camera;
    public GameController m_GameController;
    public ParticleSystem m_WeaponFlash;
    public GameObject m_SmokeImpact;
    public GameObject m_ImpactEffect;

    [Header("Shoot")]
    public LayerMask m_ShootLayerMask;
    public GameObject m_HitCollisionParticlesPrefab;
    public float m_MaxDistance = 150.0f;

    [Header("Weapon Animation")]
    public Animation m_WeaponAnimation;
    public AnimationClip m_IdleWeaponAnimation;
    public AnimationClip m_ShootWeaponAnimation;
    public AnimationClip m_ReloadWeaponAnimation;

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= m_NextTimeToFire)
        {
            m_NextTimeToFire = Time.time + 1f / m_FireRate;
            Shoot();
        }
    }

    private void Shoot()
    {
        //SetShootWeaponAnimation();
        Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit l_RaycastHit;
        m_WeaponFlash.Play();

        if (Physics.Raycast(l_Ray, out l_RaycastHit, m_MaxDistance, m_ShootLayerMask.value))
        {
            Target target = l_RaycastHit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(m_Damage);
            }

            if (l_RaycastHit.rigidbody != null)
            {
                l_RaycastHit.rigidbody.AddForce(-l_RaycastHit.normal * m_ImpactForce);
            }

            CreateShootHitParticle(l_RaycastHit.point, l_RaycastHit.normal, target != null);
        }

    }

    private void CreateShootHitParticle(Vector3 Position, Vector3 Normal, bool target)
    {
        GameObject.Instantiate(m_HitCollisionParticlesPrefab, Position, Quaternion.LookRotation(Normal) * Quaternion.Euler(0.0f, 0.0f, UnityEngine.Random.value * 180.0f), m_GameController.m_DestroyObjects);
        if (target)
        {
            GameObject.Instantiate(m_ImpactEffect, Position, Quaternion.LookRotation(Normal) * Quaternion.Euler(0.0f, 0.0f, UnityEngine.Random.value * 180.0f), m_GameController.m_DestroyObjects);
        }
        else
        {
            GameObject.Instantiate(m_SmokeImpact, Position, Quaternion.LookRotation(Normal) * Quaternion.Euler(0.0f, 0.0f, UnityEngine.Random.value * 180.0f), m_GameController.m_DestroyObjects);
        }
    }

    private void SetIdleWeaponAnimation()
    {
        m_WeaponAnimation.CrossFade(m_IdleWeaponAnimation.name);
    }

    void SetShootWeaponAnimation()
    {
        m_WeaponAnimation.CrossFade(m_ShootWeaponAnimation.name);
        m_WeaponAnimation.CrossFadeQueued(m_IdleWeaponAnimation.name);

    }
}
