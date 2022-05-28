using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlainAI : Entity
{
    private List<AbilityName> onCooldownList = new ();
    [SerializeField] public Dictionary<Entity, float> threatMap = new();

    protected override void MakeCombatDecision()
    {
        if (currentTarget != null)
        {
            if (abilities.Count == 0)
            {
                Debug.LogError(name + ": MakeCombatDecision - No abilities to choose from");
                return;
            }

            // For now, just selected a random ability not on cooldown
            Ability randAbility;
            while (true)
            {
                randAbility = abilities[Random.Range(0, abilities.Count)];
                if (!onCooldownList.Contains(randAbility.name))
                    break;
            }

            if (!(randAbility is PlainAiAbility))
            {
                Debug.LogError(name + ": MakeCombatDecision - randAbility [" + randAbility.name + "] is not a PlainAiAbility, returning...");
                return;
            }

            PlainAiAbility castedRandAbility = (PlainAiAbility)randAbility;

            SetAbilityCooldown(castedRandAbility.name, castedRandAbility.cooldownTime);

            if (IsAbilityInRange(currentTarget, randAbility.range))
            {
                navMeshAgent.isStopped = true;

                CastAbility(currentTarget, randAbility);
            }
            else
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(currentTarget.transform.position
                    + (Vector3.Normalize(transform.position - currentTarget.transform.position) * currentTarget.meleeRangeRadius));
            }
        }
    }

    protected void SetAbilityCooldown(AbilityName abilityName, float cooldownTime)
    {
        if (abilityName != AbilityName.BASIC_ATTACK)
        {
            onCooldownList.Add(abilityName);

            StartAbilityCooldownTimer(abilityName, cooldownTime);
        }
    }

    IEnumerator StartAbilityCooldownTimer(AbilityName abilityName, float time)
    {
        yield return new WaitForSeconds(time);

        onCooldownList.Remove(abilityName);
    }
}
