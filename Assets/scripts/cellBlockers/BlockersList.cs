using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

class BlockersList: ICellInfluence
{
    protected List<CellBlocker> _blockers;
    private GameObject _rootContainer;

    private int _canLeave;
    private int _canEnter;
    private int _canContainChip;
    private int _isProtecting;

    /**
     * Конструктор класса.
     * 
     * Необходимо задавать контейнер, где будут находиться блокирующие элементы.
     * 
     * @param Контейнер-родитель, куда будут помещаться блокирующие элементы
     */
    public BlockersList(GameObject root)
    {
        if (root == null) {
            throw new System.NullReferenceException("CellBlockers::CellBlockers: param root is null");
        }

        _rootContainer = root;
        _blockers      = new List<CellBlocker>();

        _addInitialBlocker();
    }

    private void _addInitialBlocker()
    {
        if (_blockers.Count <= 0) {
            CellBlocker blocker = BlockerFactory.createNew(BlockerType.NONE, _rootContainer);
        
            // Первый блокирующий элемент задает начальные значения для счетчиков.
            _canLeave       = (blocker.canLeave()       ? 1 : 0);
            _canEnter       = (blocker.canEnter()       ? 1 : 0);
            _canContainChip = (blocker.canContainChip() ? 1 : 0);
            _isProtecting   = (blocker.isProtecting()   ? 1 : 0);
            
            push(blocker);
        }
    }

    /**
     * Определяет возможность фишке покинуть ячейку.
     */
    public bool canLeave()
    {
        return (_canLeave > 0);
    }
    
    /**
     * Определяет возможность фишке войти ячейку.
     */
    public bool canEnter()
    {
        return (_canEnter > 0);
    }
    
    /**
     * Определяет возможность создать фишку внутри ячейки.
     */
    public bool canContainChip()
    {
        return (_canContainChip > 0);
    }
    
    /**
     * Защищает ли блокирующий элемент содержимое от взрыва.
     */
    public bool isProtecting()
    {
        return (_isProtecting > 0);
    }

    public void push(CellBlocker blocker)
    {
        if (blocker == null) {
            throw new System.NullReferenceException("CellBlockers::push: blocker is null");
        }

        _blockers.Insert(0, blocker);

        blocker.transform.parent        = _rootContainer.transform;
        blocker.transform.localPosition = Vector3.zero;

        // с каждым добавлением блокирующего элемента изменяем счетчики значений, 
        // чтобы не перебирать каждый раз в цикле в соответствующих методах.
        _canLeave       += (blocker.canLeave()       ? 0 : -1);
        _canEnter       += (blocker.canEnter()       ? 0 : -1);
        _canContainChip += (blocker.canContainChip() ? 0 : -1);
        _isProtecting   += (blocker.isProtecting()   ? 1 : 0);
    }

    public CellBlocker getCurrent()
    {
        if (_blockers.Count <= 0) {
            _addInitialBlocker();
        }

        return _blockers[0];
    }

    public void removeCurrent()
    {
        if (_blockers.Count <= 1) {
            return;
        }
        
        CellBlocker blocker = _blockers[0];
        _blockers.RemoveAt(0);

        _canLeave       += (blocker.canLeave()       ? 0 : 1);
        _canEnter       += (blocker.canEnter()       ? 0 : 1);
        _canContainChip += (blocker.canContainChip() ? 0 : 1);
        _isProtecting   += (blocker.isProtecting()   ? -1 : 0);

        GameObject.Destroy(blocker.gameObject);
        blocker = null;
    }
}
