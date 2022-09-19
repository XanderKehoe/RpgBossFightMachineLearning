using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : Entity
{
    public const float MELEE_RANGE = 3f;
    public const float DEFAULT_RANGED_ATTACK_RANGE = 30f;

    public CharacterAbility nextAbility { get; private set; } = null;

    public override void Start()
    {
        base.Start();

        if (mlAgentsController == null)
            Debug.LogError(name + ": Character::Start() - abilityChooser is NULL");

        if (currentTarget != null)
        {
            if (currentTarget.name == "Dragon")
            {
                currentTarget.GetComponent<Dragon>().AddToThreatMap(this);
            }
            else
                Debug.LogWarning(name + ": Character::Start() - target found, but isn't 'Dragon'?");
        }
    }

    protected void MakeCombatMovementDecision()
    {
        navMeshAgent.SetDestination(mlAgentsController.GetNewTargetPosition(this));
    }

    protected override void MakeCombatDecision()
    {
        if (currentTarget != null)
        {
            // In future
            // Determine if/where we should move
            // Determine target
            // Determine ability

            MakeCombatMovementDecision();

            if (abilities.Count == 0)
            {
                Debug.LogError("(" + simulation.name + ") " + name + ": MakeCombatDecision - No abilities to choose from");
            }

            CharacterAbility selectedAbility;
            int nextAbilityIndex = GetNextAbilityIndex();
            try
            {
                selectedAbility = (CharacterAbility)abilities[nextAbilityIndex];
                nextAbility = selectedAbility;
            }
            catch(Exception e)
            {
                Debug.LogError("(" + simulation.name + ") " + name + ": MakeCombatDecision() - Caught error: \n " + e.ToString() + " \n abilities.count/nextAbility Index [" + abilities.Count + "][" + nextAbilityIndex + "]");
                return;
            }

            if (IsAbilityInRange(currentTarget, selectedAbility.range))
            {
                //navMeshAgent.isStopped = true; //(to be removed once movement decisions are figured out)

                //Debug.Log(name + " was in range, reward is: " + MlAgentsController.REWARD_CAST_ABILITY_WITHIN_RANGE);
                mlAgentsController.GiveRewardWithTracking(MlAgentsController.REWARD_CAST_ABILITY_WITHIN_RANGE, "REWARD_CAST_ABILITY_WITHIN_RANGE"); // reward agent for casting ability within range

                if (stats.mana >= selectedAbility.manaCost)
                {
                    //Debug.Log(name + ": MakeCombatDecision() - casting [" + randAbility.name + "]");
                    CastAbility(currentTarget, selectedAbility);
                }
                else
                {
                    //Debug.Log(name + ": MakeCombatDecision() - Not enough mana to cast [" + randAbility.name + "], mana["+stats.mana+"] manaCost["+randAbility.manaCost+"] | casting basic attack");
                    CastAbility(currentTarget, abilities[0]);
                }
            }
            else
            {
                // give up to a maximum of half the reward for being in casting range, decreasing the reward the further the casting entity is from the target.
                float r = 0.1f; // chosen based on testing from Desmos.
                float maxReward = MlAgentsController.REWARD_CAST_ABILITY_WITHIN_RANGE / 2;
                float distanceNeededToCast = Vector3.Distance(currentTarget.transform.position, transform.position) - selectedAbility.range;
                float xTimesR = distanceNeededToCast * r;

                // y = ((max / 2) / (e^(rx)-rx)) + ((max / 2) / (e^(-rx)+rx))
                float reward = ((maxReward / 2) / ((float) Math.Exp(xTimesR) - xTimesR)) + ((maxReward / 2) / ((float) Math.Exp(-xTimesR) + xTimesR));

                mlAgentsController.GiveRewardWithTracking(reward, "REWARD_CAST_ABILITY_OUTSIDE_OF_RANGE");
                //Debug.Log(name + ": not in range, reward is: " + reward);
            }
            /*else // automatically get in target range (to be removed once movement decisions are figured out)
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(currentTarget.transform.position
                    + (Vector3.Normalize(transform.position - currentTarget.transform.position) * currentTarget.meleeRangeRadius));
            } */
        }
    }

    private int GetNextAbilityIndex()
    {
        return mlAgentsController.GetNextAbiliyIndex(this);
    }
}
