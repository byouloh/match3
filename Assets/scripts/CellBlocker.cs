using UnityEngine;
using System.Collections;

public enum BlockerType
{
	NONE = 0,
	WRAP,
	WRAP2,
	CHAIN,
	CHAIN2
};

public abstract class CellBlocker : MonoBehaviour, IExplodable
{
	private Callback _explodeCallback;

    public GameObject explosionPrefab = null;
	public abstract bool canLeave();
	public abstract bool canEnter();
	public abstract bool canPass();

	public bool explode(Callback callback)
    {
		_explodeCallback = callback;

        if (explosionPrefab != null) {
            GameObject gm = (GameObject) Instantiate(explosionPrefab);

			AnimationEventCallback cb = gm.GetComponent<AnimationEventCallback>();

			if (cb != null) {
				cb.initialize(_onExplodeAnimationComplete);
			} else {
				Debug.LogError("Не найдент компонент: AnimationEventCallback");
			}

            if (gameObject.transform.parent != null) {
				gm.transform.parent = gameObject.transform.parent;
				gm.transform.localPosition = Vector3.zero;
            }
            
            return true;
        }
        
        return false;
    }

	private void _onExplodeAnimationComplete(Object self)
	{
		if (_explodeCallback != null) {
			_explodeCallback();
		}
	}

	public virtual bool hasNext()
	{
		return false;
	}

	public virtual BlockerType getNext()
	{
		return BlockerType.NONE;
	}
}
