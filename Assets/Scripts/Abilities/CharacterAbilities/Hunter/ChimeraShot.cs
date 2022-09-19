using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChimeraShot : CharacterAbility
{
    public ChimeraShot()
    {
        this.name = AbilityName.CHIMERA_SHOT;
        this.damage = 75f;
        this.range = Character.DEFAULT_RANGED_ATTACK_RANGE;
        this.manaCost = 70f;
        this.animationNameList = new List<string> { "ShootArrow" };
        this.statusEffects = new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.CriticalStrikeChanceOnSelf, StatusEffectTarget.SelectedTarget, -0.35f, 15)
        };
    }

    public override float HandleSpecialAbilitiesAndGetDamageModifier(Entity castingEntity, Entity receivingEntity)
    {
        foreach (StatusEffect statusEffect in receivingEntity.currentStatusEffects)
        {
            if (statusEffect.afflictingAbilityName == AbilityName.SERPENT_STING)
            {
                statusEffect.time = Hunter.SERPENT_STING_TIME;
            }
        }

        return 0;
    }
}
