using System;
using UnityEngine;
using UnityEngine.UI;
using Assets.Ryooka.Scripts.Extension;
using Assets.Ryooka.Scripts.General;

namespace Assets.Ryooka.Scripts.UI {
	[Serializable]
	public class TransformController : MonoBehaviour {
		public Transform target;

		public Transform pivot;

		public bool angleByPivot;

		[Header("Controller mode is enabled runtime")]
		public Slider2d horizontalSlider;

		public ControllerSlider heightSlider;

		public ControllerSlider yawSlider;

		public Slider speedSlider;

		public Toggle angleByPivotToggle;

		public Button resetButton;
		
		public Vector3 SumDisplacement { get; private set; }
		public float SumRotation { get; private set; }

		private float speed;

		private Vector3 Position {
			get {
				return target.localPosition;
			}
			set {
				SumDisplacement += value - Position; // record changes
				target.localPosition = value;
			}
		}

		private float Angle {
			get {
				return target.localEulerAngles.y;
			}
			set {
				SumRotation += value - Angle; // record changes
				target.SetLocalEulerAngles(y: value);
			}
		}

		private void Start() {
			if (horizontalSlider) {
				horizontalSlider.controllerMode = true;

				horizontalSlider.onValueChanged.AddListener(delta => {
					Move(x: delta.x, z: delta.y);
				});
			}
			
			if (heightSlider) {
				heightSlider.onValueChanged.AddListener(n => {
					Move(y: n);
				});
			}

			if (yawSlider) {
				yawSlider.onValueChanged.AddListener(n => {
					Rotate(n);
				});
			}

			if (resetButton) {
				resetButton.onClick.AddListener(() => {
					ResetTransform();
				});
			}

			if (speedSlider) {
				speedSlider.onValueChanged.AddListener(speed => {
					this.speed = speed;
				});
			}

			if (angleByPivotToggle) {
				angleByPivotToggle.onValueChanged.AddListener(enable => {
					angleByPivot = enable;
				});
			}

			speed = (speedSlider) ? speedSlider.value : 1f;
			angleByPivot = (angleByPivotToggle) ? angleByPivotToggle.isOn : angleByPivot;
		}

		// x/y/z must be delta
		public void Move(float? x = null, float? y = null, float? z = null) {
			float _x = x.GetValueOrDefault(0f);
			float _y = y.GetValueOrDefault(0f);
			float _z = z.GetValueOrDefault(0f);

			Vector3 displacement = new Vector3(_x, _y, _z) * Time.deltaTime * speed;

			if (angleByPivot) {
				float angle = pivot.eulerAngles.y;
				displacement = Quaternion.AngleAxis(angle, Vector3.up) * displacement;
			}

			Position += displacement;
		}

		public void Rotate(float deltaAngle) {
			// Moves target around pivot. (Doens't rotate target.)
			Position = VectorR.RotateAroundPivot(
				point : Position, 
				pivot : target.InverseTransformPoint(pivot.position), 
				angles: new Vector3(0, deltaAngle, 0) * Time.deltaTime * speed);
			
			// Rotates the target.
			Angle += deltaAngle;
		}

		// Undo all the changes done by this controller to the target's transformation.
		public void ResetTransform() {
			Position -= SumDisplacement;
			Angle -= SumRotation;
		}
	}
}
