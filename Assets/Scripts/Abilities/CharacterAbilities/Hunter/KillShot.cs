using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillShot : CharacterAbility
{
    public KillShot()
    {
        this.name = AbilityName.KILL_SHOT;
        this.damage = 100f;
        this.range = Character.DEFAULT_RANGED_ATTACK_RANGE;
        this.manaCost = 50f;
        this.animationNameList = new List<string> { "ShootArrow" };
        this.statusEffects = new List<StatusEffect> { };
    }

    public override float HandleSpecialAbilitiesAndGetDamageModifier(Entity castingEntity, Entity receivingEntity)
    {
        float hpRatio = receivingEntity.stats.health / receivingEntity.stats.maxHealth;
        float damageModifier = 1 - hpRatio;
        return damageModifier;
    }
}
