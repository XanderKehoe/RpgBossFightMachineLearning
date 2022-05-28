using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats
{
    public float health;
    public float maxHealth;
    public float mana;
    public float maxMana;

    public float baseHitChance = 0.8f;
    public float baseCritChance = 0.1f;
    public float baseBlockChance = 0.05f;

    public float baseMoveSpeed = 4.5f;

    public Dictionary<string, float> GetHealthManaRatios()
    {
        Dictionary<string, float> ret = new();

        ret.Add("health", health / maxHealth);
        ret.Add("mana", mana / maxMana);

        return ret;
    }
}
