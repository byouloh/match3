using UnityEngine;
using System.Collections;

/** Фабрика для создания блокирующих элементов. */
public static class BlockerFactory
{
	/**
	 * Создает блокирующий элемент по типу.
	 * 
	 * @param type тип блокирующего элемента
	 * @param parent контейнер для создаваемого объекта ячейки
	 * 
	 * @return CellBlocker класс блокирующего элемента
	 * @throw System.ArgumentException, System.NullReferenceException
	 */
	public static CellBlocker createNew(BlockerType type, GameObject parent)
	{
		string prefabName = null;

		switch (type) {
			case BlockerType.NONE:
				prefabName = "blockerNone";
				break;

			case BlockerType.WRAP:
                prefabName = "blockerWrap";
				break;

			case BlockerType.WRAP2:
                prefabName = "blockerWrap2";
				break;

			case BlockerType.CHAIN:
				prefabName = "blockerChain";
				break;

			case BlockerType.CHAIN2:
                prefabName = "blockerChain2";
				break;

			default:
				throw new System.ArgumentException("Ошибка! Неизвестный параметр: " + (int)type, "type");
		}

		return createNew(prefabName, parent);
	}

	/**
	 * Создает блокирующий элемент по названию.
	 * 
	 * @param blockerName название блокирующего элемента
	 * @param parent контейнер для создаваемого объекта ячейки
	 * 
	 * @return CellBlocker класс блокирующего элемента
	 * @throw System.ArgumentException, System.NullReferenceException
	 */
    public static CellBlocker createNew(string blockerName, GameObject parent)
	{
		GameObject prefab = Resources.Load<GameObject>("prefabs/blockers/" + blockerName);

		if (prefab == null) {
			throw new System.NullReferenceException("Ошибка! Не удалось загрузить префаб: " + blockerName);
		}
		
		GameObject obj = (GameObject) UnityEngine.Object.Instantiate(prefab);

        if (parent != null) {
            obj.transform.parent        = parent.transform;
			obj.transform.localPosition = Vector3.zero;
		}

		CellBlocker res = obj.GetComponent<CellBlocker>();
		
		if (res == null) {
			UnityEngine.Object.Destroy(obj);
			throw new System.NullReferenceException("Ошибка! На префабе нет компоненты CellBlocker");
		}
		
		return res;
	}
}
