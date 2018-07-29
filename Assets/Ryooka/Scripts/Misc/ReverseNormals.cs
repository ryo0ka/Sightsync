using UnityEngine;

namespace Assets.Ryooka.Scripts.Misc {
    [RequireComponent(typeof(MeshFilter))]
    [ExecuteInEditMode]
    public class ReverseNormals: MonoBehaviour {

		[SerializeField]
		bool execute;

        void Reset() {
        }

		void OnValidate() {
			if (execute) {
				Reverse(GetComponent<MeshFilter>());
				execute = false;
			}
		}

        public static void Reverse(MeshFilter filter) {
			Mesh mesh = filter.sharedMesh;

			Vector3[] normals = mesh.normals;

			for (int i = 0; i < normals.Length; i++)
				normals[i] = -normals[i];

			mesh.normals = normals;

			for (int m = 0; m < mesh.subMeshCount; m++) {
				int[] triangles = mesh.GetTriangles(m);

				for (int i = 0; i < triangles.Length; i += 3) {
					int temp = triangles[i + 0];

					triangles[i + 0] = triangles[i + 1];
					triangles[i + 1] = temp;
				}
				mesh.SetTriangles(triangles, m);

			}
		}
    }
}