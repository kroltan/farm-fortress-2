using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace LD45 {
	[RequireComponent(typeof(NavMeshAgent))]
	[RequireComponent(typeof(Shooter))]
	public class Enemy : GameActor {
		[SerializeField]
		private LayerMask _structureLayers;

		[SerializeField]
		private float _giveUpTime;

		private NavMeshAgent _agent;
		private Shooter _shoot;

		private void Start() {
			_agent = GetComponent<NavMeshAgent>();
			_shoot = GetComponent<Shooter>();

			StartCoroutine(Fight());
		}

		private IEnumerator Fight() {
			while (true) {
				yield return new WaitForSeconds(0.5f);
				var structures = FindObjectsOfType<Structure>()
					.Where(s => _structureLayers.Contains(s.gameObject.layer))
					.ToArray();
				if (structures.Length == 0) {
					continue;
				}

				var target = structures[Random.Range(0, structures.Length)];

				if (target.Blueprint) {
					continue;
				}

				_agent.stoppingDistance = _shoot.Range - _agent.radius;
				if (!_agent.SetDestination(target.transform.position)) {
					continue;
				}

				var gaveUp = false;
				var startTime = Time.time;
				yield return new WaitUntil(() => {
					var keepGoing = Time.time - startTime < _giveUpTime;
					var inRange = Vector3.Distance(target.transform.position, transform.position) < _shoot.Range;

					if (keepGoing) {
						return inRange;
					}

					gaveUp = true;
					return true;
				});

				if (gaveUp) {
					continue;
				}
				
				yield return _shoot.Engage(target);
			}
		}
	}
}