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
	public GameObject treeContainer;
	public GameObject grassContainer;
	public GameObject structureContainer;
	public GameObject tapeContainer;
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

	public static GameObject CreateGameObjectWithParent(string name, GameObject parent) {
		GameObject go = new GameObject(name);
		go.transform.parent = parent.transform;
		//go.isStatic = true;
		return go;
	}

	public void init(Vector2 coord, int size, GameObject parent, GenerationConfig config) {
		this.config = config;

		position = coord * (size - 2*2*4);
		bounds = new Bounds(position, Vector3.one * (size + 2*2*4));
		lodMeshs = new LODMesh[config.lods.Length];
		for(int i = 0; i < config.lods.Length; i++) lodMeshs[i] = new LODMesh(config.lods[i].lod, config);

		gameObject         = CreateGameObjectWithParent("Terrain Chunk", parent);
		treeContainer      = CreateGameObjectWithParent("Tree Container", gameObject);
		grassContainer     = CreateGameObjectWithParent("Grass Container", gameObject);
		structureContainer = CreateGameObjectWithParent("Structure Container", gameObject);
		tapeContainer      = CreateGameObjectWithParent("Tape Container", gameObject);
		gameObject.transform.position = new Vector3(position.x, 0, position.y);
		gameObject.tag = "Jumpable";

		_mf = gameObject.AddComponent<MeshFilter>();
		_mr = gameObject.AddComponent<MeshRenderer>();
		_mc = gameObject.AddComponent<MeshCollider>();
		_mr.material = new Material(config.material);

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

		UpdateChunk();

		System.Random rng = new System.Random((int) (bounds.center.x * bounds.center.z + config.seed));

		for(int i = 0; i < 16; i++) {
			int y = rng.Next(map.noise.GetLength(0)), x = rng.Next(map.noise.GetLength(1));

			if(map.noise[y, x] > 0.6f && map.noise[y, x] < 0.8f) {
				GameObject tree = Instantiate(config.treePrefabs[rng.Next(config.treePrefabs.Length)]);
				tree.transform.parent = treeContainer.transform;
				tree.transform.position = new Vector3(
					bounds.center.x + 0 - bounds.size.x * ((float) (map.noise.GetLength(1) - y) / map.noise.GetLength(1) - 0.5f),
					config.curve.Evaluate(map.noise[y, x]) * config.scale.y / 1.5f,
					bounds.center.y + bounds.size.y * ((float) (map.noise.GetLength(0) - x) / map.noise.GetLength(0) - 1.5f) // Not sure why it needs to be minus 1
				);
				tree.transform.rotation = Quaternion.Euler(0, rng.NextFloat(360), 0);
			}
		}

		for(int i = 0; i < 8; i++) {
			int y = rng.Next(map.noise.GetLength(0)), x = rng.Next(map.noise.GetLength(1));

			if(map.noise[y, x] > 0.5f && map.noise[y, x] < 0.85f) {
				GameObject grass = Instantiate(config.flowerPrefabs[rng.Next(config.flowerPrefabs.Length)]);
				grass.transform.parent = grassContainer.transform;
				grass.transform.position = new Vector3(
					bounds.center.x + 0 - bounds.size.x * ((float) (map.noise.GetLength(1) - y) / map.noise.GetLength(1) - 0.5f),
					config.curve.Evaluate(map.noise[y, x]) * config.scale.y / 1.25f,
					bounds.center.y + bounds.size.y * ((float) (map.noise.GetLength(0) - x) / map.noise.GetLength(0) - 1.5f) // Not sure why it needs to be minus 1
				);
				grass.transform.rotation = Quaternion.Euler(0, rng.NextFloat(360), 0);
			}
		}

		{
			int y = rng.Next(map.noise.GetLength(0)), x = rng.Next(map.noise.GetLength(1));

			if(map.noise[y, x] > 0.4f && map.noise[y, x] < 0.6f) {
				GameObject structure = Instantiate(config.structurePrefabs[rng.Next(config.structurePrefabs.Length)]);
				structure.transform.parent = structureContainer.transform;
				structure.transform.position = new Vector3(
					bounds.center.x + 0 - bounds.size.x * ((float) (map.noise.GetLength(1) - y) / map.noise.GetLength(1) - 0.5f),
					config.curve.Evaluate(map.noise[y, x]) * config.scale.y,
					bounds.center.y + bounds.size.y * ((float) (map.noise.GetLength(0) - x) / map.noise.GetLength(0) - 1.5f) // Not sure why it needs to be minus 1
				);
				structure.transform.rotation = Quaternion.Euler(0, rng.NextFloat(360), 0);
			}
		}

		// Don't spawn monsters on top of players
		if(rng.NextFloat() < 0.5f + config.difficulty * 0.25f && Mathf.Sqrt(bounds.SqrDistance(InfiniteTerrain.viewerPosition)) > 400) {
			int y = rng.Next(map.noise.GetLength(0)), x = rng.Next(map.noise.GetLength(1));

			if(map.noise[y, x] > 0.4f && map.noise[y, x] < 0.85f) {
				GameObject monster = Instantiate(config.monsterPrefabs[rng.Next(config.monsterPrefabs.Length)]);
				monster.transform.parent = config.monsterContainer.transform;
				monster.transform.position = new Vector3(
					bounds.center.x + 0 - bounds.size.x * ((float) (map.noise.GetLength(1) - y) / map.noise.GetLength(1) - 0.5f),
					config.curve.Evaluate(map.noise[y, x]) * config.scale.y * 2,
					bounds.center.y + bounds.size.y * ((float) (map.noise.GetLength(0) - x) / map.noise.GetLength(0) - 1.5f) // Not sure why it needs to be minus 1
				);
				monster.transform.rotation = Quaternion.Euler(0, rng.NextFloat(360), 0);
			}
		}

		if(rng.NextFloat() < 0.05f && config.tapePrefabs.Length > 0) {
			int y = rng.Next(map.noise.GetLength(0)), x = rng.Next(map.noise.GetLength(1));

			int position = rng.Next(config.tapePrefabs.Length);
			GameObject tape = Instantiate(config.tapePrefabs[position]);
			config.tapePrefabs = config.tapePrefabs.RemoveAt(position);
			tape.transform.parent = tapeContainer.transform;
			tape.transform.position = new Vector3(
				bounds.center.x + 0 - bounds.size.x * ((float) (map.noise.GetLength(1) - y) / map.noise.GetLength(1) - 0.5f),
				config.curve.Evaluate(map.noise[y, x]) * config.scale.y * 2,
				bounds.center.y + bounds.size.y * ((float) (map.noise.GetLength(0) - x) / map.noise.GetLength(0) - 1.5f) // Not sure why it needs to be minus 1
			);
			tape.transform.rotation = Quaternion.Euler(0, rng.NextFloat(360), 0);
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
				grassContainer.SetActive(config.lods[index].lod <= 4);
				treeContainer.SetActive(config.lods[index].lod <= 8);
				structureContainer.SetActive(config.lods[index].lod <= 16);
				LODMesh mesh = lodMeshs[index];
				if(mesh.recieved) {
					previousLOD = index;
					_mf.sharedMesh = mesh.mesh;
					_mc.sharedMesh = mesh.mesh;
				} else if(!mesh.requested) {
					mesh.Request(UpdateChunk, map);
				}
			}
		}
	}
}