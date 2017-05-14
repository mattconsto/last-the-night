using UnityEngine;

public class LODMesh {
	public Mesh mesh;
	public bool requested;
	public bool recieved;
	public int lod;

	private GenerationConfig config;

	public LODMesh(int lod, GenerationConfig config) {
		this.lod = lod;
		this.config = config;;
	}

	public void OnMeshReceived(MeshData mesh) {
		this.mesh = mesh.CreateMesh();
		recieved = true;
	}

	public void Request(MapData map) {
		config.generator.RequestMeshData(OnMeshReceived, config, map, lod);
		requested = true;
	}
}