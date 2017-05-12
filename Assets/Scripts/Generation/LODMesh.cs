using UnityEngine;

public class LODMesh {
	public Mesh mesh;
	public bool requested;
	public bool recieved;
	public int lod;

	private ChunkGenerator generator;

	public LODMesh(int lod, ChunkGenerator generator) {
		this.lod = lod;
		this.generator = generator;
	}

	public void OnMeshReceived(MeshData mesh) {
		this.mesh = mesh.CreateMesh();
		recieved = true;
	}

	public void Request(MapData map) {
		generator.RequestMeshData(map, lod, OnMeshReceived);
		requested = true;
	}
}