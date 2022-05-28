using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paladin : Character
{
    public override void Start()
    {
        base.Start();
                    //new CharacterAbility("name", damage, range, mana cost, animationType, applied status effects
        abilities.Add(new CharacterAbility(AbilityName.BASIC_ATTACK, 35, 0, 0, Ability.AnimationType.MeleeAttack1, new List<StatusEffect> { }));
        abilities.Add(new CharacterAbility(AbilityName.AVENGERS_SHIELD, 50, 15, 100, Ability.AnimationType.CastSpell, new List<StatusEffect> 
        { 
            new StatusEffect(StatusEffectType.HitChanceOnSelf, StatusEffectTarget.SelectedTarget, 0.05f, 5)
        }));
        abilities.Add(new CharacterAbility(AbilityName.DIVINE_SHIELD, 0, 0, 100, Ability.AnimationType.CastHeal, new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.BlockChance, StatusEffectTarget.Self, 0.50f, 5)
        }));
        abilities.Add(new CharacterAbility(AbilityName.HAMMER_OF_THE_RIGHTEOUS, 30, 0, 30, Ability.AnimationType.MeleeAttack2, 
            new List<StatusEffect> 
        {
            new StatusEffect(StatusEffectType.ThreatGeneration, StatusEffectTarget.Self, 6, 5),
            new StatusEffect(StatusEffectType.ManaPerSecond, StatusEffectTarget.Self, 10, 2) 
        }));
        abilities.Add(new CharacterAbility(AbilityName.HAND_OF_PROTECTION, 0, 0, 300, Ability.AnimationType.CastHeal, new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.DamageAbsorption, StatusEffectTarget.AllFriendlies, 1000, 5)
        }));
        abilities.Add(new CharacterAbility(AbilityName.HAND_OF_RECKONING, 0, 15, 60, Ability.AnimationType.CastSpell, new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.ThreatGeneration, StatusEffectTarget.Self, 10, 6)
        }));
        abilities.Add(new CharacterAbility(AbilityName.JUDGEMENT, 150, 10, 30, Ability.AnimationType.CastSpell, new List<StatusEffect> { }));
    }
}
