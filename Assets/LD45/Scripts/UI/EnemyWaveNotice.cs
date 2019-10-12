using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace LD45.UI {
	public class EnemyWaveNotice : MonoBehaviour {
		[SerializeField]
		private TMP_Text _message;

		[SerializeField]
		private string _beginFormat;

		[SerializeField]
		private string _endFormat;

		private void Start() {
			var enemy = EnemyManager.Instance;

			_message.gameObject.SetActive(false);

			enemy.Begin += OnBegin;
			enemy.End += OnEnd;
		}

		private void OnBegin(int wave) {
			ShowMessage(string.Format(_beginFormat, wave + 1));
		}

		private void OnEnd(int wave) {
			ShowMessage(string.Format(_endFormat, wave + 1));
		}

		private YieldInstruction ShowMessage(string text) {
			const float showDuration = 0.9f;
			const float sustain = 2.3f;
			const float hideDuration = 0.4f;
			const float idleScale = 0.8f;

			_message.text = text;

			IEnumerator ShowMessageRoutine() {
				_message.gameObject.SetActive(true);
				_message.color = _message.color.Change(a: 0);
				_message.transform.localScale = Vector3.one * idleScale;

				yield return DOTween.Sequence()
					.Join(_message.transform.DOScale(1, showDuration))
					.Join(_message.DOFade(1, showDuration))
					.AppendInterval(sustain)
					.Append(_message.transform.DOScale(idleScale, hideDuration))
					.Join(_message.DOFade(0, hideDuration))
					.WaitForCompletion();

				_message.gameObject.SetActive(false);
			}

			return StartCoroutine(ShowMessageRoutine());
		}
	}
}
