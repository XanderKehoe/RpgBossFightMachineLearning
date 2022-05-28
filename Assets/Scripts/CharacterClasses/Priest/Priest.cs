using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Priest : Character
{
    public override void Start()
    {
        base.Start();

        abilities.Add(new CharacterAbility(AbilityName.BASIC_ATTACK, 50, DEFAULT_RANGED_ATTACK_RANGE, 0, Ability.AnimationType.CastSpell, 
            new List<StatusEffect> { }));
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
        abilities.Add(new CharacterAbility(AbilityName.PRAYER_OF_MENDING, 0, DEFAULT_RANGED_ATTACK_RANGE, 100, Ability.AnimationType.CastHeal, 
            new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.HealthPerSecond, StatusEffectTarget.AllFriendlies, 25, 5),
        }));
        abilities.Add(new CharacterAbility(AbilityName.SMITE, 75, float.PositiveInfinity, 75, Ability.AnimationType.CastSpell, 
            new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.HitChanceOnSelf, StatusEffectTarget.SelectedTarget, 0.1f, 15)
        }));
        abilities.Add(new CharacterAbility(AbilityName.BLINDING_LIGHT, 25, DEFAULT_RANGED_ATTACK_RANGE, 125, Ability.AnimationType.CastSpell, 
            new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.HitChanceOnTarget, StatusEffectTarget.SelectedTarget, -0.15f, 15)
        }));
        abilities.Add(new CharacterAbility(AbilityName.INNER_FIRE, 0, float.PositiveInfinity, 150, Ability.AnimationType.CastHeal, 
            new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.AttackSpeed, StatusEffectTarget.Self, 0.30f, 15),
            new StatusEffect(StatusEffectType.ManaPerSecond, StatusEffectTarget.Self, 10f, 15)
        }));
    }
}
