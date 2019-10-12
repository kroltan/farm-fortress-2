using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LD45.UI {
	public class PaletteButton : MonoBehaviour {
		public event Action<PaletteButton> Pressed;

		public bool Buyable => _button.interactable;

		public Structure Structure {
			get => _structure;
			set {
				_structure = value;
				_name.text = $"<b>{_structure.name}</b>\n{_structure.Description}".Trim();
				_health.text = string.Format(_healthFormat, _structure.Health);
				_cost.Bind(_structure.Cost);
				_production.Bind(_structure.Production);
				_consumption.Bind(_structure.Consumption);
			}
		}

		public Texture Thumbnail {
			get => _thumbnail.texture;
			set => _thumbnail.texture = value;
		}

		public KeyCode? Shortcut {
			get => _shortcut;
			set {
				_shortcut = value;
				_shortcutLabel.gameObject.SetActive(_shortcut.HasValue);
				if (_shortcut.HasValue) {
					_shortcutLabel.text = Enum
						.GetName(typeof(KeyCode), _shortcut.Value)
						.Replace("Alpha", "");
				}
			}
		}

		[SerializeField]
		private TMP_Text _name;

		[SerializeField]
		private TMP_Text _health;

		[SerializeField]
		private TMP_Text _shortcutLabel;

		[SerializeField]
		private Button _button;

		[SerializeField]
		private RawImage _thumbnail;

		[SerializeField]
		private ValueGroupDisplay _cost;

		[SerializeField]
		private ValueGroupDisplay _production;

		[SerializeField]
		private ValueGroupDisplay _consumption;

		private Structure _structure;
		private string _healthFormat;
		private KeyCode? _shortcut;

		private void Awake() {
			_healthFormat = _health.text;
		}

		private void Start() {
			_button.onClick.AddListener(InvokePressed);
			GameManager.Instance.StockpileChanged += OnStockpileChanged;
		}

		private void Update() {
			if (_shortcut.HasValue && Input.GetKeyDown(_shortcut.Value) && Buyable) {
				InvokePressed();
			}
		}

		private void OnStockpileChanged() {
			_button.interactable = GameManager.Instance.HasEnough(Structure.Cost);
		}

		private void InvokePressed() {
			Pressed?.Invoke(this);
		}
	}
}
