namespace Assets.Scripts.Tango {
	public class StartupUI : TangoEventManager.StartupUI {
		public override void OnIntended() {
			gameObject.SetActive(true);
		}

		public override void OnStarted() {
			gameObject.SetActive(false);
		}
	}
}
