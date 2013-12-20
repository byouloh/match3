using UnityEngine;
using System.Collections;

public enum ChipType
{
	RED = 0,
	GREEN,
	BLUE,
	YELLOW,
	ORANGE,
	PURPLE
}

public enum BonusType
{
	NONE = 0,			// Нет
	HORIZONTAL_STRIP,	// Горизонтальные полоски, уничтожает строку
	VERTICAL_STRIP,		// Вертикальные полоски, уничтожает столбец
	SAME_TYPE			// Уничтожает фишки определенного типа
}

public class Chip : MonoBehaviour, IExplodable
{
	public GameObject explosionPrefab;
	public ChipType type = ChipType.RED;
	public BonusType bonusType = BonusType.NONE;

	private Callback _explodeCallback;

	//
	public bool explode(Callback callback)
	{
		_explodeCallback = callback;
		
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

	private void _onExplodeAnimationComplete(Object self)
	{
		if (_explodeCallback != null) {
			_explodeCallback();
		}
	}
}
