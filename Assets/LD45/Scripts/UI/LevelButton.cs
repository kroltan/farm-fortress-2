using System;
using LD45.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LD45.UI {
	public class LevelButton : MonoBehaviour {
		public event Action Selected;

		public Level Level {
			get => _level;
			set {
				_level = value;
				name = _level.name;
				_icon.sprite = _level.Icon;
				_name.text = _level.name;
			}
		}

		[SerializeField]
		private Image _icon;

		[SerializeField]
		private TMP_Text _name;

		[SerializeField]
		private Button _button;

		private Level _level;

		private void Start() {
			_button.onClick.AddListener(() => Selected?.Invoke());
		}
	}
}