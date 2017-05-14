using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteTerrain : MonoBehaviour {
	public static float viewDistance;
	public Transform viewer;
	public static Vector2 viewerPosition;
	public int chunkSize;
	public int chunksVisble;

	public GenerationConfig config;

	private Dictionary<Vector2, TerrainChunk> chunks = new Dictionary<Vector2, TerrainChunk>();
	private List<TerrainChunk> lastChunks = new List<TerrainChunk>();

	public void Start() {
		viewDistance = config.lods[config.lods.Length-1].threshold;
		chunkSize = config.resolution - 1;
		chunksVisble = Mathf.CeilToInt(viewDistance / chunkSize);
		UpdateChunks();
	}

	public void Update() {
		viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
		UpdateChunks();
	}

	public void UpdateChunks() {
		Vector2 currentChunkCoord = new Vector2(
			Mathf.RoundToInt(viewerPosition.x / (chunkSize - 2*2*4)),
			Mathf.RoundToInt(viewerPosition.y / (chunkSize - 2*2*4))
		);

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
}
