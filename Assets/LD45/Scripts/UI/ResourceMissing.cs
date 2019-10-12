using System.Collections;
using DG.Tweening;
using LD45.Model;
using UnityEngine;
using UnityEngine.UI;

namespace LD45.UI {
	public class ResourceMissing : MonoBehaviour {
		[SerializeField]
		private CanvasGroup _content;

		[SerializeField]
		private Image _image;

		private Resource _resource;
		private int _threshold;
		private Coroutine _animation;

		public void Bind(Resource resource, int threshold) {
			_resource = resource;
			_threshold = threshold;

			_image.sprite = _resource.Icon;

			GameManager.Instance.StockpileChanged += OnStockpileChanged;
		}

		private void OnDestroy() {
			GameManager.Instance.StockpileChanged -= OnStockpileChanged;
		}

		private void OnStockpileChanged() {
			const float duration = 0.25f;

			var supplied = GameManager.Instance.Stockpile[_resource] > _threshold;
			var alpha = supplied ? 0 : 1;
			var offset = supplied ? new Vector2(0, 10) : Vector2.zero;

			IEnumerator Animate() {
				yield return DOTween.Sequence()
					.Join(_content.DOFade(alpha, duration))
					.Join(_content.transform.DOLocalMove(offset, duration))
					.WaitForCompletion();

				gameObject.SetActive(!supplied);
			}

			if (_animation != null) {
				StopCoroutine(_animation);
			}

			if (!supplied) {
				gameObject.SetActive(true);
			}

			if (gameObject.activeSelf) {
				_animation = StartCoroutine(Animate());
			}
		}
	}
}