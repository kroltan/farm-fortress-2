using DG.Tweening;
using LD45.Model;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace LD45.UI {
	public class ResourceView : MonoBehaviour {
		public Resource Resource {
			get => _resource;
			set {
				_resource = value;
				_resourceName.text = _resource.name;
				_resourceIcon.sprite = _resource.Icon;
			}
		}

		public int Value {
			get => _value;
			set {
				const float duration = 0.3f;

				_valueLabel.transform.localScale = Vector3.one;
				if (value > _value) {
					_valueLabel.transform.DOPunchScale(-0.2f * Vector3.one, duration);
				} else if (value < _value) {
					_valueLabel.transform.DOPunchScale(0.2f * Vector3.one, duration);
				}

				_value = value;
				_valueLabel.text = _value.ToString();
			}
		}

		[SerializeField]
		private TMP_Text _resourceName;

		[SerializeField]
		private Image _resourceIcon;

		[FormerlySerializedAs("_value")]
		[SerializeField]
		private TMP_Text _valueLabel;

		private Resource _resource;
		private int _value;
	}
}
