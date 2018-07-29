using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class Bools : MonoBehaviour
{
	[SerializeField]
	public class BoolEvent : UnityEvent<bool> {}

	public BoolEvent onBool;
	public UnityEvent onTrue;
	public UnityEvent onFalse;

	public void Invoke(bool b) {
		onBool.Invoke (b);
		if (b)
			onTrue.Invoke ();
		else
			onFalse.Invoke ();
	}
}