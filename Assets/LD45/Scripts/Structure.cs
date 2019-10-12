using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LD45.Model;
using LD45.UI;
using UnityEngine;
using UnityEngine.AI;

namespace LD45 {
	public class Structure : GameActor {
		private static readonly int ColorId = Shader.PropertyToID("_BaseColor");

		public bool Blueprint {
			get => _blueprint;
			set {
				_blueprint = value;

				var color = _blueprint
					? new Color(0f, 1f, 1f, 0.5f)
					: Color.white;

				var block = new MaterialPropertyBlock();
				block.SetColor(ColorId, color);

				GetComponentInChildren<NavMeshObstacle>().enabled = !_blueprint;

				foreach (var renderer in GetComponentsInChildren<MeshRenderer>()) {
					renderer.SetPropertyBlock(block);
				}

				foreach (var renderer in GetComponentsInChildren<SkinnedMeshRenderer>()) {
					renderer.SetPropertyBlock(block);
				}
			}
		}

		public IReadOnlyDictionary<Resource, int> Cost => _cost.ToDictionary(
			value => value.Resource,
			value => value.Value
		);

		public IReadOnlyDictionary<Resource, int> Consumption => _consumption.ToDictionary(
			value => value.Resource,
			value => value.Value
		);

		public IReadOnlyDictionary<Resource, int> Production => _production.ToDictionary(
			value => value.Resource,
			value => value.Value
		);

		public string Description => _description;

		[SerializeField, TextArea]
		private string _description;

		[SerializeField]
		private ResourceValue[] _cost;

		[SerializeField]
		private ResourceValue[] _consumption;

		[SerializeField]
		private ResourceValue[] _production;

		[SerializeField, Tooltip("In seconds")]
		private float _productionCycleLength;

		private bool _blueprint;
		private Coroutine _productionRoutine;

		protected override void OnEnable() {
			base.OnEnable();
			StructureStatusManager.Instance.Register(this);
			_productionRoutine = StartCoroutine(ProductionCycle());
		}

		private void OnDisable() {
			StopCoroutine(_productionRoutine);
		}

		private IEnumerator ProductionCycle() {
			var game = GameManager.Instance;
			var collect = GetComponent<CollectableStructure>();

			while (true) {
				yield return new WaitUntil(() => game.HasEnough(Consumption));
				yield return StructureStatusManager.Instance.Timer(this, _productionCycleLength);
				yield return new WaitUntil(() => game.HasEnough(Consumption));
				if (collect != null) {
					yield return collect.Display();
				}

				// Consume
				var success = game.Consume(Consumption);

				// If any resource is missing, don't produce
				if (!success) {
					continue;
				}

				game.Produce(Production);
				ResourceCounter.Instance.Send(transform.position, Production.Keys);
			}
		}
	}
}
