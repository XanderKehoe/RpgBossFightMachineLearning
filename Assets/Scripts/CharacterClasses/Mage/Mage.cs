using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : Character
{
    public override void Start()
    {
        base.Start();
                    //new CharacterAbility("name", damage, range, mana cost, animationType, applied status effects
        abilities.Add(new CharacterAbility(AbilityName.BASIC_ATTACK, 50, DEFAULT_RANGED_ATTACK_RANGE, 0, Ability.AnimationType.CastSpell, 
            new List<StatusEffect> { }));
        abilities.Add(new CharacterAbility(AbilityName.PYROBLAST, 100, DEFAULT_RANGED_ATTACK_RANGE, 70, Ability.AnimationType.CastSpell, 
            new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.HealthPerSecond, StatusEffectTarget.SelectedTarget, -20, 5)
        }));
        abilities.Add(new CharacterAbility(AbilityName.FROSTBOLT, 75, DEFAULT_RANGED_ATTACK_RANGE, 70, Ability.AnimationType.CastSpell, 
            new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.AttackSpeed, StatusEffectTarget.SelectedTarget, -0.2f, 15)
        }));
        abilities.Add(new CharacterAbility(AbilityName.SCORCH, 50, DEFAULT_RANGED_ATTACK_RANGE, 50, Ability.AnimationType.CastSpell, 
            new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.HealthPerSecond, StatusEffectTarget.SelectedTarget, -5, 5),
            new StatusEffect(StatusEffectType.HitChanceOnTarget, StatusEffectTarget.SelectedTarget, 10, 2)
        }));
        abilities.Add(new CharacterAbility(AbilityName.EVOCATION, 0, float.PositiveInfinity, 100, Ability.AnimationType.CastHeal, 
            new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.ManaPerSecond, StatusEffectTarget.Self, 60, 5)
        }));
        abilities.Add(new CharacterAbility(AbilityName.TRANSFER_MANA, 0, DEFAULT_RANGED_ATTACK_RANGE, 200, Ability.AnimationType.CastSpell, 
            new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.ManaPerSecond, StatusEffectTarget.AllFriendlies, 66 / teammates.Count, 3)
        }));
        abilities.Add(new CharacterAbility(AbilityName.TIME_WARP, 0, float.PositiveInfinity, 200, Ability.AnimationType.CastHeal, 
            new List<StatusEffect> 
        { 
            new StatusEffect(StatusEffectType.AttackSpeed, StatusEffectTarget.AllFriendlies, 0.20f, 10),
            new StatusEffect(StatusEffectType.MovementSpeedMultiplier, StatusEffectTarget.AllFriendlies, 0.30f, 10)
        }));
    }
}
