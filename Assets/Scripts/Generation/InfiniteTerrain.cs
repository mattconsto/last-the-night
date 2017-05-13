using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteTerrain : MonoBehaviour {
	public static float viewDistance;
	public Transform viewer;
	public Material material;

	public ChunkGenerator generator;
	public LODInfo[] lods;

	private Dictionary<Vector2, TerrainChunk> chunks = new Dictionary<Vector2, TerrainChunk>();
	private List<TerrainChunk> lastChunks = new List<TerrainChunk>();

	public static Vector2 viewerPosition;
	public int chunkSize;
	public int chunksVisble;

	public void Start() {
		viewDistance = lods[lods.Length-1].threshold;
		chunkSize = ChunkGenerator.resolution - 1;
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

		for(int i = 0; i < lastChunks.Count; i++) {
			lastChunks[i].visible = false;
		}
		lastChunks.Clear();

		for(int y = -chunksVisble; y <= chunksVisble; y++) {
			for(int x = -chunksVisble; x <= chunksVisble; x++) {
				Vector2 currentChunk = currentChunkCoord + new Vector2(x, y);

				if(chunks.ContainsKey(currentChunk)) {
					chunks[currentChunk].UpdateChunk();
					if(chunks[currentChunk].visible)
						lastChunks.Add(chunks[currentChunk]);
				} else {
					chunks.Add(currentChunk, new TerrainChunk(currentChunk, chunkSize, lods, gameObject.transform, material, generator));
				}
			}
		}
	}

	public class TerrainChunk {
		public Vector2 position;
		public GameObject gameObject;
		public Bounds bounds;
		public LODInfo[] lods;
		public int previousLOD = -1;
		public LODMesh[] lodMeshs;
		public MapData map;
		public bool recieved = false;

		private bool _visible;
		private MeshFilter   _mf;
		private MeshRenderer _mr;
		private MeshCollider _mc;
		private ChunkGenerator generator;

		public bool visible {
			get {return _visible;}
			set {_visible = value; gameObject.SetActive(_visible);}
		}

		public TerrainChunk(Vector2 coord, int size, LODInfo[] lods, Transform parent, Material material, ChunkGenerator generator) {
			this.position = coord * (size - 2*2*4);
			this.bounds = new Bounds(position, Vector3.one * (size + 2*2*4));
			this.lods = lods;
			this.lodMeshs = new LODMesh[lods.Length];

			for(int i = 0; i < lods.Length; i++) {
				lodMeshs[i] = new LODMesh(lods[i].lod, generator);
			}

			gameObject = new GameObject("Terrain Chunk");
			gameObject.tag = "Jumpable";
			_mf = gameObject.AddComponent<MeshFilter>();
			_mr = gameObject.AddComponent<MeshRenderer>();
			_mc = gameObject.AddComponent<MeshCollider>();
			_mr.material = new Material(material);
			gameObject.transform.position = new Vector3(position.x, 0, position.y);
			gameObject.transform.parent = parent;
			visible = false;

			this.generator = generator;
			this.generator.RequestMapData(OnMapReceived, position / size);
		}

		public void OnMapReceived(MapData map) {
			this.map = map;
			recieved = true;
			
			// Create texture
			Texture2D texture = new Texture2D (map.noise.GetLength(0), map.noise.GetLength(1));
			//texture.filterMode = FilterMode.Point;
			texture.wrapMode = TextureWrapMode.Clamp;
			texture.SetPixels(map.color);
			texture.Apply();

			_mr.sharedMaterial.mainTexture = texture;
		}

		public void OnMeshReceived(MeshData mesh) {
			_mf.sharedMesh = mesh.CreateMesh();
			_mc.sharedMesh = _mf.sharedMesh;
		}

		public void UpdateChunk() {
			if(!recieved) return;

			float distanceFromEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
			visible = distanceFromEdge <= viewDistance;

			if(visible) {
				int index = 0;
				for(int i = 0; i < lods.Length; i++) {
					if(distanceFromEdge > lods[i].threshold) {
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
}
