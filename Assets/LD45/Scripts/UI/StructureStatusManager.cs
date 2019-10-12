using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LD45.UI {
	public class StructureStatusManager : SingletonBehaviour<StructureStatusManager> {
		public event Action<Structure> Registered;

		[SerializeField]
		private FollowWorldSpace _groupTemplate;

		[SerializeField]
		private ResourceMissing _missingTemplate;

		[SerializeField]
		private Timer _timerTemplate;

		private readonly Dictionary<Structure, RectTransform> _groups = new Dictionary<Structure, RectTransform>();

		public void Register(Structure structure) {
			var group = Instantiate(_groupTemplate, transform);
			group.Target = structure.transform;

			foreach (var (resource, value) in structure.Consumption) {
				var missing = Instantiate(_missingTemplate, group.transform);
				missing.Bind(resource, value);
			}

			structure.Died += () => {
				Destroy(group.gameObject);
				_groups.Remove(structure);
			};

			_groups[structure] = (RectTransform) group.transform;
			Registered?.Invoke(structure);
		}

		public void AddSpecial(Structure structure, RectTransform widget) {
			widget.SetParent(_groups[structure]);
		}

		public YieldInstruction Timer(Structure structure, float duration) {
			var timer = Instantiate(_timerTemplate, _groups[structure]);
			var icon = structure.Production.Keys.First().Icon;
			return timer.Run(icon, duration);
		}
	}
}
