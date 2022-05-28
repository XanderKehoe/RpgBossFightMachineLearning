using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : PlainAI
{
    public override void Start()
    {
        base.Start();

        abilities.Add(new PlainAiAbility(AbilityName.BASIC_ATTACK, 75, 5f, 0, Ability.AnimationType.BasicAttack, new List<StatusEffect> { }));
        abilities.Add(new PlainAiAbility(AbilityName.RAIN_OF_FIRE, 50, 15, 45, Ability.AnimationType.BasicAttack, new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.HealthPerSecond, StatusEffectTarget.SelectedTarget, -10f, 8)
        }));

        // set boss to initially have all cooldowns on max
        foreach (PlainAiAbility ability in abilities)
        {
            SetAbilityCooldown(ability.name, ability.cooldownTime);
        }
    }
}
