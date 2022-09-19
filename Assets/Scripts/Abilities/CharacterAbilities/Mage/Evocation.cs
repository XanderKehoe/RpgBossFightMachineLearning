using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evocation : CharacterAbility
{
    public Evocation()
    {
        this.name = AbilityName.EVOCATION;
        this.damage = 0f;
        this.range = float.PositiveInfinity;
        this.manaCost = 100f;
        this.animationNameList = new List<string> { "CastHeal" };
        this.statusEffects = new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.ManaPerSecond, StatusEffectTarget.Self, 60, 5)
        };
    }

    public override float HandleSpecialAbilitiesAndGetDamageModifier(Entity castingEntity, Entity receivingEntity)
    {
        throw new System.NotImplementedException();
    }
}
