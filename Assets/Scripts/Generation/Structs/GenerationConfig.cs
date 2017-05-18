using System;
using UnityEngine;

[System.Serializable]
public class GenerationConfig : ICloneable {
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
	public int seed = 0;
	public float difficulty = 0;

	/* Resources */
	public Material material;
	public TerrainType[] regions;
	public LODInfo[] lods;
	public GameObject[] treePrefabs;
	public GameObject[] flowerPrefabs;
	public GameObject[] monsterPrefabs;
	public GameObject[] structurePrefabs;
	public GameObject[] tapePrefabs;
	public ChunkGenerator generator;
	public GameObject monsterContainer;
	public Material[] materials;

	public object Clone() {
		GenerationConfig config = new GenerationConfig();

		config.resolution       = this.resolution;
		config.scale            = this.scale;
		config.octaves          = this.octaves;
		config.persistance      = this.persistance;
		config.lacunarity       = this.lacunarity;
		config.curve            = new AnimationCurve(this.curve.keys);
		config.seed             = this.seed;
		config.difficulty       = this.difficulty;
		config.material         = new Material(this.material);
		config.regions          = (TerrainType[]) this.regions.Clone();
		config.lods             = (LODInfo[]) this.lods.Clone();
		config.treePrefabs      = (GameObject[]) this.treePrefabs.Clone();
		config.flowerPrefabs    = (GameObject[]) this.flowerPrefabs.Clone();
		config.monsterPrefabs   = (GameObject[]) this.monsterPrefabs.Clone();
		config.structurePrefabs = (GameObject[]) this.structurePrefabs.Clone();
		config.tapePrefabs      = (GameObject[]) this.tapePrefabs.Clone();
		config.generator        = this.generator;
		config.monsterContainer = this.monsterContainer;
		config.materials        = (Material[]) this.materials.Clone();

		return config;
	}
}