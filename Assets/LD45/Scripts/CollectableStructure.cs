using System;
using System.Collections;
using LD45.UI;
using UnityEngine;
using UnityEngine.UI;

namespace LD45 {
	[RequireComponent(typeof(Structure))]
	public class CollectableStructure : MonoBehaviour {
		[SerializeField]
		private Button _template;

		private Structure _structure;
		private Button _widget;

		public YieldInstruction Display() {
			IEnumerator DisplayImpl() {
				var pressed = false;

				void Callback() {
					pressed = true;
				}

				_widget.gameObject.SetActive(true);
				_widget.onClick.AddListener(Callback);
				yield return new WaitUntil(() => pressed);
				_widget.onClick.RemoveListener(Callback);
				_widget.gameObject.SetActive(false);
			}

			return StartCoroutine(DisplayImpl());
		}

		private void Awake() {
			_structure = GetComponent<Structure>();

			var status = StructureStatusManager.Instance;
			status.Registered += s => {
				if (s != _structure) {
					return;
				}

				_widget = Instantiate(_template);
				_widget.gameObject.SetActive(false);
				status.AddSpecial(_structure, (RectTransform) _widget.transform);
			};
		}
	}
}
