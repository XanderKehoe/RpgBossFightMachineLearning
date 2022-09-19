using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunter : Character
{
    public static uint SERPENT_STING_TIME = 20;

    public override void Start()
    {
        base.Start();

        abilities.Add(new CharacterBasicAttack(50, DEFAULT_RANGED_ATTACK_RANGE, new List<string> { "ShootArrow" }));
        abilities.Add(new SerpentSting());
        abilities.Add(new ChimeraShot());
        abilities.Add(new KillShot());
        abilities.Add(new FocusedAim());
        abilities.Add(new RapidFire());
        abilities.Add(new TranquilizingDart());
    }
}
