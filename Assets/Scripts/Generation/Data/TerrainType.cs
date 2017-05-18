using UnityEngine;

[System.Serializable]
public struct TerrainType {
	public string name;
	public float threshold;
	public Color color;

	public TerrainType(string name, float threshold, Color color) {
		this.name = name;
		this.threshold = threshold;
		this.color = color;
	}
}