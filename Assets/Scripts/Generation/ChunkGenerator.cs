using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour {
	private Queue<ThreadData<MapData>>  mapQueue  = new Queue<ThreadData<MapData>>();
	private Queue<ThreadData<MeshData>> meshQueue = new Queue<ThreadData<MeshData>>();

	public void RequestMapData(Action<MapData> callback, GenerationConfig config, Vector2 offset) {
		new Thread(delegate() {
			float[,] noise  = NoiseGenerator.GenerateNoise(config, offset);
			Color[]  colors = TextureGenerator.DrawColor(noise, config.regions);
			MapData  data   = new MapData(noise, colors);
			lock(mapQueue) {
				mapQueue.Enqueue(new ThreadData<MapData>(callback, data));
			}
		}).Start();
	}

	public void RequestMeshData(Action<MeshData> callback, GenerationConfig config, MapData map, int lod) {
		new Thread(delegate() {
			MeshData data = MeshGenerator.GenerateMesh(map.noise, lod, config.curve, config.scale);
			lock (meshQueue) {
				meshQueue.Enqueue(new ThreadData<MeshData>(callback, data));
			}
		}).Start();
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
