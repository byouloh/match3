using UnityEngine;
using System.Collections;

public static class ChipFactory
{
	/**
	 * Создает фишку по типу.
	 * 
	 * @param blockerName название блокирующего элемента
	 * @param parent контейнер для создаваемого объекта ячейки
	 * 
	 * @return CellBlocker класс блокирующего элемента
	 * @throw System.ArgumentException, System.NullReferenceException
	 */
	public static Chip createNew(ChipType chipType, BonusType bonusType, GameObject parent)
	{
		string spriteName = null;

		switch (chipType) {
            case ChipType.RED:
				spriteName = "chipRedSprite";
				break;
				
			case ChipType.GREEN:
				spriteName = "chipGreenSprite";
				break;
				
			case ChipType.BLUE:
				spriteName = "chipBlueSprite";
				break;
				
			case ChipType.YELLOW:
				spriteName = "chipYellowSprite";
				break;
				
			case ChipType.ORANGE:
				spriteName = "chipOrangeSprite";
				break;
				
			case ChipType.PURPLE:
				spriteName = "chipPurpleSprite";
				break;
				
			default:
				throw new System.ArgumentException("Ошибка! Неизвестный параметр: " + (int)chipType, "chipType");
		}

		switch (bonusType) {
			case BonusType.NONE:
				break;
			case BonusType.HORIZONTAL_STRIP:
				spriteName += "_H";
				break;
			case BonusType.VERTICAL_STRIP:
				spriteName += "_V";
				break;
			case BonusType.SAME_TYPE:
				spriteName = "chipBonusSameSprite";
				break;
			default:
				break;
		}

		Chip res = createNew("chip", spriteName, chipType, bonusType, parent);

		return res;
	}

	/**
	 * Создает фишку по названию.
	 * 
	 * @param blockerName название блокирующего элемента
	 * @param parent контейнер для создаваемого объекта ячейки
	 * 
	 * @return CellBlocker класс блокирующего элемента
	 * @throw System.ArgumentException, System.NullReferenceException
	 */
    private static Chip createNew(string chipName, string spriteName, ChipType chipType, BonusType bonusType, GameObject parent)
	{
		GameObject prefab = Resources.Load<GameObject>("prefabs/chips/" + chipName);
		
		if (prefab == null) {
			throw new System.NullReferenceException("Ошибка! Не удалось загрузить префаб: " + chipName);
		}
		
		GameObject obj = (GameObject) UnityEngine.Object.Instantiate(prefab);

		if (parent != null) {
            obj.transform.parent        = parent.transform;
            obj.transform.localPosition = Vector3.zero;
		}
        
        Sprite sprite = Resources.Load<Sprite>("textures/chipSprites/" + spriteName);
        
		if (sprite == null) {
			UnityEngine.Object.Destroy(obj);
			throw new System.NullReferenceException("Ошибка! Не удалось загрузить префаб: " + spriteName);
		}

		obj.GetComponent<SpriteRenderer>().sprite = sprite;

		Chip res = obj.GetComponent<Chip>();
		
		if (res == null) {
			UnityEngine.Object.Destroy(obj);
			throw new System.NullReferenceException("Ошибка! На префабе нет компоненты CellBlocker");
		}

		res.type = chipType;
		res.bonusType = bonusType;

		return res;
	}
}
