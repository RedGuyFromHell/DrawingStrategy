using System.Collections.Generic;
using UnityEngine;
using System;
using Pathfinding;

public class UnitsManager : MonoBehaviour
{
    public static List<Unit> Agent = new List<Unit>();
    public static int playerTroopsCount;
    public static int botTroopsCount;

    public bool doJob = false;

    private void Update()
    {
        if (doJob)
            ManageWithoutJob();
    }

    public int enemyCount;
    public int allyCount;
    void ManageWithoutJob ()
    {
        enemyCount = 0;
        allyCount = 0;

        for (int i = 0; i < Agent.Count; i ++)
        {
            float minDistance = 1000;
            int minIndex = i;

            if (Agent[i].type == 0)
                allyCount++;
            else
                enemyCount++;

            if (!Agent[i].isInCombat && !Agent[i].isEngaging)
                for (int j = 0; j < Agent.Count; j++)
                    if (i != j)
                        if (CalculateDistance(Agent[i].transform.position, Agent[j].transform.position) < minDistance && Agent[i].type != Agent[j].type)
                        {
                            minDistance = CalculateDistance(Agent[i].transform.position, Agent[j].transform.position);
                            minIndex = j;
                        }

            if (minIndex != i)
                SetUnitTarget(i, minIndex);
        }
    }

    void SetUnitTarget (int unitIndex, int targetIndex)
    {
        Agent[unitIndex].gameObject.GetComponent<AIDestinationSetter>().target = Agent[targetIndex].transform;
        Agent[unitIndex].target = Agent[targetIndex];
        Agent[unitIndex].isEngaging = true;

        Agent[unitIndex].transform.LookAt(Agent[targetIndex].transform.position);
    }

    float CalculateDistance(Vector3 pos1, Vector3 pos2)
    {
        double a = Math.Pow(pos1.x - pos2.x, 2);
        double b = Math.Pow(pos1.z - pos2.z, 2);
        return (float)Math.Sqrt(a + b);
    }
}
