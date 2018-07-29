using System;

namespace Assets.Ryooka.Scripts.General {
	public class Instance<T> where T : class {
		T instance;

		Func<T> generate;
		Func<T, bool> validate;
		Action<T> delete;

		public Instance(Func<T> generate, Func<T, bool> validate, Action<T> delete) {
			this.generate = generate;
			this.validate = validate;
			this.delete = delete;
		}

		public void Destroy() {
			if (instance != null) {
				delete(instance);
				instance = null;
			}
		}

		public void Initialize() {
			Destroy();
			instance = generate();
		}

		public T Value() {
			if (instance != null && validate(instance)) {
				return instance;
			} else {
				Initialize();
				return instance;
			}
		}
	}
}
