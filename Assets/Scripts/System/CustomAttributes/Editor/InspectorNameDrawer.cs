using UnityEditor;
using UnityEngine;

/**
 * Created by ChatGPT.
 */
[CustomPropertyDrawer(typeof(InspectorNameAttribute))]
public class InspectorNameDrawer : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		// Get the attribute data
		InspectorNameAttribute renameAttribute = (InspectorNameAttribute)attribute;
		
		// Change the label's text to the new name
		label.text = renameAttribute.NewName;

		// Draw the property with the new label
		EditorGUI.PropertyField(position, property, label);
	}
}
