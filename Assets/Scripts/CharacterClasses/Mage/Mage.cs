using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : Character
{
    public override void Start()
    {
        base.Start();

        abilities.Add(new CharacterBasicAttack(50, DEFAULT_RANGED_ATTACK_RANGE, new List<string> { "CastSpell" }));
        abilities.Add(new Pyroblast());
        abilities.Add(new Frostbolt());
        abilities.Add(new Scorch());
        abilities.Add(new Evocation());
        abilities.Add(new TransferMana(teammates.Count));
        abilities.Add(new TimeWarp());
    }
}
