using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerOfTheRighteous : CharacterAbility
{
    public HammerOfTheRighteous()
    {
        this.name = AbilityName.HAMMER_OF_THE_RIGHTEOUS;
        this.damage = 30f;
        this.range = Character.MELEE_RANGE;
        this.manaCost = 30f;
        this.animationNameList = new List<string> { "MeleeAttack1", "MeleeAttack2" };
        this.statusEffects = new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.ThreatGeneration, StatusEffectTarget.Self, Paladin.HAMMER_OF_THE_RIGHTEOUS_THREAT_GEN, 5),
            new StatusEffect(StatusEffectType.ManaPerSecond, StatusEffectTarget.Self, 10, 2)
        };
    }

    public override float HandleSpecialAbilitiesAndGetDamageModifier(Entity castingEntity, Entity receivingEntity)
    {
        return 0;
    }
}
