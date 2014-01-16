using UnityEngine;
using System.Collections;

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

/** Тип бонуса фишки. */
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

/** Класс фишки. */
public class Chip: MonoBehaviour, IExplodable
{
    /** Префаб анимации взрыва. */
    public GameObject explosionPrefab;
    
    /** Тип фишки. */
    public ChipType type = ChipType.RED;
    
    /** Тип бонуса фишки. */
    public BonusType bonusType = BonusType.NONE;
    
    /** Обработчик события окончании анимации взрыва. */
    private Callback _explodeCallback;

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
            gm.transform.localPosition = new Vector3(0F, 0F, Game.TOP_Z_INDEX);
        }

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
     * Возвращает копию объекта фишки.
     */
    public Chip clone()
    {
        GameObject go = GameObject.Instantiate(gameObject) as GameObject;
        Chip chip     = go.GetComponent<Chip>();

        chip.explosionPrefab = this.explosionPrefab;
        chip.type            = this.type;
        chip.bonusType       = this.bonusType;

        return chip;
    }

    /**
     * Проверка фишки на соответствие с другой фишкой.
     * 
     * @param chip Фишка, с которой нужно сравнить.
     */
    public bool compareTo(Chip chip)
    {
        if (chip == null || 
            this.bonusType == BonusType.SAME_TYPE || 
            chip.bonusType == BonusType.SAME_TYPE || 
            this.type != chip.type
        ) {
            return false;
        }

        return true;
    }
}
