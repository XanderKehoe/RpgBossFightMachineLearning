using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Entity
{
    public const float MELEE_RANGE = 3f;
    public const float DEFAULT_RANGED_ATTACK_RANGE = 30f;

    public MlAgentsController abilityChooser;
    public CharacterAbility nextAbility { get; private set; } = null;

    

    public override void Start()
    {
        base.Start();

        if (abilityChooser == null)
            Debug.LogError(name + ": Character::Start() - abilityChooser is NULL");
    }

    protected override void MakeCombatDecision()
    {
        if (currentTarget != null)
        {
            // In future
                // Determine if/where we should move
                // Determine target
                // Determine ability

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
                navMeshAgent.isStopped = true;

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
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(currentTarget.transform.position
                    + (Vector3.Normalize(transform.position - currentTarget.transform.position) * currentTarget.meleeRangeRadius));
            }
        }
    }

    private int GetNextAbilityIndex()
    {
        return abilityChooser.GetNextAbiliyIndex(this);
    }
}
