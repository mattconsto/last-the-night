using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class ChunkGenerator : MonoBehaviour {
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

	public bool update = true;

	public TerrainType[] regions;

	public Queue<ThreadData<MapData>> mapQueue = new Queue<ThreadData<MapData>>();
	public Queue<ThreadData<MeshData>> meshQueue = new Queue<ThreadData<MeshData>>();

	public float[,] GenerateNoise(int width, int height, float scale, Vector2 center) {
		System.Random rng = new System.Random(seed);
		Vector2[] offsets = new Vector2[octaves];
		for(int o = 0; o < octaves; o++) offsets[o] = new Vector2(rng.Next(10000, 20000), rng.Next(10000, 20000)) + center;

		float[,] values = new float[width, height];
		for(int i = 0; i < width; i++) {
			for(int j = 0; j < height; j++) {
				float amplitude = 1;
				float frequency = 1;

				for(int o = 0; o < octaves; o++) {
					values[i, j] += Mathf.PerlinNoise(
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
		float[,] noise = GenerateNoise(resolution, resolution, scale.x, center);
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
