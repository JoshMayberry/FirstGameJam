using UnityEngine;

/**
 * Created by ChatGPT.
 */
public class RequiredAttribute : PropertyAttribute {
	public string CustomMessage { get; }
	public string TintColor { get; } = "#FF000066";
	public string IconPath { get; }
	private float[] iconSize = { 18, 18 };
	private float[] iconOffset = { -20, 0 };

	public Vector2 IconSize => new Vector2(iconSize[0], iconSize[1]);
	public Vector2 IconOffset => new Vector2(iconOffset[0], iconOffset[1]);

	public RequiredAttribute(
		string message = "This field is required!",
		string tintColor = "#FF000066",
		string iconPath = null,
		float[] iconSize = null,
		float[] iconOffset = null
	) {
		CustomMessage = message;
		TintColor = tintColor;
		IconPath = iconPath;

		if (iconSize != null && iconSize.Length == 2) {
			this.iconSize = iconSize;
		}

		if (iconOffset != null && iconOffset.Length == 2) {
			this.iconOffset = iconOffset;
		}
	}
}
