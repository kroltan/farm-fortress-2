using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LD45.UI {
	public class Loading : MonoBehaviour {
		public Sprite Background {
			get => _image.sprite;
			set {
				_image.sprite = value;
				_fitter.aspectRatio = value.texture.width / (float) value.texture.height;
			}
		}

		public string Description {
			get => _text.text;
			set => _text.text = value;
		}

		[SerializeField]
		private AspectRatioFitter _fitter;

		[SerializeField]
		private Image _image;

		[SerializeField]
		private TMP_Text _text;

		[SerializeField]
		private CanvasGroup _group;

		public YieldInstruction ShowWhile(YieldInstruction process) {
			const float duration = 0.7f;
			IEnumerator Routine() {
				_group.blocksRaycasts = true;

				yield return _group
					.DOFade(1, duration)
					.WaitForCompletion();
				
				yield return process;

				yield return _group
					.DOFade(0, duration)
					.WaitForCompletion();

				_group.blocksRaycasts = false;
			}

			return StartCoroutine(Routine());
		}

		private void Start() {
			_group.alpha = 0;
			_group.blocksRaycasts = false;
		}
	}
}
