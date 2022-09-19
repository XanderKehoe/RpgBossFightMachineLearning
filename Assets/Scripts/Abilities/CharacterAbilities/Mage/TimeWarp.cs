using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeWarp : CharacterAbility
{
    public TimeWarp()
    {
        this.name = AbilityName.TIME_WARP;
        this.damage = 0f;
        this.range = float.PositiveInfinity;
        this.manaCost = 200f;
        this.animationNameList = new List<string> { "CastHeal" };
        this.statusEffects = new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.AttackSpeed, StatusEffectTarget.AllFriendlies, 0.30f, 10),
            new StatusEffect(StatusEffectType.MovementSpeedMultiplier, StatusEffectTarget.AllFriendlies, 0.30f, 10)
        };
    }

    public override float HandleSpecialAbilitiesAndGetDamageModifier(Entity castingEntity, Entity receivingEntity)
    {
        throw new System.NotImplementedException();
    }
}
