using System.Linq;
using UnityEngine;

namespace LD45.UI
{
	[RequireComponent(typeof(RectTransform))]
	public class FollowWorldSpace : MonoBehaviour {
		public Transform Target {
			get => _target;
			set {
				_target = value;
				UpdateOffset();
			}
		}

		[SerializeField]
		private Transform _target;

		private Canvas _canvas;
		private float _heightOffset;

		private void Awake() {
			_canvas = GetComponentInParent<Canvas>();

			if (_target != null) {
				UpdateOffset();
			}
		}

		private void LateUpdate() {
			if (_target == null) {
				return;
			}

			var world = _target.position + _target.up * _heightOffset;
			var screen = RectTransformUtility.WorldToScreenPoint(_canvas.worldCamera, world);
			transform.localPosition = screen / _canvas.scaleFactor;
		}

		private void UpdateOffset() {
			var simple = _target.GetComponentsInChildren<MeshRenderer>();
			var skinned = _target.GetComponentsInChildren<SkinnedMeshRenderer>();

			if (simple.Length + skinned.Length == 0) {
				_heightOffset = 0;
				return;
			}

			var simpleBounds = simple.Select(r => r.bounds);
			var skinnedBounds = skinned.Select(r => r.bounds);

			_heightOffset = simpleBounds
				.Concat(skinnedBounds)
				.Max(bounds => bounds.extents.y);
		}
	}
}
