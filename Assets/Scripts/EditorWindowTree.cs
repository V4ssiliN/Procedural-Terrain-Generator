using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SceneManagement;

public class EditorWindowTree : EditorWindow
{
    private Texture2D noiseMapTexture;
    private GameObject prefab;
    private PlacementGenes genes;

    private static string GenesSaveName
    {
        get { return $"Alexandre_{Application.productName}_{EditorSceneManager.GetActiveScene().name}"; }
    }

    [MenuItem("Tools/Plant Placement")]
    public static void ShowWindow()
    {
        GetWindow<EditorWindowTree>("Plant Placement");
    }

    private void OnEnable()
    {
        genes = PlacementGenes.Load(GenesSaveName);
    }

    private void OnDisable()
    {
        PlacementGenes.Save(GenesSaveName, genes);
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        noiseMapTexture = (Texture2D)EditorGUILayout.ObjectField("Noise Map Texture", noiseMapTexture, typeof(Texture2D), false);
        if (GUILayout.Button("Generate Noise"))
        {
            int width = (int)Terrain.activeTerrain.terrainData.size.x;
            int depth = (int)Terrain.activeTerrain.terrainData.size.y;
            float scale = 5;
            noiseMapTexture = NoiseTree.GetNoiseMap(width, depth, scale);
        }
        EditorGUILayout.EndHorizontal();

        genes.maxHeight = EditorGUILayout.Slider("Max Height", genes.maxHeight, 0, 1000);
        genes.minHeight = EditorGUILayout.Slider("Min Height", genes.minHeight, 0, 1000);
        genes.maxSteepness = EditorGUILayout.Slider("Max Steepness", genes.maxSteepness, 0, 90);
        genes.density = EditorGUILayout.Slider("Density", genes.density, 0, 1);
        prefab = (GameObject)EditorGUILayout.ObjectField("Object Prefab", prefab, typeof(GameObject), false);

        if (GUILayout.Button("Place Object"))
        {
            Debug.Log(Terrain.activeTerrain.terrainData.GetHeight(163, 35));
            PlaceObjects(Terrain.activeTerrain, noiseMapTexture, genes, prefab);
        }
    }

    public static void PlaceObjects(Terrain terrain, Texture2D noiseMapTexture, PlacementGenes genes, GameObject prefab)
    {
        Transform parent = new GameObject("PlaceObjects").transform;

        for (int x= 0; x <terrain.terrainData.size.x; x+=1)
        {
            for (int z = 0; z < terrain.terrainData.size.z; z+=1)
            {
                
                if (Fitness(terrain,noiseMapTexture,genes,x,z) > 1 - genes.density)
                {
                    Vector3 pos = new Vector3(x + UnityEngine.Random.Range(-0.5f,0.5f), 1, z + UnityEngine.Random.Range(-0.5f, 0.5f));
                    pos.y = terrain.SampleHeight(new Vector3(x,0,z));
                    GameObject go = Instantiate(prefab, pos, Quaternion.identity);
                    go.transform.SetParent(parent);
                }
            }
        }
    }
    public static float Fitness(Terrain terrain, Texture2D noiseMapTexture, PlacementGenes genes, int x, int z)
    {
        float fitness = noiseMapTexture.GetPixel(x, z).g;

        fitness += UnityEngine.Random.Range(-0.25f, 0.25f);
        float steepness =  terrain.terrainData.GetSteepness(x/terrain.terrainData.size.x, z/terrain.terrainData.size.z);
        if (steepness > genes.maxSteepness)
        {
            fitness = 0f;
        }
        float height = terrain.terrainData.GetHeight(x, z);
        if (height > genes.maxHeight || height < genes.minHeight)
        {
            fitness = 0f;
        }
        return fitness;
    }
    

    [Serializable] 
    public struct PlacementGenes
    {
        public float density;
        public float maxHeight;
        public float minHeight;
        public float maxSteepness;

        internal static PlacementGenes Load(string saveName)
        {
            PlacementGenes genes;
            string saveData = EditorPrefs.GetString(saveName);

            if (string.IsNullOrEmpty(saveData))
            {
                genes = new PlacementGenes();
                genes.density = 0.5f;
                genes.maxHeight = 100;
                genes.minHeight = 5;
                genes.maxSteepness = 25;
            }else
            {
                genes = JsonUtility.FromJson<PlacementGenes>(saveData);
            }
            return genes;
        }
        internal static void Save(string savename, PlacementGenes genes)
        {
            EditorPrefs.SetString(savename,JsonUtility.ToJson(genes));
        }

    }

}
