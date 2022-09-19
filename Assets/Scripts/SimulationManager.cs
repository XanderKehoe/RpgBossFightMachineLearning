using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class SimulationManager : MonoBehaviour
{
    /* Todo List
     *      - Particles from abilities (toggle option for training)
     *      - Make MlAgentsController also control character movement
     *          * Removed automated advancement to target if out of ability range, just do nothing instead.
     */

    public GameObject simulation;

    public List<Character> characterTeam = new List<Character>();

    public Dragon dragon;

    public MlAgentsController mlAgentsController;

    public Terrain terrain;

    private const int EXPECTED_CHARACTER_COUNT = 5;
    public float MAP_BORDER_SIZE = 10; // Treat as constant, but not actually set as one to make it easier to adjust in editor.

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
        else
        {
            int expectedContinuousActionsLength = EXPECTED_CHARACTER_COUNT * 2;
            int expectedDiscreteActionsLength = EXPECTED_CHARACTER_COUNT;

            ActionBuffers actionBuffers = mlAgentsController.GetStoredActionBuffers();
            if (actionBuffers.ContinuousActions.Length != expectedContinuousActionsLength)
            {
                Debug.LogError("(" + simulation.name + ") GameManager::Start() - mlAgents controller's ContinuousActions buffer is incorrect size. Expected ["+ expectedContinuousActionsLength+"] Actual ["+ actionBuffers.ContinuousActions.Length+"]");
            }
            if (actionBuffers.DiscreteActions.Length != expectedDiscreteActionsLength)
            {
                Debug.LogError("(" + simulation.name + ") GameManager::Start() - mlAgents controller's DiscreteActions buffer is incorrect size.  Expected [" + expectedDiscreteActionsLength + "] Actual [" + actionBuffers.DiscreteActions.Length + "]");
            }
        }

        if (characterTeam.Count != EXPECTED_CHARACTER_COUNT)
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
        bool anyDead = false;
        foreach (Entity e in characterTeam)
        {
            if (e.isDead)
            {
                anyDead = true;
                break;
            }
        }

        if (anyDead)
        {
            mlAgentsController.GiveRewardWithTracking(MlAgentsController.REWARD_CHARACTER_DEATH, "REWARD_CHARACTER_DEATH");
            mlAgentsController.EndEpisode();

            if (debug)
                Debug.Log("(" + simulation.name + ") " + "No AI's alive, dragon wins...");
        }
        else if (mlAgentsController.dragon.isDead)
        {
            // Calculate finalReward based upon defined reward & how fast they defeated the dragon.
            float finalReward = MlAgentsController.REWARD_KILL_BOSS / ((float) (mlAgentsController.StepCount) / (float) (2000));

            if (debug)
                Debug.Log("(" + simulation.name + ") " + "Dragon is dead, AI's win! StepCount [" + mlAgentsController.StepCount + "] finalReward [" + finalReward + "]");

            mlAgentsController.GiveRewardWithTracking(finalReward, "REWARD_KILL_BOSS");
            mlAgentsController.EndEpisode();
        }
        else
        {
            if (debug)
            {
                Debug.Log("(" + simulation.name + ") " + "Episode unfinished : " + (!anyDead) + " | " + mlAgentsController.dragon.isDead);

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

    // Translates the -1 to +1 range of inputs from the mlAgentsController's ContinuousActions buffer into world coordinates
    public Vector3 ConvertNormalizedLocalPositionToWorldPosition(Vector2 input)
    {
        // input.y is actually the z coordinate here.
        Vector3 retVal = new();

        if (input.x < -1 || input.x > +1) 
        {
            Debug.LogError("ConvertNormalizedLocalPositionToWorldPosition() - x value was invalid [" + input.x + "]");
            return retVal;
        }

        if (input.y < -1 || input.y > +1)
        {
            Debug.LogError("ConvertNormalizedLocalPositionToWorldPosition() - z value was invalid [" + input.y + "]");
            return retVal;
        }

        //Debug.Log("ConvertNormalizedLocalPositionToWorldPosition() input x/z: " + input.x + " | " + input.y);

        // adjust inputs to go from (-1, +1) range to (0, +2) range
        input.x += 1;
        input.y += 1;

        float terrainSizeX = terrain.terrainData.size.x;
        float terrainSizeZ = terrain.terrainData.size.z;

        float centerX = terrainSizeX / 2;
        float centerZ = terrainSizeZ / 2;

        retVal.x = (MAP_BORDER_SIZE / 2) + (centerX * input.x * ((terrainSizeX - (MAP_BORDER_SIZE)) / terrainSizeX));
        retVal.y = 0;
        retVal.z = (MAP_BORDER_SIZE / 2) + (centerZ * input.y * ((terrainSizeZ - (MAP_BORDER_SIZE)) / terrainSizeZ));

        //Debug.Log("ConvertNormalizedLocalPositionToWorldPosition() output x/z: " + retVal.x + " | " + retVal.z);

        // convert to local position
        retVal.x += this.transform.position.x;
        retVal.z += this.transform.position.z;

        return retVal;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(
            new Vector3(terrain.terrainData.size.x / 2 + transform.position.x, 0, terrain.terrainData.size.z / 2 + transform.position.z), 
            new Vector3(terrain.terrainData.size.x - MAP_BORDER_SIZE, 25, terrain.terrainData.size.z - MAP_BORDER_SIZE));

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(
            ConvertNormalizedLocalPositionToWorldPosition(new Vector2(-1, -1)),
            new Vector3(0.1f, 15, 0.1f));

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(
            ConvertNormalizedLocalPositionToWorldPosition(new Vector2(1, 1)),
            new Vector3(0.1f, 15, 0.1f));

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(
            ConvertNormalizedLocalPositionToWorldPosition(new Vector2(0, 0)),
            new Vector3(0.1f, 15, 0.1f));

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(
            ConvertNormalizedLocalPositionToWorldPosition(new Vector2(-0.5f, -0.5f)),
            new Vector3(0.1f, 15, 0.1f));

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(
            ConvertNormalizedLocalPositionToWorldPosition(new Vector2(0.5f, 0.5f)),
            new Vector3(0.1f, 15, 0.1f));
    }
}
