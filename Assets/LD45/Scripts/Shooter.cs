using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace LD45 {
	public class Shooter : MonoBehaviour {
		public float Range => _range;

		[SerializeField]
		private GameObject _projectileTemplate;

		[SerializeField, Tooltip("In seconds")]
		private float _cooldown;

		[SerializeField, Tooltip("In seconds")]
		private float _projectileDuration;

		[SerializeField]
		private float _range;

		[SerializeField]
		private int _damage;

		public YieldInstruction Engage(GameActor target) {
			IEnumerator EngageImpl() {
				while (target && target.Alive) {
					if (Vector3.Distance(target.transform.position, transform.position) > _range) {
						yield break;
					}

					var departure = transform.position;
					var landing = target.transform.position;
					var projectile = Instantiate(_projectileTemplate, departure, Quaternion.identity);
					projectile.transform.LookAt(landing);
					
					yield return projectile.transform
						.DOJump(landing, 0.5f, 1, _projectileDuration)
						.SetEase(Ease.Linear)
						.WaitForCompletion();

					Destroy(projectile);
					target.Health -= _damage;

					yield return new WaitForSeconds(_cooldown);
				}
			}

			return StartCoroutine(EngageImpl());
		}
	}
}
