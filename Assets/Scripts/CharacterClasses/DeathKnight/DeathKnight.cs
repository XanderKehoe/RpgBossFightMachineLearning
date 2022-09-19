using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathKnight : Character
{
    public override void Start()
    {
        base.Start();

        abilities.Add(new CharacterBasicAttack(50, Character.MELEE_RANGE, new List<string> { "MeleeAttack1", "MeleeAttack2" }));
        abilities.Add(new IcyTouch());
        abilities.Add(new PlagueStrike());
        abilities.Add(new ScourgeStrike());
        abilities.Add(new HornOfWinter());
        abilities.Add(new Obliterate());
        abilities.Add(new DeathStrike());
    }
}
