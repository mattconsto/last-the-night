using UnityEngine;

public class MapData {
	public readonly float[,] noise;
	public readonly Color[]  color;

	public MapData(float[,] noise, Color[] color) {
		this.noise  = noise;
		this.color  = color;
	}
}