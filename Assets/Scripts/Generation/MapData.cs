using UnityEngine;

public class MapData {
	public readonly float[,] noise;
	//public readonly float[,] biomes;
	public readonly Color[]  color;

	public MapData(float[,] noise, Color[] color) {
		this.noise  = noise;
		//this.biomes = biomes;
		this.color  = color;
	}
}