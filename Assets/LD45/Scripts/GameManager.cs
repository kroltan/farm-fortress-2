using System;
using System.Collections.Generic;
using System.Linq;
using LD45.Model;
using UnityEngine;

namespace LD45 {
	public class GameManager : SingletonBehaviour<GameManager> {

		public event Action StockpileChanged {
			add {
				StockpileChangedInternal += value;
				value.Invoke();
			}
			remove => StockpileChangedInternal -= value;
		}

		public IReadOnlyDictionary<Resource, int> Stockpile => _stockpile;

		private event Action StockpileChangedInternal;

		[SerializeField]
		private ResourceValue[] _initialContents;

		private Dictionary<Resource, int> _stockpile;
		private bool _stockpileChangedLastFrame;

		public bool Increment(Resource resource, int amount) {
			if (!Stockpile.ContainsKey(resource)) {
				throw new InvalidOperationException("Adding new resources at runtime is unsupported");
			}

			_stockpileChangedLastFrame = true;

			Debug.Log($"Changed {resource.name} by {amount}");

			var value = _stockpile[resource] + amount;
			if (value < 0) {
				_stockpile[resource] = 0;
				return false;
			}
			
			_stockpile[resource] = value;
			return true;
		}

		public bool Consume(IReadOnlyDictionary<Resource, int> amounts) => MassIncrement(amounts, -1);
		public bool Produce(IReadOnlyDictionary<Resource, int> amounts) => MassIncrement(amounts, 1);

		public bool HasEnough(IReadOnlyDictionary<Resource, int> requested) {
			return requested.All(kvp => _stockpile[kvp.Key] >= kvp.Value);
		}

		private void Awake() {
			_stockpile = _initialContents.ToDictionary(
				value => value.Resource,
				value => value.Value
			);

			StockpileChangedInternal?.Invoke();
		}

		private void Update() {
			if (_stockpileChangedLastFrame) {
				StockpileChangedInternal?.Invoke();
			}

			_stockpileChangedLastFrame = false;
		}


		public bool MassIncrement(IReadOnlyDictionary<Resource, int> amounts, int factor) {
			var success = true;

			foreach (var (resource, value) in amounts) {
				success = success && Increment(resource, value * factor);
			}

			return success;
		}
	}
}
