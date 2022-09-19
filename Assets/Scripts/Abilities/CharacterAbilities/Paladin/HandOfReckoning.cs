using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandOfReckoning : CharacterAbility
{
    public HandOfReckoning()
    {
        this.name = AbilityName.HAND_OF_RECKONING;
        this.damage = 0f;
        this.range = Character.MELEE_RANGE + 15f;
        this.manaCost = 60f;
        this.animationNameList = new List<string> { "CastSpell" };
        this.statusEffects = new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.ThreatGeneration, StatusEffectTarget.Self, Paladin.HAND_OF_RECKONING_THREAT_GEN, 6)
        };
    }

    public override float HandleSpecialAbilitiesAndGetDamageModifier(Entity castingEntity, Entity receivingEntity)
    {
        return 0;
    }
}
