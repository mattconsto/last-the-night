using UnityEngine;

public class NoiseGenerator {
	public static float[,] GenerateNoise(GenerationConfig config, Vector2 offset) {
		System.Random rng = new System.Random((int) (Constants.seed * offset.x * offset.y + config.seed));
		Vector2[] offsets = new Vector2[config.octaves];

		int width = config.resolution, height = config.resolution;

		for(int o = 0; o < config.octaves; o++) {
			offsets[o] = new Vector2(rng.NextFloat(0, 10000) + offset.x * width, rng.Next(0, 10000) - offset.y * height);
		}

		float[,] values = new float[width, height];
		for(int j = 0; j < height; j++) {
			for(int i = 0; i < width; i++) {
				float amplitude = 1;
				float frequency = 1;

				for(int o = 0; o < config.octaves; o++) {
					values[i, j] += Mathf.PerlinNoise(
						(i - width  / 2 + offsets[o].x) / config.scale.x * frequency,
						(j - height / 2 + offsets[o].y) / config.scale.x * frequency
					) * amplitude; 

					amplitude *= config.persistance;
					frequency *= config.lacunarity;
				}
			}
		}

		return values;
	}
}