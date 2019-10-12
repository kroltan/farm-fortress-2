using System.Collections;
using System.Linq;
using LD45.UI;
using UnityEngine;

namespace LD45 {
	[RequireComponent(typeof(Shooter))]
	[RequireComponent(typeof(Structure))]
	public class Turret : MonoBehaviour {
		private Shooter _shoot;
		private GameActor _target;
		private Coroutine _shooting;
		private Structure _structure;

		private void Start() {
			_shoot = GetComponent<Shooter>();
			_structure = GetComponent<Structure>();
		}

		private void Update() {
			var newTarget = Physics
				.OverlapSphere(transform.position, _shoot.Range)
				.Select(c => c.GetComponent<Enemy>())
				.FirstOrDefault(c => c != null);

			if (newTarget == _target) {
				return;
			}

			if (_shooting != null) {
				StopCoroutine(_shooting);
			}

			_target = newTarget;
			_shooting = StartCoroutine(Shoot());
		}

		private IEnumerator Shoot() {
			yield return _shoot.Engage(_target);
			if (!_target || !_target.Alive) {
				GameManager.Instance.Produce(_structure.Production);
				ResourceCounter.Instance.Send(transform.position, _structure.Production.Keys);
			}
			_target = null;
		}
	}
}
