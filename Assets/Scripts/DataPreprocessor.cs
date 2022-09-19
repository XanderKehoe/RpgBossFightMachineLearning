using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPreprocessor
{
    private static Dictionary<StatusEffectType, float> statusEffectMinValues = new(); // intentionally left private incase initialization is needed
    private static Dictionary<StatusEffectType, float> statusEffectMaxValues = new();

    public static bool initialized = false;

    public static int BucketData(float value, float min, float max, int range)
    {
        float normalizedValue = (value - min) / (max - min);

        int retVal = Mathf.FloorToInt(normalizedValue * range);

        if (retVal < 0 || retVal > range)
            Debug.LogError("BucketData outside of valid range! Value: "+value+" Min: "+min+" Max: "+max+" Range: "+range);

        return retVal;
    }

    public static float NormalizeData(float value, float min, float max)
    {
        if (value < min || value > max)
        {
            Debug.LogWarning("NormalizeData() - value ["+value+"] was outside of min/max range ["+min+"-"+max+"]");
        }

        return (value - min) / (max - min);
    }

    public static bool IsStatusEffectTypeProcessed(StatusEffectType type, List<Character> characters)
    {
        if (!initialized)
        {
            Initialize(characters);
        }

        if (statusEffectMinValues.ContainsKey(type) && statusEffectMaxValues.ContainsKey(type))
        {
            if (statusEffectMinValues[type] == 0 && statusEffectMaxValues[type] == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            Debug.LogWarning("IsStatusEffectTypeProcessed() - type [" + type.ToString() + "] not in min/max dictionaries");
            return false;
        }
    }

    public static float GetMinStatusEffectValue(StatusEffectType type, List<Character> characters)
    {
        if (characters == null || characters.Count != 5)
        {
            Debug.LogWarning("DataPreprocessor - GetMinStatusEffectValue() - characters was NULL or invalid count");
            return 0;
        }

        if (!initialized)
        {
            Initialize(characters);
        }

        if (statusEffectMinValues.ContainsKey(type))
        {
            return statusEffectMinValues[type];
        }
        else
        {
            Debug.LogWarning("DataPreprocessor - GetMinStatusEffectValue() no value found for type [" + type.ToString() + ". Returning 0");
            return 0;
        }
    }

    public static float GetMaxStatusEffectValue(StatusEffectType type, List<Character> characters)
    {
        if (characters == null || characters.Count != 5)
        {
            Debug.LogWarning("DataPreprocessor - GetMaxStatusEffectValue() - characters was NULL or invalid count");
            return 0;
        }

        if (!initialized)
        {
            Initialize(characters);
        }

        if (statusEffectMaxValues.ContainsKey(type))
        {
            return statusEffectMaxValues[type];
        }
        else
        {
            Debug.LogWarning("DataPreprocessor - GetMaxStatusEffectValue() no value found for type [" + type.ToString() + ". Returning 0");
            return 0;
        }
    }

    public static void Initialize(List<Character> characters)
    {
        void InitializeMinValues()
        {
            // assumes no duplicate characters/status effects (i.e. friendly team buffs)
            foreach (Character character in characters)
            {
                foreach (Ability ability in character.abilities)
                {
                    foreach (StatusEffect statusEffect in ability.statusEffects)
                    {
                        if (statusEffect.strength < 0)
                        {
                            statusEffectMinValues[statusEffect.type] += statusEffect.strength;
                        } 
                    }
                }
            }
        }

        void InitializeMaxValues()
        {
            // assumes no duplicate characters/status effects (i.e. friendly team buffs)
            foreach (Character character in characters)
            {
                foreach (Ability ability in character.abilities)
                {
                    foreach (StatusEffect statusEffect in ability.statusEffects)
                    {
                        if (statusEffect.strength > 0)
                        {
                            statusEffectMaxValues[statusEffect.type] += statusEffect.strength;
                        }
                    }
                }
            }
        }

        if (characters == null)
            return;

        foreach (StatusEffectType statusEffect in Enum.GetValues(typeof(StatusEffectType)))
        {
            statusEffectMinValues.Add(statusEffect, 0);
            statusEffectMaxValues.Add(statusEffect, 0);
        }

        InitializeMinValues();
        InitializeMaxValues();

        initialized = true;
    }
}
