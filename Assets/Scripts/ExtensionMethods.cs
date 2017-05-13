using UnityEngine;
using UnityEngine.UI;

public static class ExtensionMethods {
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