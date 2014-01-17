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
public class Chip: MonoBehaviour, IExplodable, IFallable
{
    /** Префаб анимации взрыва. */
    public GameObject explosionPrefab;
    
    /** Тип фишки. */
    public ChipType type = ChipType.RED;
    
    /** Тип бонуса фишки. */
    public BonusType bonusType = BonusType.NONE;
    
    /** Класс, который возвращает список взрываемых ячеек текущей фишкой. */
    public IExplodeHelper explodeHelper;

    /** Контроллер анмации падения вниз вверх.*/
    public RuntimeAnimatorController fallDownController = null;

    /** Контроллер анмации падения вниз вверх.*/
    public RuntimeAnimatorController fallRigthController = null;

    /** Контроллер анмации падения вниз вверх.*/
    public RuntimeAnimatorController fallLeftController = null;

    /** Контроллер анимации подсказски.*/
    public RuntimeAnimatorController helpAnimationController = null;

    /** Компонент анимации фишки*/
    private Animator _animator; 
    
    /** Обработчик события окончании анимации взрыва. */
    private Callback _explodeCallback;
    
    /** Количество очков, за взрыв фишки. */
    private uint _explodePoints;


    /**
     * Инициализация параметров фишки. 
     */
    private void Start()
    {
        _animator = gameObject.GetComponent<Animator>() as Animator;
        _animator.runtimeAnimatorController = helpAnimationController;
    }

    /**
     * Взрывает фишку /**
     * Запускаем анимацию падения.
     *
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
            gm.transform.localPosition = new Vector3(0f, 0f, Game.TOP_Z_INDEX);
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

    /**
     * Запускаем анимацию падения.
     * 
     * @param fallDirection направление падения.
     */
    public void onFallingStart(FallDirection fallDirection)
    {
        RuntimeAnimatorController controller = null;

        gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, 
                                                         gameObject.transform.localPosition.y, 
                                                         Game.TOP_Z_INDEX);

        switch (fallDirection) 
        {
            case FallDirection.DOWN:
                controller = fallDownController;
                break;

            case FallDirection.LEFT_DOWN:
                controller = fallLeftController;
                break;

            case FallDirection.RIGHT_DOWN:
                controller = fallRigthController;
                break;

            default:
                break;
        }

        if (controller != null) {
            _animator.runtimeAnimatorController = controller;
            _animator.speed = 1.0f;
            _animator.SetTrigger("animation");
        }
    }

    /**
     * Останавливаем анимацию падения.
     */
    public void onFallingStop()
    {
        _animator.speed = 1.0f;
        gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, 
                                                         gameObject.transform.localPosition.y, 
                                                         0f);

        if (helpAnimationController != null) {
            _animator.runtimeAnimatorController = helpAnimationController;
        }
    }

    /**
     * Функия перерасчитывает скорость падения фищек.
     * 
     * @param fallSpeed скорость падения
     */
    public void onFalling(float fallSpeed)
    {
        _animator.speed = fallSpeed * 80;
    }

    /**
     * Геттер для transorm.
     */
    public Transform getTransform()
    {
        return gameObject.transform;
    }

    /**
     * Запускаем анимацию подсказски.
     */
    public void startHelpAnimation()
    {
        _animator.speed = 1.0f;

        if (helpAnimationController != null) {
            _animator.runtimeAnimatorController = helpAnimationController;
            _animator.SetTrigger("animation");
        }
    }
}

