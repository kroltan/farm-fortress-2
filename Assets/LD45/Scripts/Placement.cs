using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Analytics;

namespace LD45 {
	public class Placement : MonoBehaviour {
		[SerializeField]
		private float _anglePerturbation;

		[SerializeField]
		private float _scalePerturbation;

		[SerializeField]
		private LayerMask _layers;

		private TaskCompletionSource<Structure> _placement;
		private Structure _blueprint;
		private Camera _camera;
		private readonly RaycastHit[] _hits = new RaycastHit[4];

		public Task<Structure> Place(Structure blueprint) {
			_placement = new TaskCompletionSource<Structure>(
				TaskCreationOptions.AttachedToParent
				| TaskCreationOptions.RunContinuationsAsynchronously
			);

			blueprint.enabled = false;
			_blueprint = Instantiate(blueprint);
			_blueprint.name = blueprint.name;
			_blueprint.transform.Rotate(0, Random.value * _anglePerturbation, 0, Space.Self);
			_blueprint.transform.localScale += Random.value * _scalePerturbation * Vector3.one;
			_blueprint.Blueprint = true;

			return _placement.Task;
		}

		private void Awake() {
			_camera = Camera.main;
		}

		private void Update() {
			if (_blueprint == null) {
				return;
			}

			var ray = _camera.ScreenPointToRay(Input.mousePosition);
			var hitCount = Physics.RaycastNonAlloc(ray, _hits, 100, _layers, QueryTriggerInteraction.Ignore);
			if (
				hitCount > 0
				&& NavMesh.SamplePosition(_hits[hitCount - 1].point, out var navHit, 3, NavMesh.AllAreas)
			) {
				_blueprint.transform.position = navHit.position;
			}

			if (hitCount > 0 && Input.GetMouseButtonDown(0)) {
				Confirm();
			}

			if (Input.GetMouseButtonDown(1)) {
				Cancel();
			}
		}

		private void Confirm() {
			AnalyticsEvent.Custom("build", new Dictionary<string, object> {
				{"structure", _blueprint.name},
				{"instance", _blueprint.gameObject.GetInstanceID()}
			});

			_blueprint.Blueprint = false;
			_blueprint.enabled = true;
			_placement.SetResult(_blueprint);
			_placement = null;
			_blueprint = null;
		}

		private void Cancel() {
			Destroy(_blueprint.gameObject);
			_placement.SetCanceled();
			_placement = null;
			_blueprint = null;
		}
	}
}