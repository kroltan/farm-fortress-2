using System;
using System.Collections.Generic;
using LD45.UI;
using UnityEngine;
using UnityEngine.Analytics;

namespace LD45 {
	public class GameActor : MonoBehaviour {
		public event Action HealthChanged;

		public event Action Died;

		public bool Alive => !_dead;

		public int MaxHealth { get; set; }

		public int Health {
			get => _health;
			set {
				_health = value;
				HealthChanged?.Invoke();

				if (!(Health <= 0)) {
					return;
				}

				if (gameObject != null) {
					AnalyticsEvent.Custom("actor_died", new Dictionary<string, object> {
						{"instance", gameObject.GetInstanceID()}
					});
				}

				_dead = true;
				Died?.Invoke();
				Destroy(gameObject);
			}
		}

		[SerializeField]
		private int _health;

		private bool _dead;

		protected virtual void OnEnable() {
			MaxHealth = Health;
			HealthBarManager.Instance.Register(this);
		}
	}
}