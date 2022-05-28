using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathKnight : Character
{
    public override void Start()
    {
        base.Start();
                    //new CharacterAbility("name", damage, range, mana cost, animationType, applied status effects
        abilities.Add(new CharacterAbility(AbilityName.BASIC_ATTACK, 50, 0, 0, Ability.AnimationType.MeleeAttack1, 
            new List<StatusEffect> { }));
        abilities.Add(new CharacterAbility(AbilityName.ICY_TOUCH, 75, 5, 75, Ability.AnimationType.CastSpell, 
            new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.AttackSpeed, StatusEffectTarget.SelectedTarget, -0.10f, 10)
        }));
        abilities.Add(new CharacterAbility(AbilityName.PLAGUE_STRIKE, 75, 0, 75, Ability.AnimationType.MeleeAttack2, 
            new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.AttackPower, StatusEffectTarget.SelectedTarget, -0.10f, 5)
        }));
        abilities.Add(new CharacterAbility(AbilityName.SCOURGE_STRIKE, 100, 0, 100, Ability.AnimationType.MeleeAttack2, 
            new List<StatusEffect> { }));
        abilities.Add(new CharacterAbility(AbilityName.HORN_OF_WINTER, 0, float.PositiveInfinity, 150, Ability.AnimationType.CastHeal, 
            new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.AttackPower, StatusEffectTarget.AllFriendlies, 0.25f, 10)
        }));
        abilities.Add(new CharacterAbility(AbilityName.OBLITERATE, 150, 0, 100, Ability.AnimationType.MeleeAttack1, 
            new List<StatusEffect>{ }));
        abilities.Add(new CharacterAbility(AbilityName.DEATH_STRIKE, 100, 0, 100, Ability.AnimationType.MeleeAttack1, 
            new List<StatusEffect> { }));
    }
}
