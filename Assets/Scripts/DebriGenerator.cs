using System.Collections.Generic;
using UnityEngine;
using UnscriptedLogic;

public struct DebriGenSettings
{
    public int amount;
    public float verticalOffset;
    public DebriThemeSO debriTheme;
    public List<Vector3> pathlist;
    public List<Vector3> nodes;
    public Transform parent;
    public List<HeatMapNode> heatmap;
    public LayerMask debriLayer;

    public DebriGenSettings(int amount, float verticalOffset, DebriThemeSO debriTheme, List<Vector3> pathlist, List<Vector3> nodes, Transform parent, List<HeatMapNode> heatmap, LayerMask debriLayer)
    {
        this.amount = amount;
        this.verticalOffset = verticalOffset;
        this.debriTheme = debriTheme;
        this.pathlist = pathlist;
        this.nodes = nodes;
        this.parent = parent;
        this.heatmap = heatmap;
        this.debriLayer = debriLayer;
    }
}

public static class DebriGenerator
{
    public static void GenerateDebrisRandomly(DebriGenSettings settings)
    {
        for (int i = 0; i < settings.debriTheme.Debris.Count; i++)
        {
            Debri debri = settings.debriTheme.Debris[i];
            List<Vector3> possiblePositions = new List<Vector3>();
            if (debri.heatmapRange.x >= 0)
            {
                for (int j = 0; j < settings.heatmap.Count; j++)
                {
                    if (MathLogic.isWithinRange(settings.heatmap[j].shortestDistance, debri.heatmapRange))
                    {
                        possiblePositions.Add(settings.heatmap[j].position);
                    }
                }
            }

            int spawnCount = 0;
            for (int j = 0; j < 50; j++)
            {
                TryPutDebri(settings, debri, possiblePositions.Count > 0 ? RandomLogic.FromList(possiblePositions) : RandomLogic.FromList(settings.heatmap).position, out bool passed);
                if (passed)
                {
                    spawnCount++;
                    if (spawnCount >= debri.mustContain)
                    {
                        break;
                    }
                }
            }
        }
    }

    private static void TryPutDebri(DebriGenSettings settings, Debri debri, Vector3 position, out bool passed)
    {
        passed = IsClearOfPath(settings, debri, position);
        if (passed)
        {
            GameObject debriObject = Object.Instantiate(debri.prefab, position + Vector3.up * settings.verticalOffset, Quaternion.identity, settings.parent);

            if (debri.faceClosestPath)
            {
                HeatMapNode heatMapNode = settings.heatmap.Find(x => x.position == position);
                debriObject.transform.forward = (heatMapNode.closestPathPosition - heatMapNode.position).normalized;
            }
            else
            {
                debriObject.transform.forward = RandomLogic.VectorDirAroundY();
            }
        }
    }

    public static bool IsClearOfPath(DebriGenSettings settings, Debri debri, Vector3 position)
    {
        if (Physics.Raycast(position, Vector3.up, out RaycastHit hitinfo, 1f, settings.debriLayer))
        {
            if (hitinfo.collider != null)
            {
                return false;
            }
        }

        if (debri.heatmapRange.x >= 0)
        {
            HeatMapNode heatMapNode = settings.heatmap.Find(x => x.position == position);
            if (heatMapNode == null)
            {
                //Debug.Log($"Cant find heatmap node: {position}");
                return false;
            }

            if (!MathLogic.isWithinRange(heatMapNode.shortestDistance, debri.heatmapRange))
            {
                //Debug.Log("Within heatmap failed");
                return false;
            }
        }

        if (Physics.CheckSphere(position, debri.size, settings.debriLayer))
        {
            //Debug.Log("Away from other debri failed");
            return false;
        }

        return true;
    }
}