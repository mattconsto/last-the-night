using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class SimplexTexture : MonoBehaviour {
	[Range(1, 500)]
	public const int resolution = 65;
	public Vector2 scale = new Vector2(20, 20);
	[Range(1, 8)]
	public int octaves = 1;
	[Range(0, 1)]
	public float persistance = 1f;
	[Range(0, 20)]
	public float lacunarity = 1f;
	public int seed = 0;

	public AnimationCurve curve;

	public enum DrawMode {Noise, Color};
	public DrawMode mode = DrawMode.Color;

	public bool update = true;

	public TerrainType[] regions;

	public Queue<ThreadData<MapData>> mapQueue = new Queue<ThreadData<MapData>>();
	public Queue<ThreadData<MeshData>> meshQueue = new Queue<ThreadData<MeshData>>();

	public float[,] Calc2D(int width, int height, float scale, Vector2 center) {
		System.Random rng = new System.Random(seed);
		Vector2[] offsets = new Vector2[octaves];
		for(int o = 0; o < octaves; o++) offsets[o] = new Vector2(rng.Next(10000, 20000), rng.Next(10000, 20000)) + center;

		float[,] values = new float[width, height];
		for(int i = 0; i < width; i++) {
			for(int j = 0; j < height; j++) {
				float amplitude = 1;
				float frequency = 1;

				for(int o = 0; o < octaves; o++) {
					values[i, j] += Simplex.Noise.Generate(
						(i - width  / 2) / scale * frequency + offsets[o].x,
						(j - height / 2)/ scale * frequency + offsets[o].y
					) * amplitude;

					amplitude *= persistance;
					frequency *= lacunarity;
				}
			}
		}

		// Normalise
		float maximum = values.Cast<float>().Max(), minimum = values.Cast<float>().Min();
		for(int i = 0; i < width; i++) {
			for(int j = 0; j < height; j++) {
				values[i,j] = Mathf.InverseLerp(minimum, maximum, values[i,j]);
			}
		}

		return values;
	}

	public void DrawNoise(float[,] noise, Color[] colors) {
		for(int y = 0; y < resolution; y++) {
			for(int x = 0; x < resolution; x++) {
				colors[y * resolution + x] = Color.Lerp(Color.black, Color.white, noise[x,y]);
			}
		}
	}

	public void DrawColor(float[,] noise, Color[] colors) {
		for(int y = 0; y < resolution; y++) {
			for(int x = 0; x < resolution; x++) {
				for(int i = 0; i < regions.Length; i++) {
					if(noise[x,y] <= regions[i].threshold) {
						colors[y * resolution + x] = regions[i].color;
						break;
					}
				}
			}
		}
	}

	public MeshData GenerateMesh(float[,] noise, int lod) {
		int width = noise.GetLength(0);
		int height = noise.GetLength(1);
		Vector2 vertOffset = new Vector2((width - 1) / -2f, (height - 1) / -2f);

		int increment = (lod == 0) ? 1 : lod * 2;
		int resolution = (width - 1) / increment + 1;

		MeshData meshData = new MeshData(resolution, resolution);
		int vertIndex = 0;

		for(int y = 0; y < height; y += increment) {
			for(int x = 0; x < width; x += increment) {
				meshData.vertices[vertIndex] = new Vector3(vertOffset.x + x, curve.Evaluate(noise[x,y]) * scale.y, vertOffset.y - y);
				meshData.uvs[vertIndex] = new Vector2(x / (float) width, y / (float) height);

				if(x < width - increment && y < height - increment) {
					meshData.AddTriangle(vertIndex, vertIndex+resolution+1, vertIndex+resolution);
					meshData.AddTriangle(vertIndex+resolution+1, vertIndex, vertIndex+1);
				}

				vertIndex++;
			}
		}

		return meshData;
	}

	public void RequestMapData(Action<MapData> callback, Vector2 center) {
		ThreadStart threadStart = delegate {MapDataThread(callback, center);};
		new Thread(threadStart).Start();
	}

	public void MapDataThread(Action<MapData> callback, Vector2 center) {
		lock (mapQueue) {
			MapData data = GenerateMapData(center);
			mapQueue.Enqueue(new ThreadData<MapData>(callback, data));
		}
	}

	public void RequestMeshData(MapData map, int lod, Action<MeshData> callback) {
		ThreadStart threadStart = delegate {MeshDataThread(map, lod, callback);};
		new Thread(threadStart).Start();
	}

	public void MeshDataThread(MapData map, int lod, Action<MeshData> callback) {
		lock (meshQueue) {
			MeshData data = GenerateMeshData(map, lod);
			meshQueue.Enqueue(new ThreadData<MeshData>(callback, data));
		}
	}

	public MapData GenerateMapData(Vector2 center) {
		// Generate noise
		float[,] noise = Calc2D(resolution, resolution, scale.x, center);

		// Color it
		Color[] colors = new Color[resolution * resolution];

		switch(mode) {
			case DrawMode.Noise: DrawNoise(noise, colors); break;
			case DrawMode.Color: DrawColor(noise, colors); break;
		}

		return new MapData(noise, colors);
	}

	public MeshData GenerateMeshData(MapData map, int lod) {
		// Generate noise
		return GenerateMesh(map.noise, lod);
	}

	/*public void OnMapRecieved(MapData map) {
		GetComponent<MeshFilter>().sharedMesh = map.mesh.CreateMesh();
		GetComponent<MeshCollider>().sharedMesh = GetComponent<MeshFilter>().sharedMesh;
		
		// Create texture
		Texture2D texture = new Texture2D (resolution, resolution);
		//texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels(map.color);
		texture.Apply();

		GetComponent<MeshRenderer>().sharedMaterial.mainTexture = texture;
	}*/

	public void Update() {
		for(int i = 0; i < mapQueue.Count; i++) {
			ThreadData<MapData> data = mapQueue.Dequeue();
			data.callback(data.parameter);
		}
		for(int i = 0; i < meshQueue.Count; i++) {
			ThreadData<MeshData> data = meshQueue.Dequeue();
			data.callback(data.parameter);
		}
	}

	//public void Awake() {if(update) RequestMapData(OnMapRecieved);}
	//public void Start() {if(update) RequestMapData(OnMapRecieved);}
	//public void OnValidate() {if(update) RequestMapData(OnMapRecieved);}
}

public struct ThreadData<T> {
	public readonly Action<T> callback;
	public readonly T parameter;

	public ThreadData(Action<T> callback, T parameter) {
		this.callback = callback;
		this.parameter = parameter;
	}
}

[System.Serializable]
public struct TerrainType {
	public string name;
	public float threshold;
	public Color color;
}

public class MeshData {
	public Vector3[] vertices;
	public int[] triangles;
	public Vector2[] uvs;

	private int index;

	public MeshData(int width, int height) {
		vertices = new Vector3[width * height];
		triangles = new int[(width - 1) * (height - 1) * 6];
		uvs = new Vector2[width * height];
	}

	public void AddTriangle(int a, int b, int c) {
		triangles[index++] = a;
		triangles[index++] = b;
		triangles[index++] = c;
	}

	public Mesh CreateMesh() {
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		mesh.RecalculateNormals();
		return mesh;
	}
}

public class MapData {
	public readonly float[,] noise;
	public readonly Color[] color;

	public MapData(float[,] noise, Color[] color) {
		this.noise = noise;
		this.color = color;
	}
}