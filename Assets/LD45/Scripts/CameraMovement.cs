using Cinemachine;
using UnityEngine;

namespace LD45 {
	[RequireComponent(typeof(CinemachineVirtualCamera))]
	public class CameraMovement : MonoBehaviour {
		[SerializeField]
		private int _mouseZoneSize;

		[SerializeField]
		private float _speed = 1;

		private CinemachineVirtualCamera _virtual;
		private TerrainCollider _bounds;

		private void Awake() {
			_virtual = GetComponent<CinemachineVirtualCamera>();
			_bounds = FindObjectOfType<TerrainCollider>();
		}

		private void Update() {
			if (Input.mousePosition.x < _mouseZoneSize) {
				Displace(Vector3.left);
			}

			if (Input.mousePosition.y < _mouseZoneSize) {
				Displace(Vector3.down);
			}

			if (Input.mousePosition.x > Screen.width - _mouseZoneSize) {
				Displace(Vector3.right);
			}

			if (Input.mousePosition.y > Screen.height - _mouseZoneSize) {
				Displace(Vector3.up);
			}

			Displace(Input.GetAxis("Horizontal") * Vector3.right);
			Displace(Input.GetAxis("Vertical") * Vector3.up);
		}

		private void Displace(Vector3 direction) {
			var target = _virtual.Follow;
			var plane = new Plane(target.up, Vector3.zero);

			var forward = plane.ClosestPointOnPlane(transform.forward + transform.up).normalized;
			var right = plane.ClosestPointOnPlane(transform.right).normalized;

			var relative =
				forward * direction.y
				+ right * direction.x;

			var position = target.position + Time.timeScale * _speed * relative;
			position = _bounds.bounds.ClosestPoint(position);
			target.position = position;
		}
	}
}
