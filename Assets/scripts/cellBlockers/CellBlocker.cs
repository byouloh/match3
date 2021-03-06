﻿using UnityEngine;
using System.Collections;

/**
 * Тип блокирующего элемента ячейки.
 */
public enum BlockerType: int
{
    /** Обычная ячейка. */
    NONE = 0,
    
    /** Цепь. */
    CHAIN,
    
    /** Двойная цепь. */
    CHAIN2,
    
    /** Обертка. */
    WRAP,
    
    /** Двойная обертка. */
    WRAP2
};

/**
 * Абстрактный(базовый) класс для блокирующих элементов.
 * 
 * @author Timur Bogotov timur@e-magic.org
 * @author Azamat Bogotov azamat@e-magic.org
 */
public abstract class CellBlocker: MonoBehaviour, IExplodable, ICellInfluence
{
    /** Обработчик события по окончании взрыва. */
    private Callback _explodeCallback;
    
    /** Количество очков, за взрыв ячейки. */
    private uint _explodePoints;
    
	/** Префаб анимации взрыва. */
    public GameObject explosionPrefab = null;
    
    /** Фишка может покинуть ячейку. */
    public abstract bool canLeave();
    
    /** Фишка может войти в ячейку. */
    public abstract bool canEnter();

    /** Определяет возможность создать фишку внутри ячейки. */
    public abstract bool canContainChip();
    
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
        if (explosionPrefab == null) {
            return false;
        }

        _explodeCallback = callback;
        GameObject gm    = (GameObject)Instantiate(explosionPrefab);

        AnimationEventCallback cb = gm.GetComponent<AnimationEventCallback>();

        if (cb != null) {
            cb.initialize(_onExplodeAnimationComplete);
        } else {
            Debug.LogError("CellBlocker::explode: Не найдент компонент: AnimationEventCallback");
        }

        if (gameObject.transform.parent != null) {
            gm.transform.parent        = gameObject.transform.parent;
            gm.transform.localPosition = new Vector3(0f, 0f, Game.TOP_Z_INDEX);
        }

        return true;
    }
    
    /**
     * Должен ли взорваться блокирующий элемент при вызове функции explode().
     */
    public bool canExplode()
    {
        return explosionPrefab != null;
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
     * Есть ли у блокирующего элемента следующий за ним блокирующий элемент.
     * 
     * @return bool есть ли у блокирующего элемента следующий за ним блокирующий элемент.
     */
    public virtual bool hasNext()
    {
        return false;
    }
    
    /**
     * Возвращает следующий блокирующий элемент, который нужно создать после текущего.
     * 
     * @return BlockerType следующий блокирующий элемент
     */
    public virtual BlockerType getNext()
    {
        return BlockerType.NONE;
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
