using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Класс ячейки.
 * 
 * Ячейка - элемент игрового поля. Она может содержать в себе группу блокирующих элементов и фишку.
 * 
 * Поведение ячейки применительно к фишке может быть разным:
 * - Пустая ячейка(не может содержать фишку);
 * - Обычная(может содержать фишку, которая может входить и покаидать ячейку);
 * - Телепорт(не может содержать фишку, но может пропускать вниз через себя);
 * - Создатель(не может содержать фишку, но может ее создавать);
 * 
 * Поведение определяется свойствами самой ячейки и группой блокирующих элементов.
 */
public class Cell: MonoBehaviour, IExplodable, ICellInfluence
{
    /** Фишка. */
	public Chip chip = null;
    
    /** Действующий блокирующий элемент . */
    public CellBlocker activeBlocker
    {
        get
        {
            return _blockers.getCurrent();
        }
    }

    /** Матричные координаты(номер строки и столбца) */
    public IntVector2 position;

    /** Определяет поведение ячейки применительно к фишке. */
    private CellBehaviour _cellBehaviour;

    /** Список блокирующих элементов. */
    private BlockersList _blockers;

    /** Обработчик события по окончании взрыва. */
    private Callback _explodeCallback = null;

    /**
     * Инициализирует ячейку.
     * 
     * @param cellBehaviour Поведение ячейки при обработке и перемещении фишки
     * @param position      Матричные координаты(номер строки и столбца)
     */
    public void initialize(CellBehaviour cellBehaviour, IntVector2 position)
    {
        // создаем контейнер для блокирующих элементов.
        GameObject blockersRoot = new GameObject("blockersRoot");
        blockersRoot.transform.parent        = gameObject.transform;
        blockersRoot.transform.localPosition = Vector3.zero;

        _cellBehaviour = cellBehaviour;
        this.position  = position;
        _blockers      = new BlockersList(blockersRoot);
    }

    /** Может ли фишка покинуть ячейку. */
	public bool canLeave()
	{
		return _blockers.canLeave() && _cellBehaviour.canLeave();
	}
	
    /** Может ли фишка войти в ячейку. */
	public bool canEnter()
	{
        return _blockers.canEnter() && _cellBehaviour.canEnter();
    }
	
    /** Является ли ячейка пустой. */
    public bool isEmpty()
    {
        if (chip == null) {
            return true;
        } else {
            return false;
        }
    }


    /** Определяет возможность создать фишку внутри ячейки. */
    public bool canContainChip()
    {
        return _blockers.canContainChip() && _cellBehaviour.canContainChip();
    }

    public void addBlocker(CellBlocker blocker)
    {
        if (blocker == null) {
            throw new System.NullReferenceException("Blocker can not be null");
        }

        _blockers.push(blocker);
    }

    public Chip takeChip()
    {
        return _cellBehaviour.takeChip();
    }
    
    public void setChip(Chip chip)
    {
        if (chip == null) {
            throw new System.NullReferenceException("Cell::setChip: chip is null");
        }
        
        this.chip = chip;
        chip.transform.parent = gameObject.transform;
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
		_explodeCallback     = callback;
        Callback tmpCallback = _onExplodeComplete;

		// Защищена ли фишка
		bool chipProtected   = false;
        bool blockerExploded = false;
        
        if (_blockers.getCurrent().explode(tmpCallback)) {
            blockerExploded = true;
            
			if (_blockers.isProtecting()) {
				chipProtected = true;
			}

            CellBlocker blocker = null;

            // Если за взорвавшимся нужно создать другой элемент, то создаем.
            if (_blockers.getCurrent().hasNext()) {
                try {
                    blocker = BlockerFactory.createNew(_blockers.getCurrent().getNext(), gameObject);
                } catch (System.Exception e) {
                    Debug.LogError("Cell::explode: " + e.Message);
                }
			}

            // Удаляем взорвавшийся блокирующий элемент и добавляем новый, если есть.
            try {
                _blockers.removeCurrent();

                if (blocker != null) {
                    _blockers.push(blocker);
                }
            } catch (System.Exception e) {
                Debug.LogError("Cell::explode: " + e.Message);
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
}
