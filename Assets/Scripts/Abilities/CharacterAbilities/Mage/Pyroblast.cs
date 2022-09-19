using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pyroblast : CharacterAbility
{
    public Pyroblast()
    {
        this.name = AbilityName.PYROBLAST;
        this.damage = 100f;
        this.range = Character.DEFAULT_RANGED_ATTACK_RANGE;
        this.manaCost = 70f;
        this.animationNameList = new List<string> { "CastSpell" };
        this.statusEffects = new List<StatusEffect> 
        {
            new StatusEffect(StatusEffectType.HealthPerSecond, StatusEffectTarget.SelectedTarget, -20, 5)
        };
    }

    public override float HandleSpecialAbilitiesAndGetDamageModifier(Entity castingEntity, Entity receivingEntity)
    {
        return 0;
    }
}
