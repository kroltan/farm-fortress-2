using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace LD45.UI {
	public class StructurePalette : MonoBehaviour {
		private const string ThumbnailAssetName = "IngameThumbnail";

		[SerializeField]
		private PaletteButton _buttonTemplate;

		[SerializeField]
		private RectTransform _container;

		[SerializeField]
		private Placement _placement;

		[SerializeField]
		private Structure[] _structures;

		[SerializeField, Layer]
		private int _thumbnailLayer;

		[SerializeField]
		private Material _thumbnailPedestal;

		[SerializeField]
		private KeyCode[] _availableShortcuts;

		private void Start() {
			for (var index = 0; index < _structures.Length; index++) {
				var structure = _structures[index];
				var button = Instantiate(_buttonTemplate, _container);
				button.Pressed += OnPalettePressed;
				button.Structure = structure;
				if (index < _availableShortcuts.Length) {
					button.Shortcut = _availableShortcuts[index];
				}
			}

			StartCoroutine(CollectThumbnails());
		}
		
		private IEnumerator CollectThumbnails() {
			for (var index = 0; index < _structures.Length; index++) {
				var structure = _structures[index];
				var go = structure.gameObject;

				if (go == null) {
					continue;
				}

				var instance = Instantiate(go,  new Vector3(10 * index, 1000, 0), Quaternion.identity);
				var texture = new RenderTexture(256, 384, 24, DefaultFormat.HDR);

				var scale = instance.transform.localScale;
				var fit = Mathf.Max(scale.x, scale.y, scale.z);

				var camera = new GameObject($"{go.name}-Drawer")
					.AddComponent<Camera>();

				var pedestal = GameObject.CreatePrimitive(PrimitiveType.Cube);
				pedestal.transform.parent = instance.transform;
				pedestal.transform.localPosition = new Vector3(0, -2 * fit, 0);
				pedestal.transform.localScale = new Vector3(3, 4 * fit, 3);
				pedestal.GetComponent<MeshRenderer>().material = _thumbnailPedestal;

				camera.enabled = false;
				camera.clearFlags = CameraClearFlags.Color;
				camera.transform.parent = instance.transform;
				camera.transform.localPosition = new Vector3(5, 5 * fit, 5);
				camera.transform.LookAt(instance.transform);
				camera.orthographic = true;
				camera.targetTexture = texture;
				camera.orthographicSize = 2 * fit / camera.aspect;
				camera.cullingMask = 1 << _thumbnailLayer;

				ChangeLayerDeep(instance, _thumbnailLayer);

				camera.Render();
				yield return null;
				
				_container.GetChild(index).GetComponent<PaletteButton>().Thumbnail = texture;
			}
		}

		private void ChangeLayerDeep(GameObject obj, int layer) {
			obj.layer = layer;

			foreach (Transform child in obj.transform) {
				ChangeLayerDeep(child.gameObject, layer);
			}
		}

		private async void OnPalettePressed(PaletteButton sender) {
			try {
				await _placement.Place(sender.Structure);
				GameManager.Instance.Consume(sender.Structure.Cost);
			} catch (OperationCanceledException) {
				// Natural
			}
		}
	}
}
