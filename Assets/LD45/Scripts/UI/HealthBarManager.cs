using UnityEngine;

namespace LD45.UI {
	public class HealthBarManager : SingletonBehaviour<HealthBarManager> {
		[SerializeField]
		private HealthBar _barTemplate;

		public void Register(GameActor actor) {
			var bar = Instantiate(_barTemplate, transform);
			bar.Bind(actor);
		}
	}
}
