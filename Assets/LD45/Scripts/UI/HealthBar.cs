using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LD45.UI {
	[RequireComponent(typeof(CanvasGroup))]
	public class HealthBar : MonoBehaviour {
		[SerializeField]
		private TMP_Text _name;

		[SerializeField]
		private Image _fill;

		[SerializeField]
		private Image _emphasis;
		
		private GameActor _actor;
		private Coroutine _fillAnimation;

		public void Bind(GameActor actor) {
			_actor = actor;

			var follower = GetComponent<FollowWorldSpace>();
			if (follower != null) {
				follower.Target = _actor.transform;
			}

			_actor.HealthChanged += OnHealthChanged;
			_actor.Died += OnDied;

			_name.text = _actor.name;

			OnHealthChanged();
		}

		private void OnDied() {
			IEnumerator Remove() {
				yield return GetComponent<CanvasGroup>()
					.DOFade(0, 0.3f)
					.WaitForCompletion();
				yield return _fillAnimation;
				Destroy(gameObject);
			}

			StartCoroutine(Remove());
		}

		private void OnHealthChanged() {
			const float duration = 0.25f;

			if (_actor.Health == _actor.MaxHealth) {
				gameObject.SetActive(false);
				return;
			}

			gameObject.SetActive(true);

			var percent = _actor.Health / (float) _actor.MaxHealth;

			IEnumerator Animate() {
				yield return _emphasis.DOFillAmount(percent, duration)
					.SetEase(Ease.OutCirc)
					.WaitForCompletion();
				yield return _fill.DOFillAmount(percent, duration)
					.SetEase(Ease.InCirc)
					.WaitForCompletion();
			}

			if (_fillAnimation != null) {
				StopCoroutine(_fillAnimation);
			}

			_fillAnimation = StartCoroutine(Animate());
		}
	}
}