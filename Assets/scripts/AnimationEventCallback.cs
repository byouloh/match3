using UnityEngine;
using System.Collections;

public class AnimationEventCallback : MonoBehaviour
{
	public bool autoDestroy = true;
	private ObjectCallback _callback;

	public void initialize(ObjectCallback callback)
	{
		this._callback = callback;
	}

	public void OnAnimationComplete()
	{
		if (_callback != null) {
			_callback(this);
		}

		if (autoDestroy) {
			Destroy(gameObject);
		}
	}
}
