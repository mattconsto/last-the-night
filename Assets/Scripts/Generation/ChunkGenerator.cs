using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class ChunkGenerator : MonoBehaviour {
	[Range(1, 500)]
	public const int resolution = 129;
	public Vector2 scale = new Vector2(20, 20);
	[Range(1, 8)]
	public int octaves = 1;
	[Range(0, 1)]
	public float persistance = 1f;
	[Range(0, 20)]
	public float lacunarity = 1f;
	public int seed = 0;

	public AnimationCurve curve;

	public bool update = true;

	public TerrainType[] regions;

	public Queue<ThreadData<MapData>> mapQueue = new Queue<ThreadData<MapData>>();
	public Queue<ThreadData<MeshData>> meshQueue = new Queue<ThreadData<MeshData>>();

	public float[,] GenerateNoise(int width, int height, float scale, Vector2 offset) {
		System.Random rng = new System.Random(seed);
		Vector2[] offsets = new Vector2[octaves];

		for(int o = 0; o < octaves; o++) {
			offsets[o] = new Vector2(rng.Next(10000, 20000) + offset.x * width, rng.Next(10000, 20000) - offset.y * height);
		}

		float[,] values = new float[width, height];
		for(int j = 0; j < height; j++) {
			for(int i = 0; i < width; i++) {
				float amplitude = 1;
				float frequency = 1;

				for(int o = 0; o < octaves; o++) {
					values[i, j] += Mathf.PerlinNoise(
						(i - width  / 2 + offsets[o].x) / scale * frequency,
						(j - height / 2 + offsets[o].y) / scale * frequency
					) * amplitude; 

					amplitude *= persistance;
					frequency *= lacunarity;
				}
			}
		}

		return values;
	}

	public void RequestMapData(Action<MapData> callback, Vector2 offset) {
		ThreadStart threadStart = delegate {MapDataThread(callback, offset);};
		new Thread(threadStart).Start();
	}

	public void MapDataThread(Action<MapData> callback, Vector2 offset) {
		lock (mapQueue) {
			MapData data = GenerateMapData(offset);
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

	public MapData GenerateMapData(Vector2 offset) {
		float[,] noise = GenerateNoise(resolution, resolution, scale.x, offset);
		Color[] colors = TextureGenerator.DrawColor(noise, regions);
		return new MapData(noise, colors);
	}

	public MeshData GenerateMeshData(MapData map, int lod) {
		return MeshGenerator.GenerateMesh(map.noise, lod, curve, scale);
	}

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
}
