using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace LD45.UI {
	public class Timer : MonoBehaviour {
		[SerializeField]
		private Image _fill;

		[SerializeField]
		private Image _icon;

		public YieldInstruction Run(Sprite icon, float duration) {
			const float appearDuration = 0.3f;
			const float disappearDuration = 0.15f;

			_icon.sprite = icon;

			IEnumerator TimerRoutine() {
				_fill.color = _fill.color.Change(a: 0);
				_fill.fillAmount = 0;

				yield return DOTween.Sequence()
					.Join(_fill.DOFade(1, appearDuration))
					.Join(_fill.transform.DOPunchScale(0.2f * Vector3.one, appearDuration))
					.WaitForCompletion();
				yield return _fill
					.DOFillAmount(1, duration)
					.SetEase(Ease.Linear)
					.WaitForCompletion();
				yield return _fill
					.DOFade(0, disappearDuration)
					.WaitForCompletion();

				Destroy(gameObject);
			}

			return StartCoroutine(TimerRoutine());
		}
	}
}
