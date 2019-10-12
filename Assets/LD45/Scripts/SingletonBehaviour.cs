using UnityEngine;

namespace LD45
{
	public class SingletonBehaviour<T> : MonoBehaviour
	where T : SingletonBehaviour<T>
	{
		private static T _instance;

		public static T Instance {
			get {
				if (_instance == null) {
					_instance = FindObjectOfType<T>();
				}

				return _instance;
			}
		}
	}
}
