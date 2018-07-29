#if false

using UnityEngine;

namespace Assets.Ryooka.Scripts.External {
	public class EasyMovieTextureInitializationTest : MonoBehaviour {
		[SerializeField]
		Mesh mesh;

		[SerializeField]
		Material material;

		[SerializeField]
		Shader shader;

		[SerializeField]
		Vector3 scale;

		[SerializeField]
		Vector3 direction;

		[SerializeField]
		string filePath;

		[SerializeField]
		MediaPlayerCtrl prefab;

		void Start() {
			InstantiatePlayer();
		}

		public MediaPlayerCtrl InstantiatePlayer() {
			//var videoGO = new GameObject();
			////videoGO.AddComponent<MeshFilter>().sharedMesh = mesh;
			////videoGO.AddComponent<MeshRenderer>().material = material;
			////videoGO.GetComponent<MeshRenderer>().material.shader = shader;
			////videoGO.GetComponent<MeshRenderer>().RenderAsBackground();
			//var video = videoGO.AddComponent<MediaPlayerCtrl>();
			////video.transform.localScale = scale;
			////video.transform.localEulerAngles = direction;
			//video.m_bLoop = true;
			//video.m_TargetMaterial = new GameObject[] { gameObject };
			//video.m_strFileName = filePath;
			var video = Instantiate(prefab);
			return video;

			//TODO flipped?
		}
	}
}

#endif