using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LD45 {
	[CustomPropertyDrawer(typeof(SceneAttribute))]
	public class SceneAttributeDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			var scenes = new GUIContent[EditorBuildSettings.scenes.Length];

			for (var i = 0; i < scenes.Length; i++) {
				scenes[i] = new GUIContent(EditorBuildSettings.scenes[i].path);
			}

			property.intValue = EditorGUI.Popup(position, label, property.intValue, scenes);
		}
	}
}