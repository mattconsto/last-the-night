using System;
using UnityEngine;

public class LODMesh {
	public Mesh mesh;
	public bool requested;
	public bool recieved;
	public int lod;

	private Action<LODMesh> _callback;
	private GenerationConfig config;

	public LODMesh(int lod, GenerationConfig config) {
		this.lod = lod;
		this.config = config;
	}

	public void OnMeshReceived(MeshData mesh) {
		this.mesh = mesh.CreateMesh();
		recieved = true;
		if(_callback != null) _callback(this);
	}

	public void Request(Action<LODMesh> callback, MapData map) {
		_callback = callback;
		requested = true;
		config.generator.RequestMeshData(OnMeshReceived, config, map, lod);
	}
}