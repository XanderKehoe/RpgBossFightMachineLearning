using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunter : Character
{
    public static uint SERPENT_STING_TIME = 20;

    public override void Start()
    {
        base.Start();
                    //new CharacterAbility("name", damage, range, mana cost, animationType, applied status effects
        abilities.Add(new CharacterAbility(AbilityName.BASIC_ATTACK, 50, DEFAULT_RANGED_ATTACK_RANGE, 0, Ability.AnimationType.ShootArrow, 
            new List<StatusEffect> { }));
        abilities.Add(new CharacterAbility(AbilityName.SERPENT_STING, 100, DEFAULT_RANGED_ATTACK_RANGE, 140, Ability.AnimationType.ShootArrow, 
            new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.HealthPerSecond, StatusEffectTarget.SelectedTarget, -30, SERPENT_STING_TIME)
        }));
        abilities.Add(new CharacterAbility(AbilityName.CHIMERA_SHOT, 75, DEFAULT_RANGED_ATTACK_RANGE, 70, Ability.AnimationType.ShootArrow, 
            new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.CriticalStrikeChanceOnSelf, StatusEffectTarget.SelectedTarget, -0.2f, 15)
        }));
        abilities.Add(new CharacterAbility(AbilityName.KILL_SHOT, 50, DEFAULT_RANGED_ATTACK_RANGE, 50, Ability.AnimationType.ShootArrow, 
            new List<StatusEffect> { }));
        abilities.Add(new CharacterAbility(AbilityName.FOCUSED_AIM, 0, float.PositiveInfinity, 100, Ability.AnimationType.CastHeal, 
            new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.HitChanceOnTarget, StatusEffectTarget.AllFriendlies, 0.1f, 10)
        }));
        abilities.Add(new CharacterAbility(AbilityName.RAPID_FIRE, 0, DEFAULT_RANGED_ATTACK_RANGE, 100, Ability.AnimationType.CastHeal, 
            new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.AttackSpeed, StatusEffectTarget.Self, 0.5f, 8)
        }));
        abilities.Add(new CharacterAbility(AbilityName.TRANQUILIZING_DART, 0, float.PositiveInfinity, 100, Ability.AnimationType.ShootArrow, 
            new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.AttackPower, StatusEffectTarget.SelectedTarget, -0.2f, 15),
        }));
    }
}
