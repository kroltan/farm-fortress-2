﻿using UnityEditor;
using UnityEngine;

namespace LD45 {
	[CustomPropertyDrawer(typeof(LayerAttribute))]
	public class LayerAttributeDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			property.intValue = EditorGUI.LayerField(position, label, property.intValue);
		}
	}
}