using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;
using LD45.Model;
using UnityEngine;
using UnityEngine.UI;

namespace LD45.UI {
	public class ResourceCounter : SingletonBehaviour<ResourceCounter> {
		[SerializeField]
		private RectTransform _container;

		[SerializeField]
		private ResourceView _viewTemplate;

		[SerializeField]
		private Image _flyerTemplate;

		[SerializeField]
		private RectTransform _flyerContainer;

		private readonly Dictionary<Resource, ResourceView> _views = new Dictionary<Resource, ResourceView>();
		private Canvas _canvas;

		private void Start() {
			_canvas = GetComponentInParent<Canvas>();
			var game = GameManager.Instance;

			game.StockpileChanged += OnStockpileChanged;

			foreach (var (resource, value) in game.Stockpile) {
				var view = Instantiate(_viewTemplate, _container);
				view.Resource = resource;
				view.Value = value;

				_views[resource] = view;

				if (value == 0) {
					view.gameObject.SetActive(false);
				}
			}
		}

		private void OnStockpileChanged() {
			var game = GameManager.Instance;
			foreach (var view in _container.GetComponentsInChildren<ResourceView>(true)) {
				view.Value = game.Stockpile[view.Resource];

				if (view.Value > 0) {
					view.gameObject.SetActive(true);
				}
			}
		}

		public void Send(Vector3 worldPosition, IEnumerable<Resource> resources) {
			const float transition = 0.3f;

			var local = RectTransformUtility.WorldToScreenPoint(_canvas.worldCamera, worldPosition)
			            / _canvas.scaleFactor;

			if (!_flyerContainer.rect.Contains(local)) {
				return;
			}

			foreach (var resource in resources) {
				var flyer = Instantiate(_flyerTemplate, _flyerContainer);
				flyer.sprite = resource.Icon;
				flyer.color = flyer.color.Change(a: 0);
				flyer.transform.localPosition = local;
				flyer.transform.localScale = Vector3.zero;

				var offshoot = Random.onUnitSphere;


				IEnumerator Animate() {
					var positions = new Vector3[3];
					positions[0] = _views[resource].transform.position;
					positions[1] = flyer.transform.position + offshoot * 100;
					positions[2] = positions[0] + new Vector3(0, -offshoot.y) * 100;

					yield return DOTween.Sequence()
						.Join(flyer.transform.DOScale(1, transition))
						.Join(flyer.DOFade(1, transition))
						.WaitForCompletion();

					yield return DOTween.Sequence()
						.Append(flyer.transform.DOPath(positions, 1f, PathType.CubicBezier))
						.Append(flyer.transform.DOScale(0, transition))
						.Join(flyer.DOFade(0, transition))
						.WaitForCompletion();
				}

				StartCoroutine(Animate());
			}
		}
	}
}