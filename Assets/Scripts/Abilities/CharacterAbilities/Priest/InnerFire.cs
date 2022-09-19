using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerFire : CharacterAbility
{
    public InnerFire()
    {
        this.name = AbilityName.INNER_FIRE;
        this.damage = 0f;
        this.range = float.PositiveInfinity;
        this.manaCost = 150f;
        this.animationNameList = new List<string> { "CastHeal" };
        this.statusEffects = new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.AttackSpeed, StatusEffectTarget.Self, 0.30f, 15),
            new StatusEffect(StatusEffectType.ManaPerSecond, StatusEffectTarget.Self, 10f, 15)
        };
    }

    public override float HandleSpecialAbilitiesAndGetDamageModifier(Entity castingEntity, Entity receivingEntity)
    {
        throw new System.NotImplementedException();
    }
}
