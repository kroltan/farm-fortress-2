using System.Collections.Generic;
using LD45.Model;
using UnityEngine;

namespace LD45.UI
{
	public class ValueGroupDisplay : MonoBehaviour {
		[SerializeField]
		private ResourceView _viewTemplate;

		public void Bind(IReadOnlyDictionary<Resource, int> values) {
			foreach (Transform child in transform) {
				Destroy(child.gameObject);
			}

			foreach (var (resource, value) in values) {
				var view = Instantiate(_viewTemplate, transform);
				view.Resource = resource;
				view.Value = value;
			}
		}
	}
}
