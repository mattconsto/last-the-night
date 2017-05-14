using UnityEngine;

[System.Serializable]
public class GenerationConfig {
	/* Noise Settings */
	[Range(1, 500)]
	public int resolution = 129;
	public Vector2 scale = new Vector2(20, 20);
	[Range(1, 8)]
	public int octaves = 1;
	[Range(0, 1)]
	public float persistance = 1f;
	[Range(0, 20)]
	public float lacunarity = 1f;
	public AnimationCurve curve;

	/* Resources */
	public Material material;
	public TerrainType[] regions;
	public LODInfo[] lods;
	public GameObject[] treePrefabs;
	public GameObject[] flowerPrefabs;
	public GameObject[] monsterPrefabs;
	public GameObject[] structurePrefabs;

	public ChunkGenerator generator;
}