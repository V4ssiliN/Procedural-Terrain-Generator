using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;
        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            {
                mapGen.DrawMapInEditor();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            mapGen.DrawMapInEditor();
        }
        if (GUILayout.Button("Unit Score"))
        {
            MapData mapData = mapGen.GenerateMapData(Vector2.zero);

            int[,] U = PlayabilityScore.CalculateUnitMap(mapData.heightMap, mapGen.unitThreshold);
            Debug.Log(PlayabilityScore.CalculateUnitScore(U));
        }
        if (GUILayout.Button("Building Score"))
        {
            MapData mapData = mapGen.GenerateMapData(Vector2.zero);

            int[,] U = PlayabilityScore.CalculateUnitMap(mapData.heightMap, mapGen.unitThreshold);
            Debug.Log(PlayabilityScore.CalculateBuildingScore(mapData.heightMap, U, mapGen.buildingThreshold, mapGen.buildingSize));
        }
        if (GUILayout.Button("Erosion Score"))
        {
            MapData mapData = mapGen.GenerateMapData(Vector2.zero);
            Debug.Log(PlayabilityScore.CalculateErosionScore(mapData.heightMap));
        }
        if (GUILayout.Button("Detail Score"))
        {
            MapData mapData = mapGen.GenerateMapData(Vector2.zero);
            Debug.Log(PlayabilityScore.CalculateDetailScore(mapData.heightMap));
        }
        if (GUILayout.Button("Playability Score"))
        {
            MapData mapData = mapGen.GenerateMapData(Vector2.zero);
            Debug.Log(PlayabilityScore.CalculatePlayabilityScore(mapData.heightMap, mapGen.unitThreshold, mapGen.buildingThreshold, mapGen.buildingSize)[0]);
        }
    }
}
