using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityUseTracker
{
    Dictionary<string, Dictionary<AbilityName, int>> abilityTrackDict = new();
    public AbilityUseTracker() { }

    public void AddAbilityUse(Entity e, AbilityName abilityName)
    {
        string entityName = e.name;

        if (abilityTrackDict.ContainsKey(entityName))
        {
            if (abilityTrackDict[entityName].ContainsKey(abilityName)) 
            {
                abilityTrackDict[entityName][abilityName] += 1;
            }
            else
            {
                abilityTrackDict[entityName].Add(abilityName, 1);
            }
        }
        else
        {
            abilityTrackDict.Add(entityName, new Dictionary<AbilityName, int>());
            abilityTrackDict[entityName].Add(abilityName, 1);
        }
    }

    public void ClearData()
    {
        abilityTrackDict.Clear();
    }

    public void PrintData(bool clearAfter = true)
    {
        foreach (KeyValuePair<string, Dictionary<AbilityName, int>> kvp1 in abilityTrackDict)
        {
            Debug.Log("Printing results for: " + kvp1.Key);
            int totalAbilityUses = 0;
            foreach (KeyValuePair<AbilityName, int> kvp2 in abilityTrackDict[kvp1.Key])
            {
                totalAbilityUses += kvp2.Value;
            }

            foreach (KeyValuePair<AbilityName, int> kvp2 in abilityTrackDict[kvp1.Key])
            {
                float percentage = (float)(kvp2.Value) / (float)(totalAbilityUses);
                Debug.Log("\t" + kvp2.Key + ": " + string.Format("{0:0.00}", percentage * 100f)+"%");
            }
        }

        if (clearAfter)
        {
            ClearData();
        }
    }


}
