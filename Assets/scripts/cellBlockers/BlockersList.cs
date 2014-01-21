using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * Группа блокирующих элементов.
 * 
 * Используется для хранения и обработки группы блокирующих элементов применительно к одной ячейке.
 * Можно работать с группой как с одним блокирующим элементом.
 * 
 * @author Azamat Bogotov azamat@e-magic.org
 */
class BlockersList: ICellInfluence
{
    /** Список блокирующих элементов. */
    protected List<CellBlocker> _blockers;

    /** Контейнер для хранения создаваемых блокирующих элементов. */
    private GameObject _rootContainer;

    /** Счетчик, подсчитывающий количество блокирующих элементов, повлиявших на свойство canLeave. */
    private int _canLeave;

    /** Счетчик, подсчитывающий количество блокирующих элементов, повлиявших на свойство canEnter. */
    private int _canEnter;

    /** Счетчик, подсчитывающий количество блокирующих элементов, повлиявших на свойство canContainChip. */
    private int _canContainChip;

    /** Счетчик, подсчитывающий количество блокирующих элементов, повлиявших на свойство isProtecting. */
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

        // Добавляем первый блокирующий элемент по умолчанию.
        _addInitialBlocker();
    }

    /**
     * Добавляет блокирующий элемент по умолчанию и задает начальные значения для счетчиков.
     * 
     * Добавляет первый и не удаляемый блокирующий элемент по умолчанию, 
     * т.к. в списке должен оставаться хотя бы один элемент для обработки.
     */
    private void _addInitialBlocker()
    {
        if (_blockers.Count <= 0) {
            CellBlocker blocker = BlockerFactory.createNew(BlockerType.NONE, _rootContainer);
        
            // Первый блокирующий элемент задает начальные значения для счетчиков.
            _canLeave       = (blocker.canLeave()       ? 1 : 0);
            _canEnter       = (blocker.canEnter()       ? 1 : 0);
            _canContainChip = (blocker.canContainChip() ? 1 : 0);
            _isProtecting   = (blocker.isProtecting()   ? 1 : 0);

            _blockers.Add(blocker);
            
            blocker.transform.parent        = _rootContainer.transform;
            blocker.transform.localPosition = Vector3.zero;
        }
    }

    /**
     * Определяет возможность фишке покинуть ячейку, исходя из общего влияния группы блокирующих элементов.
     */
    public bool canLeave()
    {
        return (_canLeave > 0);
    }
    
    /**
     * Определяет возможность фишке войти ячейку, исходя из общего влияния группы блокирующих элементов.
     */
    public bool canEnter()
    {
        return (_canEnter > 0);
    }
    
    /**
     * Определяет возможность создать фишку внутри ячейки, исходя из общего влияния группы блокирующих элементов.
     */
    public bool canContainChip()
    {
        return (_canContainChip > 0);
    }
    
    /**
     * Определяет защищает ли группа блокирующих элементов содержимое ячейки от взрыва.
     */
    public bool isProtecting()
    {
        return (_isProtecting > 0);
    }

    /**
     * Добавляет новый блокирующий элемент в список(стек).
     * 
     * @param blocker Блокирующий элемент отличный от BlockerType.NONE.
     */
    public void push(CellBlocker blocker)
    {
        if (blocker == null) {
            throw new System.NullReferenceException("CellBlockers::push: blocker is null");
        }

        // Запрещаем добавление блокирующего элемента BlockerNone, т.к. он уже содержится в списке.
        // Если его добавить, то, т.к. у него нет взрыва, все элементы, добавленные до него остануться навсегда.
        // см. Cell.explode()
        if (blocker is BlockerNone) {
            return;
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

    /**
     * Возвращает текущий(активный) блокирующий элемент.
     * 
     * Если список пуст, то создается блокирующий элемент по умолчанию и возвращается он.
     * 
     * @return Возвращает текущий(активный) блокирующий элемент.
     */
    public CellBlocker getCurrent()
    {
        if (_blockers.Count <= 0) {
            _addInitialBlocker();
        }

        return _blockers[0];
    }

    /**
     * Удаляет текущий(активный) блокирующий элемент из списка.
     * 
     * Элемент удаляется как списка и затем сам уничтожается.
     * При удалении объекта, изменяются счетчики свойств, на которые он влиял.
     */
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
