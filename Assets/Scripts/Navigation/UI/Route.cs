using Assets.Ryooka.Scripts.General;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Navigation.UI {
	// Consumes a LineRenderer so it will function as a route
	// that fades out from its tail as a player proceeds to the end.
	// After running this script, the attached LineRenderer may be left empty.
	[RequireComponent(typeof(LineRenderer))]
	public class Route: MonoBehaviour {
		public bool IsRunning { get; private set; }

		public bool HasFinished { get; private set; }

		public LineRenderer Line { get; private set; }

		public float OffsetSpeed { get; set; }
		
		// player farther from all the points than this distance will be ignored.
		public float MaxDistance { get; set; }

		// Distance will be calculated in this frequency.
		public float IntervalSeconds { get; set; }

		// Invoked when a player reaches the end of this route.
		public event System.Action OnEnd;
		
		IEnumerator Start() {
			if (IsRunning) {
				Debug.LogAssertion("Route is already running.");
				yield break;
			}

			IsRunning = true;

			// guaranteed to succeed (RequireComponent).
			Line = GetComponent<LineRenderer>();

			// Starts the beltconveyer animation of texture.
			StartCoroutine(Animate());

			// in each interval, check where on the (whole) line the user is the closest to,
			// then create a new point at the position and delete every point behind it.
			// stop when there is less than one point left in the list.
			while (Line.positionCount >= 2) {
				if (!IsRunning) {
					break; // straight into OnEnd.
				}

				float shortestDistance = float.MaxValue;
				int? closestPointIndex = null; // points behind this index will be removed.
				Vector3? closestNewPoint = null; // after removal, this will be added to [0].

				for (int i = 0; i < Line.positionCount - 1; i++) {
					Vector3 point1 = Line.GetPosition(i);
					Vector3 point2 = Line.GetPosition(i + 1);
					Vector3 pointU = Camera.main.transform.position; // player position
					Vector3 pointP = Math3d.ProjectPointOnLineSegment(point1, point2, pointU);
					float distance = (pointU - pointP).magnitude;

					if (distance > MaxDistance) {
						// pass; player is too far.
					} else if (distance < shortestDistance) {
						shortestDistance = distance;

						if (pointP == point1) {
							// when the 1st point is the closest:
							closestPointIndex = i;
						} else if (pointP == point2) {
							// when the 2nd point is the closest:
							closestPointIndex = i + 1;
						} else {
							// when the closest point is in the middle:
							// split the segment by 1 meter, which is the length of one tile.
							// get the closest division point and use it as the closest point.
							// this is done in order to prevent tiles' hiccups.
							int length = (int)Vector3.Distance(point1, pointP);
							closestNewPoint = Vector3.MoveTowards(point1, point2, length);

							// Use the previous (point1) index as a placeholder
							// which will be replaced by the new point in later process.
							closestPointIndex = i;
						}
					}
				}

				if (!closestPointIndex.HasValue) {
					// Player is too far.
				} else { 
					if (closestNewPoint.HasValue) {
						// replace the placeholder with the new closest point.
						Line.SetPosition(closestPointIndex.Value, closestNewPoint.Value);
					}

					// points behind this index will be removed from the line.
					int removedPointIndex = closestPointIndex.Value - 1;

					if (removedPointIndex < 0) {
						// there is nothing behind the first point.
					} else {
						// index : 0 1 2 3 4 5 6 7 8
						// before: x x x x o o o o o
						// after : o o o o o        where x: removed, o: alive.
						for (int ib = removedPointIndex + 1; ib < Line.positionCount; ib++) {
							int ia = ib - (removedPointIndex + 1);
							Line.SetPosition(ia, Line.GetPosition(ib));
						}

						// Make sure nulls won't be left in the tail.
						Line.positionCount -= removedPointIndex + 1;
					}
				}

				// wait for specified length of time until the next calculation.
				yield return new WaitForSeconds(IntervalSeconds);
			}

			IsRunning = false;
			HasFinished = true;

			// Flush out.
			Line.positionCount = 0;

			Debug.Log("Route completed: " + name);
			OnEnd();

			Destroy(gameObject);
		}

		// Animate the texture's offset so it looks like a beltconveyer.
		IEnumerator Animate() {
			const string MAINTEX = "_MainTex";
			float currentOffset = 0f;

			while (true) {
				currentOffset += Time.deltaTime * OffsetSpeed;
				currentOffset %= 100f;
				
				var offset = new Vector2(100f - currentOffset, 0);
				Line.material.SetTextureOffset(MAINTEX, offset);

				yield return null;
			}
		}

		public void Halt() {
			IsRunning = false;
		}
	}
}

