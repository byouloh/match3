using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** Тип фишки. */
public enum ChipType
{
    RED = 0,
    GREEN,
    BLUE,
    YELLOW,
    ORANGE,
    PURPLE
}

/**
 * Тип бонуса фишки.
 * Чем больше порядковый номер типа бонуса, тем лучше она считается.
 */
public enum BonusType
{
    /** Нет. */
    NONE = 0,
    
    /** Горизонтальные полоски, уничтожает строку. */
    HORIZONTAL_STRIP,
    
    /** Вертикальные полоски, уничтожает столбец. */
    VERTICAL_STRIP,
    
    /** Уничтожает фишки определенного типа. */
    SAME_TYPE
}

/**
 * Класс фишки.
 * 
 * @author Azamat Bogotov azamat@e-magic.org
 * @author Timur Bogotov timur@e-magic.org
 */
public class Chip: MonoBehaviour, IExplodable
{
    /** Префаб анимации взрыва. */
    public GameObject explosionPrefab;
    
    /** Тип фишки. */
    public ChipType type = ChipType.RED;
    
    /** Тип бонуса фишки. */
    public BonusType bonusType = BonusType.NONE;
    
    /** Класс, который возвращает список взрываемых ячеек текущей фишкой. */
    public IExplodeHelper explodeHelper;
    
    /** Обработчик события окончании анимации взрыва. */
    private Callback _explodeCallback;
    
    /** Количество очков, за взрыв фишки. */
    private uint _explodePoints;
    
    /**
     * Взрывает фишку
     * 
     * @param callback обработчик события окончания анимации взрыва фишки
     * 
     * @return bool true, если запущена анимация взрыва, иначе false
     */
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
    
    /**
     * Должна ли взорваться фишка при вызове функции explode().
     */
    public bool canExplode()
    {
        return true;
    }
    
    /**
     * Обработчик события окончании проигрывания анимации взрыва.
     * 
     * @param self взорвавшийся объект
     */
    private void _onExplodeAnimationComplete(Object self)
    {
        if (_explodeCallback != null) {
            _explodeCallback();
        }
    }
    
    /**
     * Возвращает список взрываемых ячеек после взрыва текущей ячейки.
     * 
     * @param currentCell текущая взрываемая ячейка
     * @param targetCellInfo информация о второй передвигаемой фишке
     * 
     * @return Match список взрываемых ячеек
     */
    public Match affectCells(Cell currentCell, BonusInfo targetCellInfo)
    {
        if (explodeHelper != null) {
            return explodeHelper.affectCells(currentCell, targetCellInfo);
        }
        
        return null;
    }
    
    /**
     * Возвращает количество очков за взрыв объекта.
     * 
     * @return uint количество очков за взрыв объекта
     */
    public uint getExplodePoints()
    {
        return _explodePoints;
    }
    
    /**
     * Задает количество очков за взрыв объекта.
     * 
     * @param explodePoints количество очков за взрыв объекта
     */
    public void setExplodePoints(uint explodePoints)
    {
        _explodePoints = explodePoints;
    }
}
