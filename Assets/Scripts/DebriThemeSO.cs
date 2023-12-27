using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Debri
{
    [Tooltip("The prefab of the debri to use")]
    public GameObject prefab;

    [Tooltip("The size in radius to avoid instantiating objects too close to other debris. Set to 0 for no restrictions")]
    public float size;

    [Tooltip("The value on the heatmap to place this debri. Set to -1 on minimum to ignore heatmap")]
    public Vector2 heatmapRange;

    public bool faceClosestPath;

    [Tooltip("The amount that a map must contain. The algorithm will keep trying to find a spot.")]
    public int mustContain;
}

[CreateAssetMenu(fileName = "New Debri Theme", menuName = "ScriptableObjects/Create New Debri Theme")]
public class DebriThemeSO : ScriptableObject
{
    [SerializeField] private List<Debri> debris = new List<Debri>();

    public List<Debri> Debris => debris;
}