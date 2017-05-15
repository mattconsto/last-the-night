[System.Serializable]
public struct LODInfo {
	public int lod;
	public float threshold;

	public LODInfo(int lod, float threshold) {
		this.lod = lod;
		this.threshold = threshold;
	}
}