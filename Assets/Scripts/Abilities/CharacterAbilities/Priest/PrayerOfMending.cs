using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrayerOfMending : CharacterAbility
{
    public PrayerOfMending()
    {
        this.name = AbilityName.PRAYER_OF_MENDING;
        this.damage = 0f;
        this.range = float.PositiveInfinity;
        this.manaCost = 85f;
        this.animationNameList = new List<string> { "CastHeal" };
        this.statusEffects = new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.HealthPerSecond, StatusEffectTarget.AllFriendlies, 30, 5)
        };
    }

    public override float HandleSpecialAbilitiesAndGetDamageModifier(Entity castingEntity, Entity receivingEntity)
    {
        throw new System.NotImplementedException();
    }
}
