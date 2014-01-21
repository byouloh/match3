using UnityEngine;
using System.Collections;

/**
 * Фабрика для создания блокирующих элементов.
 * 
 * @author Timur Bogotov timur@e-magic.org
 * @author Azamat Bogotov azamat@e-magic.org
 */
public static class BlockFactory
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
        
        return createNew(prefabName, parent, Game.getInstance().level.blockersInfo[(int)type].explodePoints);
    }
    
    /**
     * Создает блокирующий элемент по названию.
     * 
     * @param blockerName   название блокирующего элемента
     * @param parent        контейнер для создаваемого объекта ячейки
     * @param explodePoints количество очков за взрыв объекта
     * 
     * @return CellBlocker класс блокирующего элемента
     * @throw System.ArgumentException, System.NullReferenceException
     */
    public static CellBlocker createNew(string blockerName, GameObject parent, uint explodePoints)
    {
        GameObject prefab = Resources.Load<GameObject>("prefabs/blockers/" + blockerName);
        
        if (prefab == null) {
            throw new System.NullReferenceException("Ошибка! Не удалось загрузить префаб: " + blockerName);
        }
        
        GameObject blockerObject = (GameObject)UnityEngine.Object.Instantiate(prefab);
        
        if (parent != null) {
            blockerObject.transform.parent        = parent.transform;
            blockerObject.transform.localPosition = Vector3.zero;
        }
        
        CellBlocker blocker = blockerObject.GetComponent<CellBlocker>();
        
        if (blocker == null) {
            UnityEngine.Object.Destroy(blockerObject);
            throw new System.NullReferenceException("Ошибка! На префабе нет компоненты CellBlocker");
        }
        
        blocker.setExplodePoints(explodePoints);
        
        return blocker;
    }
}
