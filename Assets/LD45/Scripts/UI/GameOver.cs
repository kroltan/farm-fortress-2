using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LD45.UI {
	public class GameOver : MonoBehaviour {
		[SerializeField]
		private CanvasGroup _group;

		[SerializeField]
		private Graphic[] _fades;

		[SerializeField]
		private TMP_Text _waveCounter;

		[SerializeField]
		private Button _restart;

		[SerializeField]
		private Button _changeLevel;

		[SerializeField, Scene]
		private int _menuScene;

		private int _alive;

		private void Start() {
			_group.alpha = 0;
			_group.blocksRaycasts = false;

			_restart.onClick.AddListener(() => StartCoroutine(Reload()));
			_changeLevel.onClick.AddListener(() => StartCoroutine(ChangeLevel()));
			StructureStatusManager.Instance.Registered += OnRegistered;
		}

		private void OnRegistered(Structure structure) {
			_alive++;
			structure.Died += OnDied;
		}

		private void OnDied() {
			_alive--;

			if (_alive == 0 && !FindObjectsOfType<PaletteButton>().Any(b => b.Buyable)) {
				StartCoroutine(Show());
			}
		}

		private IEnumerator Show() {
			const float step = 0.4f;

			AnalyticsEvent.LevelComplete(SceneManager.GetActiveScene().name);

			foreach (var graphic in _fades) {
				graphic.color = graphic.color.Change(a: 0);
			}

			_waveCounter.text = string.Format(_waveCounter.text, EnemyManager.Instance.CurrentWave);
			_group.blocksRaycasts = true;

			yield return _group.DOFade(1, step);

			foreach (var graphic in _fades) {
				yield return graphic.DOFade(1f, step).WaitForCompletion();
			}
		}

		private IEnumerator Reload() {
			yield return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
		}

		private IEnumerator ChangeLevel() {
			yield return SceneManager.LoadSceneAsync(_menuScene, LoadSceneMode.Single);
			yield return null;
			yield return null;
			FindObjectOfType<Splash>().LevelSelect = true;
			yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
		}
	}
}