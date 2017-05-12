using UnityEngine;

public class TextureGenerator {
	public static Color[] DrawNoise(float[,] noise) {
		int height = noise.GetLength(0), width = noise.GetLength(1);
		Color[] colors = new Color[width * height];

		for(int y = 0; y < height; y++) {
			for(int x = 0; x < width; x++) {
				colors[y * width + x] = Color.Lerp(Color.black, Color.white, noise[x,y]);
			}
		}

		return colors;
	}

	public static Color[] DrawColor(float[,] noise, TerrainType[] regions) {
		int height = noise.GetLength(0), width = noise.GetLength(1);
		Color[] colors = new Color[width * height];

		for(int y = 0; y < height; y++) {
			for(int x = 0; x < width; x++) {
				for(int i = 0; i < regions.Length; i++) {
					if(noise[x,y] <= regions[i].threshold) {
						colors[y * width + x] = regions[i].color;
						break;
					}
				}
			}
		}

		return colors;
	}
}