using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Analytics;
using Random = UnityEngine.Random;

namespace LD45 {
	public class EnemyManager : SingletonBehaviour<EnemyManager> {
		[Serializable]
		private class WaveRule {
			public int MinimumStructures;
			public int SpawnCount;
		}

		public event Action<int> Begin;
		public event Action<int> End;

		public int CurrentWave { get; private set; }

		[SerializeField]
		private WaveRule[] _waves;

		[SerializeField]
		private Collider _spawnBounds;

		[SerializeField]
		private float _timeBetweenWaves = 1;

		[SerializeField]
		private Enemy _enemyTemplate;

		[SerializeField]
		private float _minStructureDistance;

		private IEnumerator Start() {
			_spawnBounds = FindObjectOfType<TerrainCollider>();

			for (var waveIndex = 0; waveIndex < _waves.Length; CurrentWave++) {
				var wave = _waves[waveIndex];
				var structures = new Structure[0];
				while (structures.Length < wave.MinimumStructures) {
					yield return new WaitForSeconds(_timeBetweenWaves);
					structures = FindObjectsOfType<Structure>()
						.Where(s => s.enabled)
						.ToArray();
				}

				Begin?.Invoke(CurrentWave);
				AnalyticsEvent.Custom("wave_start", new Dictionary<string, object> {
					{"counter", CurrentWave}
				});

				var alive = wave.SpawnCount;

				void OnDied() {
					alive--;
				}

				var start = FindWaveStartPoint(structures);
				var enemies = new Enemy[wave.SpawnCount];
				for (var i = 0; i < enemies.Length; i++) {
					enemies[i] = Instantiate(_enemyTemplate, start, Quaternion.identity);
					enemies[i].Died += OnDied;
				}

				yield return new WaitUntil(() => alive == 0);

				End?.Invoke(CurrentWave);
				AnalyticsEvent.Custom("wave_complete", new Dictionary<string, object> {
					{"counter", CurrentWave}
				});

				if (waveIndex < _waves.Length - 1) {
					waveIndex++;
				}

			}
		}

		private void OnDrawGizmos() {
			if (_spawnBounds == null) {
				return;
			}

			var bounds = _spawnBounds.bounds;
			Gizmos.color = new Color(1f, 0.5f, 0f, 0.25f);
			Gizmos.DrawCube(bounds.center, bounds.size + Vector3.one * 0.01f);
		}

		private Vector3 FindWaveStartPoint(Structure[] structures) {
			var bounds = _spawnBounds.bounds;

			Vector3 RandomInBounds() {
				var rand = Random.insideUnitSphere * bounds.extents.magnitude + bounds.center;
				var clamped = bounds.ClosestPoint(rand);
				return clamped;
			}

			NavMeshHit hit;

			var distance = _minStructureDistance;

			bool Acceptable(Structure s) {
				Debug.DrawLine(hit.position, s.transform.position, Color.red, 1, false);
				return Vector3.Distance(s.transform.position, hit.position) < distance;
			}

			while (NavMesh.SamplePosition(RandomInBounds(), out hit, _minStructureDistance * 5, NavMesh.AllAreas)) {
				if (!structures.Any(Acceptable)) {
					Debug.DrawLine(hit.position, hit.position + Vector3.up, Color.green, 1, false);
					break;
				}

				if (distance > _minStructureDistance / 3) {
					distance -= 0.5f;
				}
			}

			return hit.position;
		}
	}
}