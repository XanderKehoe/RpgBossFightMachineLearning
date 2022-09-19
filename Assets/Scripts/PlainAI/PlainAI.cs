using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlainAI : Entity
{
    private HashSet<AbilityName> onCooldownList = new ();
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
            Ability randAbility = null;
            bool foundAbility = false;
            while (!foundAbility)
            {
                randAbility = abilities[Random.Range(0, abilities.Count)];
                //Debug.Log(name + ": Randomly trying: " + randAbility.name.ToString());
                if (!onCooldownList.Contains(randAbility.name))
                    foundAbility = true;
            }

            //Debug.Log(name + ": Randomly chose: " + randAbility.name.ToString());

            if (!(randAbility is PlainAiAbility))
            {
                Debug.LogError(name + ": MakeCombatDecision - randAbility [" + randAbility.name + "] is not a PlainAiAbility, returning...");
                return;
            }

            PlainAiAbility castedRandAbility = (PlainAiAbility)randAbility;

            if (IsAbilityInRange(currentTarget, castedRandAbility.range))
            {
                navMeshAgent.isStopped = true;
                //Debug.Log(name + ": isStopped = true");

                CastAbility(currentTarget, castedRandAbility);
                SetAbilityCooldown(castedRandAbility.name, castedRandAbility.cooldownTime);

                //Debug.Log(name + ": MakeCombatDecision() - Casted [" + castedRandAbility.name.ToString() + "]");
            }
            else
            {
                navMeshAgent.isStopped = false;
                //Debug.Log(name + ": isStopped = false");
            }
        }
    }

    protected void SetAbilityCooldown(AbilityName abilityName, float cooldownTime)
    {
        if (abilityName != AbilityName.BASIC_ATTACK)
        {
            onCooldownList.Add(abilityName);

            StartCoroutine(StartAbilityCooldownTimer(abilityName, cooldownTime));
        }
    }

    IEnumerator StartAbilityCooldownTimer(AbilityName abilityName, float time)
    {
        yield return new WaitForSeconds(time);

        //Debug.Log(name + " Removed [" + abilityName.ToString() + "] off cooldown");
        onCooldownList.Remove(abilityName);
    }

    public override void ResetStatsAndStatusEffects()
    {
        base.ResetStatsAndStatusEffects();

        StopAllCoroutines(); // stop coroutines from previous episodes.

        ResetGlobalCooldownTimer();

        onCooldownList.Clear();

        // set boss to initially have all cooldowns on max
        foreach (PlainAiAbility ability in abilities)
        {
            if (ability.name != AbilityName.BASIC_ATTACK)
                SetAbilityCooldown(ability.name, ability.cooldownTime);
        }
    }

    public void AddToThreatMap(Entity entity) 
    {
        if (!threatMap.ContainsKey(entity))
            threatMap.Add(entity, 0);
    }
}
