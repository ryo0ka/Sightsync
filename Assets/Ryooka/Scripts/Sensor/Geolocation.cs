using System;
using UnityEngine;

namespace Assets.Ryooka.Scripts.Sensor {
	[Serializable]
	public sealed class Geolocation: ICloneable {
		//http://www.movable-type.co.uk/scripts/latlong.html
		static readonly double R = 6371e3;  // Earth radius in metres

		[SerializeField]
		float latitude, longitude;

		public Geolocation(float latitude, float longitude) {
			this.latitude = latitude;
			this.longitude = longitude;
		}

		public static Geolocation Last() {
			return new Geolocation(
				latitude: Input.location.lastData.latitude,
				longitude: Input.location.lastData.longitude);
		}

		public float Latitude {
			get { return latitude; }
		}

		public float Longitude {
			get { return longitude; }
		}

		//http://stackoverflow.com/questions/1369512/converting-longitude-latitude-to-x-y-on-a-map-with-calibration-points
		public Vector2 Map(float width, float height) {
			float y = (latitude  * height) / 180;
			float x = (longitude * width ) / 360;
			return new Vector2(x, y);
		}

		//http://www.movable-type.co.uk/scripts/latlong.html
		public double Distance(Geolocation that) {
			var φ1 = this.latitude * Mathf.Deg2Rad;
			var φ2 = that.latitude * Mathf.Deg2Rad;
			var Δφ = (that.latitude - this.latitude) * Mathf.Deg2Rad;
			var Δλ = (that.longitude - this.longitude) * Mathf.Deg2Rad;

			var a = Math.Sin(Δφ/2) * Math.Sin(Δφ/2) + Math.Cos(φ1) * Math.Cos(φ2) * Math.Sin(Δλ/2) * Math.Sin(Δλ/2);
			var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1-a));
			var d = R * c;

			return d;
		}

		public Geolocation Clone(float? latitude = null, float? longitude = null) {
			return new Geolocation(
				latitude: latitude.GetValueOrDefault(Latitude),
				longitude: longitude.GetValueOrDefault(Longitude));
		}

		object ICloneable.Clone() {
			return Clone();
		}

		public override int GetHashCode() {
			return (int)latitude + (int)longitude; //TODO
		}

		bool Equals(Geolocation that) {
			return this.latitude == that.latitude
				&& this.longitude == that.longitude;
		}

		public override bool Equals(object obj) {
			var that = obj as Geolocation;
			return that != null && this.Equals(that);
		}

		public override string ToString() {
			return string.Format("Latitude: {0}, Longitude: {1}", latitude, longitude);
		}
	}
}
