using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBolt : MonoBehaviour
{
    public GameObject target;
    public float speed = 0.25f;

    public ParticleSystem psMain;
    public ParticleSystem psExplode;


    // Update is called once per frame
    void Update()
    {
        FaceTarget();
        MoveToTarget();

        if (HasReachedTarget())
        {
            psExplode.Play();
        }
    }

    void SetColors(Color startMin, Color startMax, Gradient lifetimeMin, Gradient lifetimeMax)
    {
        ParticleSystem.MainModule psMainMainSettings = psMain.main;
        psMainMainSettings.startColor = new ParticleSystem.MinMaxGradient(startMin, startMax);

        ParticleSystem.ColorOverLifetimeModule psMainLifetimeColorSettings = psMain.colorOverLifetime;
        psMainLifetimeColorSettings.color = new ParticleSystem.MinMaxGradient(lifetimeMin, lifetimeMax);
    }

    void FaceTarget()
    {
        this.transform.LookAt(target.transform.position);
    }

    void MoveToTarget()
    {
        Vector3 targetDir = Vector3.Normalize(target.transform.position - this.transform.position);
        this.transform.position += targetDir * speed;
    }

    bool HasReachedTarget()
    {
        Collider targetCollider = target.GetComponent<Collider>();

        if (targetCollider != null)
        {
            if (Vector3.Distance(targetCollider.ClosestPoint(this.transform.position), this.transform.position) < 2f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (Vector3.Distance(target.transform.position, this.transform.position) < 8f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    
}
