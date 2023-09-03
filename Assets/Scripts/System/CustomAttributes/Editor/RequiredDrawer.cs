using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

/**
 * Created by ChatGPT.
 * Custom Property Drawer for the RequiredAttribute.
 * 
 * Example Use
 *     // Basic usage
 *     [Required]
 *     public GameObject test;
 * 
 *     // Basic usage with a custom message
 *     [Required("Lorem is missing!")]
 *     public GameObject lorem;
 *     
 *     // Using a custom message, color tint, and icon path:
 *     [Required("Ipsum is missing!", "#FF000088", "Assets/Icons/warningIcon.png")]
 *     public GameObject ipsum;
 *     
 *     // Customizing further with icon size and position:
 *     [Required("Dolor is missing!", "#00FF0088", "Assets/Icons/errorIcon.png", new Vector2(24, 24), new Vector2(-30, -2))]
 *     public Transform dolor;
 *     
 *     // Using the default message but customizing the appearance:
 *     [Required(tintColor: "#0000FF88", iconPath: "Assets/Icons/infoIcon.png", iconSize: new Vector2(16, 16), iconOffset: new Vector2(-28, 0))]
 *     public Collider sit;
 *     
 *     // Just a tint without any icon:
 *     [Required(tintColor: "#FFFF0088")]
 *     public Light amet;
 *     
 *     // Using nested properties and arrays:
 *     [Required("Consectetur is missing elements!", tintColor: "#FF00FF88", iconPath: "Assets/Icons/alertIcon.png")]
 *     public Rigidbody[] consectetur;
 *     
 *     // For strings:
 *     [Required("Adipiscing string is empty!")]
 *     public string adipiscing;
 *     
 *     // For basic value types (like integers):
 *     [Required("Elit integer is not set!", tintColor: "#FF880088")]
 *     public int elit;
 */
[CustomPropertyDrawer(typeof(RequiredAttribute))]
public class RequiredDrawer : PropertyDrawer {
	private const int MAX_DEPTH = 3;
	private static Dictionary<string, Texture> iconCache = new Dictionary<string, Texture>();

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		var requiredAttr = attribute as RequiredAttribute;
		if (requiredAttr == null) return;

		bool isFieldMissing = IsFieldMissing(property);

		Rect originalPosition = position;
		originalPosition.height = base.GetPropertyHeight(property, label);

		if (isFieldMissing) {
			Color tint;
			if (!ColorUtility.TryParseHtmlString(requiredAttr.TintColor, out tint)) {
				tint = new Color(1, 0, 0, 0.4f); // Default to 40% opacity red if parsing fails
			}
			EditorGUI.DrawRect(originalPosition, tint);

			if (!string.IsNullOrEmpty(requiredAttr.IconPath)) {
				if (!iconCache.TryGetValue(requiredAttr.IconPath, out Texture customIcon)) {
					customIcon = EditorGUIUtility.Load(requiredAttr.IconPath) as Texture;
					if (customIcon != null) {
						iconCache[requiredAttr.IconPath] = customIcon;
					}
					else {
						Debug.LogWarning($"Required attribute icon not found at path: {requiredAttr.IconPath}");
					}
				}

				if (customIcon != null) {
					position.width = requiredAttr.IconSize.x;
					position.height = requiredAttr.IconSize.y;
					position.x += originalPosition.width + requiredAttr.IconOffset.x;
					position.y += requiredAttr.IconOffset.y;

					GUI.DrawTexture(position, customIcon);
				}
			}
		}

		EditorGUI.PropertyField(originalPosition, property, label, true);

		if (isFieldMissing) {
			position.y += originalPosition.height;
			position.height = 20;
			EditorGUI.HelpBox(position, requiredAttr.CustomMessage, MessageType.Warning);
		}
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		if (IsFieldMissing(property)) {
			return EditorGUI.GetPropertyHeight(property, label, true) + 22;
		}
		return EditorGUI.GetPropertyHeight(property, label, true);
	}

	/// <summary>
	/// Determines if a serialized property is missing a required value.
	/// </summary>
	/// <param name="property">The serialized property to check.</param>
	/// <returns>True if the property is missing a value, otherwise false.</returns>
	private bool IsFieldMissing(SerializedProperty property, int currentDepth = 0) {
		if (currentDepth > MAX_DEPTH) {
			return false;
		}

		if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null) {
			return true;
		}
		if (property.propertyType == SerializedPropertyType.String && string.IsNullOrEmpty(property.stringValue)) {
			return true;
		}
		if (property.propertyType == SerializedPropertyType.ArraySize && property.arraySize == 0) {
			return true;
		}
		if (property.propertyType == SerializedPropertyType.Float && property.floatValue == float.MinValue) {
			return true;
		}
		if (property.propertyType == SerializedPropertyType.Integer && property.intValue == int.MinValue) {
			return true;
		}
		if (property.isArray) {
			for (int i = 0; i < property.arraySize; i++) {
				if (property.GetArrayElementAtIndex(i).objectReferenceValue == null) {
					return true;
				}
			}
		}

		if (property.hasChildren) {
			
			SerializedProperty child = property.Copy();
			SerializedProperty endProperty = property.GetEndProperty();
			bool enterChildren = true;
			while (child.NextVisible(enterChildren)) {
				if (SerializedProperty.EqualContents(child, endProperty))
					break;

				if (IsFieldMissing(child, currentDepth + 1)) {
					return true;
				}
				enterChildren = false;
			}
		}

		return false;
	}
}
