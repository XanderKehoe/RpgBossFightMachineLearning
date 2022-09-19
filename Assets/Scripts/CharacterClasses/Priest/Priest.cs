using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Priest : Character
{
    public override void Start()
    {
        base.Start();

        abilities.Add(new CharacterBasicAttack(50, DEFAULT_RANGED_ATTACK_RANGE, new List<string> { "CastSpell" }));
        abilities.Add(new PrayerOfMending());
        abilities.Add(new Smite());
        abilities.Add(new BlindingLight());
        abilities.Add(new InnerFire());
    }

    /*abilities.Add(new CharacterAbility("Holy Light", 0, DEFAULT_RANGED_ATTACK_RANGE, 70, Ability.AnimationType.CastHeal, new List<StatusEffect>
    {
        new StatusEffect(StatusEffectType.HealthPerSecond, StatusEffectTarget.SelectedTarget, 75, 3)
    }));
    abilities.Add(new CharacterAbility("Renew", 0, DEFAULT_RANGED_ATTACK_RANGE, 35, Ability.AnimationType.CastHeal, new List<StatusEffect>
    {
        new StatusEffect(StatusEffectType.HealthPerSecond, StatusEffectTarget.SelectedTarget, 15, 10)
    }));
    abilities.Add(new CharacterAbility("Holy Shield", 0, DEFAULT_RANGED_ATTACK_RANGE, 70, Ability.AnimationType.CastSpell, new List<StatusEffect>
    {
        new StatusEffect(StatusEffectType.DamageAbsorption, StatusEffectTarget.SelectedTarget, 250, 10)
    }));*/
}
