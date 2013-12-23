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
	/** Обработчик события по окончании взрыва. */
	private Callback _explodeCallback;

	/** Префаб анимации взрыва. */
    public GameObject explosionPrefab = null;

	/** Фишка может покинуть ячейку. */
	public abstract bool canLeave();

	/** Фишка может войти в ячейку. */
	public abstract bool canEnter();

	/** Фишка может пройти через ячейку. */
	public abstract bool canPass();

	/** Защищает ли блокирующий элемент содержимое от взрыва. */
	public abstract bool isProtecting();

	/**
	 * Взрывает блокирующий элемент.
	 * 
	 * @param callback обработчик событий по окончании взрыва
	 * 
	 * @return true, если началась анимация взрыва, иначе false
	 */
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
