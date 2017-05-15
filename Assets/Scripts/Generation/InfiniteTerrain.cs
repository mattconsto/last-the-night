using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteTerrain : MonoBehaviour {
	public static float viewDistance;
	public Transform viewer;
	public static Vector2 viewerPosition;
	public Vector2 lastCoord = new Vector2(Mathf.Infinity, Mathf.Infinity);
	public int chunkSize;
	public int chunksVisble;

	public GenerationConfig config;

	private Dictionary<Vector2, TerrainChunk> chunks = new Dictionary<Vector2, TerrainChunk>();
	private List<TerrainChunk> lastChunks = new List<TerrainChunk>();

	private System.Random rng = new System.Random();
	private float _spawnTimer = -1;
	private bool _firstRun = true;

	public void Start() {
		enabled = false;
		viewDistance = config.lods[config.lods.Length-1].threshold;
		chunkSize = config.resolution - 1;
		chunksVisble = Mathf.CeilToInt(viewDistance / chunkSize);
	}

	public void Update() {
		if(_firstRun) {
			rng = new System.Random(config.seed);

			config.scale.y = rng.NextFloat(20, 100);
			config.octaves = rng.Next(2, 6);
			config.persistance = rng.NextFloat(0.2f, 0.6f);
			config.lacunarity = rng.NextFloat(1.5f, 3.0f);

			Debug.Log("Pruning prefabs");
			// Pick our prefabs
			for(int i = (config.treePrefabs.Length - 1) / 2; i > 0; i--) {
				config.treePrefabs = config.treePrefabs.RemoveAt(rng.Next(config.treePrefabs.Length));
			}

			for(int i = (config.flowerPrefabs.Length - 1) / 2; i > 0; i--) {
				config.flowerPrefabs = config.flowerPrefabs.RemoveAt(rng.Next(config.flowerPrefabs.Length));
			}

			for(int i = (config.monsterPrefabs.Length - 1) / 2; i > 0; i--) {
				config.monsterPrefabs = config.monsterPrefabs.RemoveAt(rng.Next(config.monsterPrefabs.Length));
			}

			for(int i = (config.structurePrefabs.Length - 1) / 2; i > 0; i--) {
				config.structurePrefabs = config.structurePrefabs.RemoveAt(rng.Next(config.structurePrefabs.Length));
			}

			Debug.Log("Picking colors");
			List<TerrainType> regions = new List<TerrainType>();
			float threshold = 0;
			while(threshold < 2) {
				threshold += rng.NextFloat(0.01f, 0.3f);
				regions.Add(new TerrainType("", threshold, rng.ColorHSV(0, 1, 0.25f, 0.75f, 0.5f, 1)));
			}
			config.regions = regions.ToArray();
			for(int i = 0; i < config.materials.Length; i++) {
				config.materials[i] = new Material(config.materials[i]);
				config.materials[i].color = rng.ColorHSV(0, 1, 0.25f, 0.75f, 0.5f, 1);
			}

			_firstRun = false;
		}

		viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
		UpdateChunks();

		_spawnTimer -= Time.deltaTime;

		// Wandering Monster
		if(_spawnTimer < 0) {
			Debug.Log("Wandering Monster!");
			_spawnTimer = rng.NextFloat(10, 20);

			for(int i = 0; i < 1; i++) {
				GameObject player = GetComponent<GameController>().player.gameObject;

				GameObject monster = Instantiate(config.monsterPrefabs[rng.Next(config.monsterPrefabs.Length)]);
				monster.transform.parent = config.monsterContainer.transform;
				monster.transform.position = player.transform.position + new Vector3(
					(rng.NextBool() ? 1 : -1) * rng.NextFloat(20, 100),
					rng.NextFloat(0, 20),
					(rng.NextBool() ? 1 : -1) * rng.NextFloat(20, 100)
				);
				monster.transform.rotation = Quaternion.Euler(0, rng.NextFloat(360), 0);
			}
		}
	}

	public void UpdateChunks() {
		Vector2 currentChunkCoord = new Vector2(
			Mathf.RoundToInt(viewerPosition.x / (chunkSize - 2*2*4)),
			Mathf.RoundToInt(viewerPosition.y / (chunkSize - 2*2*4))
		);
		
		if(lastCoord != currentChunkCoord) {
			Debug.Log("Update chunks.");

			for(int i = 0; i < lastChunks.Count; i++) lastChunks[i].visible = false;
			lastChunks.Clear();

			for(int y = -chunksVisble; y <= chunksVisble; y++) {
				for(int x = -chunksVisble; x <= chunksVisble; x++) {
					Vector2 currentChunk = currentChunkCoord + new Vector2(x, y);

					if(chunks.ContainsKey(currentChunk)) {
						chunks[currentChunk].UpdateChunk();
						if(chunks[currentChunk].visible)
							lastChunks.Add(chunks[currentChunk]);
					} else {
						TerrainChunk chunk = ScriptableObject.CreateInstance<TerrainChunk>();
						chunk.init(currentChunk, chunkSize, gameObject, config);
						chunks.Add(currentChunk, chunk);
					}
				}
			}
		}

		lastCoord = currentChunkCoord;
	}
}
