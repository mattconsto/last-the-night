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

	private System.Random rng;
	private float _spawnTimer = -1;

	public void Start() {
		viewDistance = config.lods[config.lods.Length-1].threshold;
		chunkSize = config.resolution - 1;
		chunksVisble = Mathf.CeilToInt(viewDistance / chunkSize);
		rng = new System.Random(config.seed);
		enabled = false;
	}

	public void Update() {
		viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
		UpdateChunks();

		_spawnTimer -= Time.deltaTime;

		// Wandering Monster
		if(_spawnTimer < 0) {
			Debug.Log("Wandering Monster!");
			_spawnTimer = rng.NextFloat(10, 30);

			for(int i = 0; i < 1; i++) {
				GameObject player = GetComponent<GameController>().player.gameObject;

				GameObject monster = Instantiate(config.monsterPrefabs[rng.Next(config.monsterPrefabs.Length)]);
				monster.transform.parent = config.monsterContainer.transform;
				monster.transform.position = player.transform.position + new Vector3(rng.NextFloat(-20, 20), rng.NextFloat(0, 10), rng.NextFloat(-20, 20));
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
			print("Update chunks.");

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
