using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAnimator : MonoBehaviour
{
    NavMeshAgent navMeshAgent;
    Animator animator;

    const float ANIMATION_SMOOTH_TIME = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        if (navMeshAgent == null)
            Debug.LogError(name + ": CharacterAnimator - navMeshAgent is NULL");

        if (animator == null)
            Debug.LogError(name + ": CharacterAnimator - animator is NULL");
    }

    // Update is called once per frame
    void Update()
    {
        float speedPercent = navMeshAgent.velocity.magnitude / navMeshAgent.speed;
        animator.SetFloat("speedPercent", speedPercent, ANIMATION_SMOOTH_TIME, Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Keydown A detected.");
            if (this.name.Contains("Dragon"))
                animator.CrossFade("Flame Attack", 0.25f);
            else
                animator.CrossFade("CastSpell", 0.25f);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (this.name.Contains("Dragon"))
                animator.CrossFade("Defend", 0.25f);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (this.name.Contains("Dragon"))
                animator.CrossFade("Get Hit", 0.25f);
        }
    }
}
