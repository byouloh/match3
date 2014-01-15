using UnityEngine;
using System.Collections;

/** Класс ячейки. */
public class Cell : MonoBehaviour, IExplodable
{
    /** Фишка. */
	public Chip chip = null;
    
    /** Блокирующий элемент. */
	public CellBlocker blocker = null;
    
    /** Обработчик события по окончании взрыва. */
    private Callback _explodeCallback = null;
    
    /** Матричные координаты(номер строки и столбца) */
    public IntVector2 position;

	/**
     * Инициализирует ячейку.
     * 
     * @param blocker блокирующий элемент
     * @param chip фишка ячейки(может быть null)
     * @param position матричные координаты(номер строки и столбца)
     */
	public void initialize(CellBlocker blocker, Chip chip, IntVector2 position)
	{
		this.blocker        = blocker;
		this.chip           = chip;
        this.position       = position;
	}
    
    /** Может ли фишка покинуть ячейку. */
	public bool canLeave()
	{
		return blocker.canLeave();
	}
	
    /** Может ли фишка войти в ячейку. */
	public bool canEnter()
	{
        return blocker.canEnter();
    }
	
    /** Может ли фишка пройти сквозь ячейку. */
	public bool canPass()
    {
		return blocker.canPass();
    }
    
    /**
     * Взрывает блокирующий элемент, если есть, при повторном вызове взрывает фишку, если есть.
     * 
     * @param callback обработчик событий по окончании взрыва
     * 
     * @return true, если началась анимация взрыва, иначе false
     */
	public bool explode(Callback callback)
	{
		_explodeCallback = callback;

		Callback tmpCallback = _onExplodeComplete;

		// Защищена ли фишка
		bool chipProtected   = false;

		// 
		bool blockerExploded = false;

        if (blocker.explode(tmpCallback)) {
			blockerExploded = true;

			if (blocker.isProtecting()) {
				chipProtected = true;
			}

			if (blocker.hasNext()) {
				BlockerType nextBlockerType = blocker.getNext();
				
				Destroy(blocker.gameObject);
				
				try {
					blocker = BlockFactory.createNew(nextBlockerType, gameObject);
                } catch (System.Exception e) {
                    Debug.LogError(e.Message);
				}
			}
		}

		if (!chipProtected) {
			if (chip == null || !chip.explode(tmpCallback)) {
				return blockerExploded;
			}

			Destroy(chip.gameObject);

			chip = null;
        }

		return true;
	}
    
    /**
     * Обработчик события окончании проигрывания анимации взрыва.
     */
    private void _onExplodeComplete()
	{
		if (_explodeCallback != null) {
			_explodeCallback();
		}
	}
    
    /**
     * Возвращает количество очков за взрыв объекта.
     * 
     * @return uint количество очков за взрыв объекта
     */
    public uint getExplodePoints()
    {
        uint res = 0;
        
        // Защищена ли фишка
        bool chipProtected = false;
        
        if (blocker.canExplode()) {
            if (blocker.isProtecting()) {
                chipProtected = true;
            }
            
            res += blocker.getExplodePoints();
        }
        
        if (!chipProtected && chip != null && chip.canExplode()) {
            res += chip.getExplodePoints();
        }
        
        return res;
    }
    
    /**
     * Задает количество очков за взрыв объекта.
     * 
     * @param explodePoints количество очков за взрыв объекта
     */
    public void setExplodePoints(uint explodePoints)
    {
        // ничего не делать
    }
    
    /** Возвращает список ячеек, которые взрываются после взрыва текущей ячейки. */
    public Match affectCells(BonusInfo targetCell)
    {
        if (!blocker.isProtecting() && chip != null) {
            return chip.affectCells(this, targetCell);
        }
        
        return null;
    }
    
}
