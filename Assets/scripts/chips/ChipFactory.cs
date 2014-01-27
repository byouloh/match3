using UnityEngine;
using System.Collections;

/**
 * Фабрика для создания фишек.
 * 
 * @author Timur Bogotov timur@e-magic.org
 */
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
        
        return createNew("chip", spriteName, chipType, bonusType, parent);
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
        
        GameObject chipObject = (GameObject)UnityEngine.Object.Instantiate(prefab);
        
        if (parent != null) {
            chipObject.transform.parent   = parent.transform;
            chipObject.transform.position = parent.transform.position;
        }
        
        Sprite sprite = Resources.Load<Sprite>("textures/chipSprites/" + spriteName);
        
        if (sprite == null) {
            UnityEngine.Object.Destroy(chipObject);
            throw new System.NullReferenceException("Ошибка! Не удалось загрузить префаб: " + spriteName);
        }
        
        chipObject.GetComponent<SpriteRenderer>().sprite = sprite;
        
        Chip chip = chipObject.GetComponent<Chip>();
        
        if (chip == null) {
            UnityEngine.Object.Destroy(chipObject);
            throw new System.NullReferenceException("Ошибка! На префабе нет компоненты Spite renderer");
        }
        
        chip.type      = chipType;
        chip.bonusType = bonusType;
        
        switch (bonusType) {
            case BonusType.NONE :
                chip.explodeHelper = null;
                break;
                
            case BonusType.HORIZONTAL_STRIP :
                chip.explodeHelper = new ExplodeLineHelper(bonusType);
                break;
                
            case BonusType.VERTICAL_STRIP :
                chip.explodeHelper = new ExplodeLineHelper(bonusType);
                break;
            
            case BonusType.SAME_TYPE :
                chip.explodeHelper = new ExplodeSameHelper();
                break;
                
            default:
                chip.explodeHelper = null;
                throw new System.ArgumentException("Ошибка! Неверный тип бонуса");
        }
        
        chip.setExplodePoints(Game.getInstance().level.chipsInfo[(int)bonusType].explodePoints);

        return chip;
    }
}
