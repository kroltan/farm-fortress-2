using UnityEngine;
using UnityEngine.EventSystems;

namespace LD45.UI {
	public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
		[SerializeField]
		private RectTransform _body;

		public void OnPointerEnter(PointerEventData eventData) {
			_body.gameObject.SetActive(true);
		}

		public void OnPointerExit(PointerEventData eventData) {
			_body.gameObject.SetActive(false);
		}

		private void Awake() {
			_body.gameObject.SetActive(false);
		}
	}
}
