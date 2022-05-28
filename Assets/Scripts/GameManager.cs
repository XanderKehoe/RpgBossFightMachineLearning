using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class GameManager : MonoBehaviour
{
    public GameObject simulation;

    public List<Character> characterTeam = new List<Character>();

    public Dragon dragon;

    public MlAgentsController mlAgentsController;

    // Start is called before the first frame update
    public void Start()
    {
        if (simulation == null)
        {
            Debug.LogError("GameManager::Start() - simulation is NULL");
        }

        if (mlAgentsController == null)
        {
            Debug.LogError("(" + simulation.name + ") GameManager::Start() - mlAgents controller is NULL");
        }

        if (characterTeam.Count != 5)
        {
            Debug.LogError("(" + simulation.name + ") GameManager::Start() - character team count != 5");
        }

        if (dragon == null)
        {
            Debug.LogError("(" + simulation.name + ") GameManager::Start() - dragon is NULL");
        }
    }

    public void CheckForEndOfEpisode(bool debug = false)
    {
        bool oneAlive = false;
        foreach (Entity e in characterTeam)
        {
            if (!e.isDead)
            {
                oneAlive = true;
                break;
            }
        }

        if (!oneAlive)
        {
            mlAgentsController.AddReward(MlAgentsController.REWARD_TEAM_WIPE);
            mlAgentsController.EndEpisode();

            if (debug)
                Debug.Log("(" + simulation.name + ") " + "No AI's alive, dragon wins...");
        }
        else if (mlAgentsController.dragon.isDead)
        {
            float finalReward = MlAgentsController.REWARD_KILL_BOSS / ((float) (mlAgentsController.StepCount) / (float) (2000));

            //if (debug)
            //Debug.Log("(" + simulation.name + ") " + "Dragon is dead, AI's win! StepCount [" + mlAgentsController.StepCount + "] finalReward [" + finalReward + "]");

            mlAgentsController.AddReward(finalReward);
            mlAgentsController.EndEpisode();
        }
        else
        {
            if (debug)
            {
                Debug.Log("(" + simulation.name + ") " + "Episode unfinished : " + (!oneAlive) + " | " + mlAgentsController.dragon.isDead);

                foreach (Entity e in characterTeam)
                {
                    if (!e.isDead)
                    {
                        Debug.Log("(" + simulation.name + ") " + e.name + " is NOT dead!");
                    }
                }
            }
        }
    }

    // Update is called once per frame
    public void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Printing teams...");
            foreach (KeyValuePair<ushort, List<Entity>> entry in entitiesTeamList)
            {
                string teamOutput = "Team #" + entry.Key + ": ";
                foreach(Entity e in entry.Value)
                {
                    teamOutput += e.name + ", ";
                }

                Debug.Log(teamOutput);
            }
        }*/

        CheckForEndOfEpisode();
    }

    public List<Entity> GetTeamMembers(Entity thisEntity) 
    {
        List<Entity> ret = null;
        if (thisEntity is Character)
        {
             ret = new();

            foreach(Character character in characterTeam)
            {
                if (character != thisEntity)
                    ret.Add(character);
            }
        }

        return ret;
    }
}
