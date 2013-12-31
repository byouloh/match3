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
    NONE = 0,            // Нет
    HORIZONTAL_STRIP,    // Горизонтальные полоски, уничтожает строку
    VERTICAL_STRIP,        // Вертикальные полоски, уничтожает столбец
    SAME_TYPE            // Уничтожает фишки определенного типа
}

/** Класс фишки. */
public class Chip : MonoBehaviour, IExplodable
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
            gm.transform.localPosition = Vector3.zero;
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
}
