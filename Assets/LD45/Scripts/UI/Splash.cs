using System;
using UnityEngine;
using UnityEngine.Analytics;

namespace LD45.UI {
	public class Splash : MonoBehaviour {
		public bool LevelSelect {
			get => _animator.GetBool(_parameter);
			set => _animator.SetBool(_parameter, value);
		}

		[SerializeField]
		private string _parameter;

		[SerializeField]
		private Animator _animator;

		private void Awake() {
		}

		private void Update() {
			var value = LevelSelect;
			if (!value && Input.anyKeyDown) {
				LevelSelect = true;
			} else if (value && Input.GetKeyDown(KeyCode.Escape)) {
				LevelSelect = false;
			}
		}
	}
}
