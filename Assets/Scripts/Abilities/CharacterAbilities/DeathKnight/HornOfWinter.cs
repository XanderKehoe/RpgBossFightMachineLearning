using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornOfWinter : CharacterAbility
{
    public HornOfWinter()
    {
        this.name = AbilityName.HORN_OF_WINTER;
        this.damage = 0f;
        this.range = float.PositiveInfinity;
        this.manaCost = 150f;
        this.animationNameList = new List<string> { "CastHeal" };
        this.statusEffects = new List<StatusEffect> 
        {
            new StatusEffect(StatusEffectType.AttackPower, StatusEffectTarget.AllFriendlies, 0.25f, 10)
        };
    }

    public override float HandleSpecialAbilitiesAndGetDamageModifier(Entity castingEntity, Entity receivingEntity)
    {
        throw new System.NotImplementedException();
    }
}
