using System;
using UnityEngine;
using UnityEngine.UI;

public static class ExtensionMethods {
	/* Arrays */

	public static T[] RemoveAt<T>(this T[] source, int index) {
	    T[] dest = new T[source.Length - 1];

	    if(index > 0) Array.Copy(source, 0, dest, 0, index);
	    if(index < source.Length - 1) Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

	    return dest;
	}

	/* Random Extensions */

	public static double NextDouble(this System.Random r, double a) {
		if(a < 0) throw new ArgumentOutOfRangeException(a + "");
		return r.NextDouble() * a;
	}

	public static double NextDouble(this System.Random r, double a, double b) {
		double min = Math.Min(a, b), max = Math.Max(a, b);
		return r.NextDouble() * (max - min) + min;
	}

	public static float NextFloat(this System.Random r) {
		return (float) r.NextDouble();
	}

	public static float NextFloat(this System.Random r, float a) {
		return (float) r.NextDouble(a);
	}

	public static float NextFloat(this System.Random r, float a, float b) {
		return (float) r.NextDouble(a, b);
	}

	public static Color ColorHSV(this System.Random r) {
		return r.ColorHSV(0, 1, 0, 1, 0, 1);
	}

	public static Color ColorHSV(this System.Random r, float ha, float hb) {
		return r.ColorHSV(ha, hb, 0, 1, 0, 1);
	}

	public static Color ColorHSV(this System.Random r, float ha, float hb, float sa, float sb) {
		return r.ColorHSV(ha, hb, sa, sb, 0, 1);
	}

	public static Color ColorHSV(this System.Random r, float ha, float hb, float sa, float sb, float va, float vb) {
		return Color.HSVToRGB(
			r.NextFloat(ha, hb) % 1,
			Mathf.Clamp01(r.NextFloat(sa, sb)),
			Mathf.Clamp01(r.NextFloat(va, vb))
		);
	}

	public static Color ColorRGB(this System.Random r) {
		return r.ColorRGB(0, 1, 0, 1, 0, 1);
	}

	public static Color ColorRGB(this System.Random r, float ra, float rb) {
		return r.ColorRGB(ra, rb, 0, 1, 0, 1);
	}

	public static Color ColorRGB(this System.Random r, float ra, float rb, float ga, float gb) {
		return r.ColorRGB(ra, rb, ga, gb, 0, 1);
	}

	public static Color ColorRGB(this System.Random r, float ra, float rb, float ga, float gb, float ba, float bb) {
		return new Color(
			Mathf.Clamp01(r.NextFloat(ra, rb)),
			Mathf.Clamp01(r.NextFloat(ga, gb)),
			Mathf.Clamp01(r.NextFloat(ba, bb)),
			1
		);
	}

	public static Quaternion Rotation(this System.Random r) {
		return new Quaternion(r.NextFloat(), r.NextFloat(), r.NextFloat(), r.NextFloat());
	}

	/* Alpha Setters */

	public static void SetAlpha(this Material material, float value) {
		Color color = material.color;
		color.a = value;
		material.color = color;
	}

	public static void SetAlpha(this Text text, float value) {
		Color color = text.color;
		color.a = value;
		text.color = color;
	}

	public static void SetAlpha(this RawImage image, float value) {
		Color color = image.color;
		color.a = value;
		image.color = color;
	}

	/* LocalScale Setters */

	public static void SetLocalScaleX(this Transform transform, float value) {
		Vector3 scale = transform.localScale;
		scale.x = value;
		transform.localScale = scale;
	}

	public static void SetLocalScaleY(this Transform transform, float value) {
		Vector3 scale = transform.localScale;
		scale.y = value;
		transform.localScale = scale;
	}

	public static void SetLocalScaleZ(this Transform transform, float value) {
		Vector3 scale = transform.localScale;
		scale.z = value;
		transform.localScale = scale;
	}
}