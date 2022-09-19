using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerpentSting : CharacterAbility
{
    public SerpentSting()
    {
        this.name = AbilityName.SERPENT_STING;
        this.damage = 50f;
        this.range = Character.DEFAULT_RANGED_ATTACK_RANGE;
        this.manaCost = 140f;
        this.animationNameList = new List<string> { "ShootArrow" };
        this.statusEffects = new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.HealthPerSecond, StatusEffectTarget.SelectedTarget, -30, Hunter.SERPENT_STING_TIME)
        };
    }

    public override float HandleSpecialAbilitiesAndGetDamageModifier(Entity castingEntity, Entity receivingEntity)
    {
        return 0;
    }
}
