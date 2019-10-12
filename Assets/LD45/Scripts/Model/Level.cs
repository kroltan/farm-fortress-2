using UnityEngine;

namespace LD45.Model {
	[CreateAssetMenu(menuName = "LD45/Level")]
	public class Level : ScriptableObject {
		public string Scene;
		public Sprite Icon;
	}
}
