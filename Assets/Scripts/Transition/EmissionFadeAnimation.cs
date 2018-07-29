using Assets.Ryooka.Scripts.EditorExtension;
using Assets.Ryooka.Scripts.Extension;
using Assets.Ryooka.Scripts.General;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Transition {
	public class EmissionFadeAnimation: MonoBehaviour {
		public class MaterialCached {
			public Material material;

			public Material srcMaterial;

			public StandardUtil.BlendMode srcMode;

			public Renderer renderer;
		}

		[Serializable]
		public class EmissionAnimation {
			public AnimationCurve curve;

			public Color dstColor;

			bool keyEnabled;
			
			// Don't change the emission key; this class caches it for performance.
			public void Update(float time, MaterialCached[] caches) {
				float value = curve.Evaluate(time);

				bool _keyEnabled = value != 0f;
				bool mustUpdateKeyState = false;

				if (_keyEnabled != keyEnabled) {
					keyEnabled = _keyEnabled;
					mustUpdateKeyState = true;
				}

				for (int i = 0; i < caches.Length; i++) {
					MaterialCached cache = caches[i];
					Material material = cache.material;
					Material srcMaterial = cache.srcMaterial;

					if (mustUpdateKeyState) {
						material.SetActiveKeyword(StandardUtil.KEY_EMISSION, keyEnabled);
					}

					Color srcColor = srcMaterial.GetEmissionColor();

					material.SetEmissionColor(new Color {
						r = (dstColor.r - srcColor.r) * value + srcColor.r,
						g = (dstColor.g - srcColor.g) * value + srcColor.g,
						b = (dstColor.b - srcColor.b) * value + srcColor.b,
					});
				}
			}

			// If you need to, use this method to manually update the cache.
			public void UpdateEmissionEnabled(bool enabled) {
				keyEnabled = enabled;
			}
		}

		[Serializable]
		public class TransparencyAnimation {
			public AnimationCurve curve;

			public float dstValue;

			bool transparent;
			bool invisible;

			// Don't change any properties related to transparency; this class caches them for performance.
			public void Update(float time, MaterialCached[] caches) {
				float value = curve.Evaluate(time);

				bool _transparent = value < 1f;
				bool invisible = Mathf.Approximately(value, 0f);
				bool mustUpdateBlendMode = false;

				if (_transparent != transparent) {
					transparent = _transparent;
					mustUpdateBlendMode = true;	
				}

				for (int i = 0; i < caches.Length; i++) {
					MaterialCached cache = caches[i];
					Material material = cache.material;
					Material srcMaterial = cache.srcMaterial;
					StandardUtil.BlendMode srcMode = cache.srcMode;

					if (mustUpdateBlendMode) {
						// If the material is not transparent by default:
						if (srcMode != StandardUtil.BlendMode.Cutout) {
							if (transparent) {
								// Make it Fade.
								material.SetBlendMode(StandardUtil.BlendMode.Fade);
							} else {
								// Turn back to its original transparency setting.
								material.SetBlendMode(srcMode);
							}
						}
					}
					
					Color dstColor = srcMaterial.GetColor(StandardUtil.COLOR);
					dstColor.a *= value;
					material.SetColor(StandardUtil.COLOR, dstColor);

					cache.renderer.enabled = !invisible;
				}
			}

			// If you need to, use this method to manually update the cache.
			public void UpdateFadeModeOn(bool on) {
				transparent = on;
			}
		}

		[UnfoldInInspector]
		public TwoWayTransition pp;
		
		public EmissionAnimation emission;
		
		public TransparencyAnimation transparency;
		
		MaterialCached[] materials;

		IEnumerator animationRunning;

		bool animatingForward;

		private void Reset() {
			emission.dstColor = Color.white;

			emission.curve.AddKey(new Keyframe(  0f, 0f, 0f, 0f));
			emission.curve.AddKey(new Keyframe(0.5f, 1f, 0f, 0f));
			emission.curve.AddKey(new Keyframe(  1f, 1f, 0f, 0f));

			transparency.curve.AddKey(new Keyframe(  0f, 1f, 0f, 0f));
			transparency.curve.AddKey(new Keyframe(0.5f, 1f, 0f, 0f));
			transparency.curve.AddKey(new Keyframe(  1f, 0f, 0f, 0f));
		}

		// Use this for initialization
		private void Awake() {
			List<MaterialCached> _materials = new List<MaterialCached>();

			foreach (var renderer in GetComponentsInChildren<Renderer>(true)) {
				foreach (var material in renderer.materials) { // not of "shared".
					if (material.IsOfStandardShader()) {
						_materials.Add(new MaterialCached {
							material = material,
							srcMaterial = Instantiate(material),
							srcMode = material.GetBlendMode(),
							renderer = renderer,
						});
					}
				}
			}

			materials = _materials.ToArray();
		}
		
		public void Play(bool forward) {
			animatingForward = forward;

			if (animationRunning == null) {
				animationRunning = DoPlay().EndWith(() => {
					animationRunning = null;
				});

				StartCoroutine(animationRunning);
			}
		}

		IEnumerator DoPlay() {
			while (true) {
				float position;
				bool doneAnimating;

				pp.Update(animatingForward, out position, out doneAnimating);

				if (doneAnimating) {
					break;
				}

				emission.Update(position, materials);
				transparency.Update(position, materials);

				yield return null;
			}
		}
	}
}
