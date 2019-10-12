using System.Collections;
using LD45.Model;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

namespace LD45.UI {
	public class LevelList : MonoBehaviour {
		[SerializeField]
		private Level[] _levels;

		[SerializeField]
		private LevelButton _buttonTemplate;

		[SerializeField]
		private Loading _loading;

		private void Start() {
			foreach (var level in _levels) {
				var button = Instantiate(_buttonTemplate, transform);
				button.Level = level;
				button.Selected += () => StartCoroutine(Go(level));
			}
		}

		private IEnumerator Go(Level level) {
			AnalyticsEvent.LevelStart(level.Scene);

			_loading.Background = level.Icon;
			_loading.Description = level.name;
			yield return _loading.ShowWhile(SceneManager.LoadSceneAsync(level.Scene, LoadSceneMode.Single));
		}
	}
}
