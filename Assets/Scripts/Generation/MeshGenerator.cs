using UnityEngine;

public class MeshGenerator {
	public static MeshData GenerateMesh(float[,] noise, int lod, AnimationCurve curve, Vector2 scale) {
		int height = noise.GetLength(0), width = noise.GetLength(1);
		Vector2 vertOffset = new Vector2((width - 1) / -2f, (height - 1) / -2f);

		int increment = (lod == 0) ? 1 : lod * 2;
		int resolution = (width - 1) / increment + 1;

		MeshData meshData = new MeshData(resolution, resolution);
		int vertIndex = 0;

		for(int y = 0; y < height; y += increment) {
			for(int x = 0; x < width; x += increment) {
				float evaluated = curve.Evaluate(noise[x,y]) * scale.y;

				/*if(x < InfiniteTerrain.seamOverlap) {
					evaluated -= (InfiniteTerrain.seamOverlap - x) * 1.1f;
				} else if(x > width - InfiniteTerrain.seamOverlap - 1) {
					evaluated -= (InfiniteTerrain.seamOverlap - (width - 1 - x)) * 1.1f;
				} else if(y < InfiniteTerrain.seamOverlap) {
					evaluated -= (InfiniteTerrain.seamOverlap - y) * 1.1f;
				} else if(y > height - InfiniteTerrain.seamOverlap - 1) {
					evaluated -= (InfiniteTerrain.seamOverlap - (height - 1 - y)) * 1.1f;
				}*/

				if(x <= 0 || y <= 0 || x >= width - 4 || y >= height - 4) {
					evaluated = 0;
				}

				meshData.vertices[vertIndex] = new Vector3(vertOffset.x + x, evaluated, vertOffset.y - y);
				meshData.uvs[vertIndex] = new Vector2(x / (float) width, y / (float) height);

				if(x < width - increment && y < height - increment) {
					meshData.AddTriangle(vertIndex, vertIndex+resolution+1, vertIndex+resolution);
					meshData.AddTriangle(vertIndex+resolution+1, vertIndex, vertIndex+1);
				}

				vertIndex++;
			}
		}

		return meshData;
	}
}