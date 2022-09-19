using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameBreath : PlainAiAbility
{
    // All character's have a basic attack ability which can have different damage, range, and animations.
    public FlameBreath()
    {
        this.name = AbilityName.FLAME_BREATH;
        this.damage = 0f;
        this.range = 30f;
        this.cooldownTime = 15f;
        this.animationNameList = new List<string> { "Flame Breath" };
        this.statusEffects = new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.HealthPerSecond, StatusEffectTarget.AreaOfEffect, -40f, 6),
        };
    }

    public override float HandleSpecialAbilitiesAndGetDamageModifier(Entity castingEntity, Entity receivingEntity)
    {
        return 0; // no special effects or damage modifiers
    }
}
