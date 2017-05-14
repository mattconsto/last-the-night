using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk : ScriptableObject {
	public Vector2 position;
	public Bounds bounds;
	public int previousLOD = -1;
	public LODMesh[] lodMeshs;
	public MapData map;
	public GameObject gameObject;
	public bool recieved = false;

	private bool _visible;
	private MeshFilter   _mf;
	private MeshRenderer _mr;
	private MeshCollider _mc;

	private GenerationConfig config;

	public bool visible {
		get {return _visible;}
		set {_visible = value; gameObject.SetActive(_visible);}
	}

	public void init(Vector2 coord, int size, Transform parent, GenerationConfig config) {
		this.config = config;

		position = coord * (size - 2*2*4);
		bounds = new Bounds(position, Vector3.one * (size + 2*2*4));

		lodMeshs = new LODMesh[config.lods.Length];
		for(int i = 0; i < config.lods.Length; i++) lodMeshs[i] = new LODMesh(config.lods[i].lod, config);

		gameObject = new GameObject("Terrain Chunk");
		gameObject.tag = "Jumpable";
		_mf = gameObject.AddComponent<MeshFilter>();
		_mr = gameObject.AddComponent<MeshRenderer>();
		_mc = gameObject.AddComponent<MeshCollider>();
		_mr.material = new Material(config.material);
		gameObject.transform.position = new Vector3(position.x, 0, position.y);
		gameObject.transform.parent = parent;
		visible = false;

		config.generator.RequestMapData(OnMapReceived, config, position / size);
	}

	public void OnMapReceived(MapData map) {
		this.map = map;
		recieved = true;
		
		// Create texture
		Texture2D texture = new Texture2D (map.noise.GetLength(0), map.noise.GetLength(1));
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels(map.color);
		texture.Apply();

		_mr.sharedMaterial.mainTexture = texture;

		System.Random rng = new System.Random((int) (Constants.seed * bounds.center.x * bounds.center.z));

		for(int i = 0; i < 20; i++) {
			int y = rng.Next(map.noise.GetLength(0)), x = rng.Next(map.noise.GetLength(1));

			if(map.noise[y, x] > 0.6f && map.noise[y, x] < 0.8f) {
				GameObject tree = Instantiate(config.treePrefabs[rng.Next(config.treePrefabs.Length)]);
				tree.transform.parent = gameObject.transform;
				tree.transform.position = new Vector3(
					bounds.center.x + 0 - bounds.size.x * ((float) (map.noise.GetLength(1)-y) / map.noise.GetLength(1) - 0.5f),
					config.curve.Evaluate(map.noise[y, x]) * 40, // TODO remove horrible magic number
					bounds.center.y + bounds.size.y * ((float) (map.noise.GetLength(0)-x) / map.noise.GetLength(0) - 1.5f) // Not sure why it needs to be minus 1
				);
			}
		}
	}

	public void UpdateChunk() {
		if(!recieved) return;

		float distanceFromEdge = Mathf.Sqrt(bounds.SqrDistance(InfiniteTerrain.viewerPosition));
		visible = distanceFromEdge <= InfiniteTerrain.viewDistance;

		if(visible) {
			int index = 0;
			for(int i = 0; i < config.lods.Length; i++) {
				if(distanceFromEdge > config.lods[i].threshold) {
					index = i + 1;
				} else break;
			}

			if(index != previousLOD) {
				LODMesh mesh = lodMeshs[index];
				if(mesh.recieved) {
					previousLOD = index;
					_mf.sharedMesh = mesh.mesh;
					_mc.sharedMesh = mesh.mesh;
				} else if(!mesh.requested) {
					mesh.Request(map);
				}
			}
		}
	}
}