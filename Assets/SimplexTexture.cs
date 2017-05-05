using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class SimplexTexture : MonoBehaviour {
	public int resolution = 128;
	public Vector2 scale = new Vector2(20, 20);
	[Range(1, 8)]
	public int octaves = 1;
	[Range(0, 1)]
	public float persistance = 1f;
	[Range(0, 20)]
	public float lacunarity = 1f;
	public int seed = 0;

	public AnimationCurve curve;

	public enum DrawMode {Noise, Color};
	public bool drawMesh = true;
	public DrawMode mode = DrawMode.Color;

	public bool update = true;

	public TerrainType[] regions;

	public float[,] Calc2D(int width, int height, float scale) {
		System.Random rng = new System.Random(seed);
		Vector2[] offsets = new Vector2[octaves];
		for(int o = 0; o < octaves; o++) offsets[o] = new Vector2(rng.Next(10000, 20000), rng.Next(10000, 20000));

		float[,] values = new float[width, height];
		for(int i = 0; i < width; i++) {
			for(int j = 0; j < height; j++) {
				float amplitude = 1;
				float frequency = 1;

				for(int o = 0; o < octaves; o++) {
					values[i, j] += Simplex.Noise.Generate(
						(i - width  / 2) / scale * frequency + offsets[o].x,
						(j - height / 2)/ scale * frequency + offsets[o].y
					) * amplitude;

					amplitude *= persistance;
					frequency *= lacunarity;
				}
			}
		}

		// Normalise
		float maximum = values.Cast<float>().Max(), minimum = values.Cast<float>().Min();
		for(int i = 0; i < width; i++) {
			for(int j = 0; j < height; j++) {
				values[i,j] = Mathf.InverseLerp(minimum, maximum, values[i,j]);
			}
		}

		return values;
	}

	public void DrawNoise(float[,] noise, Color[] colors) {
		for(int y = 0; y < resolution; y++) {
			for(int x = 0; x < resolution; x++) {
				colors[y * resolution + x] = Color.Lerp(Color.black, Color.white, noise[x,y]);
			}
		}
	}

	public void DrawColor(float[,] noise, Color[] colors) {
		for(int y = 0; y < resolution; y++) {
			for(int x = 0; x < resolution; x++) {
				for(int i = 0; i < regions.Length; i++) {
					if(noise[x,y] <= regions[i].threshold) {
						colors[y * resolution + x] = regions[i].color;
						break;
					}
				}
			}
		}
	}

	public void RenderTexture() {
		resolution = Mathf.Max(resolution, 1);

		// Generate noise
		float[,] noise = Calc2D(resolution, resolution, scale.x);

		// Color it
		Color[] colors = new Color[resolution * resolution];
		switch(mode) {
			case DrawMode.Noise: DrawNoise(noise, colors); break;
			case DrawMode.Color: DrawColor(noise, colors); break;
		}

		// Heightmap
		if(drawMesh) {
			MeshData meshData = new MeshData(resolution, resolution);
			int vertIndex = 0;
			Vector2 vertOffset = new Vector2((resolution - 1) / -2f, (resolution - 1) / -2f);
			for(int y = 0; y < resolution; y++) {
				for(int x = 0; x < resolution; x++) {
					meshData.vertices[vertIndex] = new Vector3(vertOffset.x + x, curve.Evaluate(noise[x,y]) * scale.y, vertOffset.y - y);
					meshData.uvs[vertIndex] = new Vector2(x / (float) resolution, y / (float) resolution);

					if(x < resolution - 1 && y < resolution - 1) {
						meshData.AddTriangle(vertIndex, vertIndex+resolution+1, vertIndex+resolution);
						meshData.AddTriangle(vertIndex+resolution+1, vertIndex, vertIndex+1);
					}

					vertIndex++;
				}
			}
			GetComponent<MeshFilter>().sharedMesh = meshData.CreateMesh();
		}
		
		// Create texture
		Texture2D texture = new Texture2D (resolution, resolution);
		//texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels(colors);
		texture.Apply();

		GetComponent<MeshRenderer>().sharedMaterial.mainTexture = texture;
	}

	public void Start() {if(update) RenderTexture();}
	public void OnValidate() {if(update) RenderTexture();}
}

[System.Serializable]
public struct TerrainType {
	public string name;
	public float threshold;
	public Color color;
}

public class MeshData {
	public Vector3[] vertices;
	public int[] triangles;
	public Vector2[] uvs;

	private int index;

	public MeshData(int width, int height) {
		vertices = new Vector3[width * height];
		triangles = new int[(width - 1) * (height - 1) * 6];
		uvs = new Vector2[width * height];
	}

	public void AddTriangle(int a, int b, int c) {
		triangles[index++] = a;
		triangles[index++] = b;
		triangles[index++] = c;
	}

	public Mesh CreateMesh() {
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		mesh.RecalculateNormals();
		return mesh;
	}
}