using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FallingManager
{
    public float fallSpeed {
        get
        {
            return _fallingSpeed;
        }
        set
        {
            if (value > 0) {
                _fallingSpeed = value;
            }
        }
    }

	private List<GameObject> _items;

    /**
     * Обьекты которые мы будем удалять.
     */
    private List<GameObject> _removeItems;

  

    /**
     * Сетка ячеек которые мы обрабатываем.
     */
    private Grid _grid;

    /**
     * Запущен ли процесс падения фишек и заполнения пустых ячеек.
     */
    private bool _isStarting;

    /**
     * Упалили все фишки которые должны были упасть.
     */
    private bool _isFalling;

    /**
     * Скорость падения фишек.
     */
    private float _fallingSpeed    = 1.2f;

    /**
     * Максимальная скорость падения фишек.
     */
    private float _fallingMaxSpeed = 1f;

    /**
     * Callback  функция вызываемая после падения фишек.
     */
    private Callback _completeCallback;

    /**
     * Конструктор класса.
     */
    public FallingManager()
    {
        _isStarting  = false;
        _isFalling   = false;
        _grid        = null;
        _items       = new List<GameObject>();
        _removeItems = new List<GameObject>();

        _completeCallback = null;
    }

    /**
     * Метод реализует падение и вызывается при взрыве фишек.
     */
    public void step(float deltaTime)
    {
        if (!_isFalling || !_isStarting) {
            return;
        }

        bool finish = true;

        float speed    = _fallingSpeed * deltaTime;
        float sqrSpeed = speed * speed;

        for (int i = 0; i < _items.Count; i++) {
            if (_items[i].transform.localPosition.sqrMagnitude < sqrSpeed) {
                _items[i].transform.localPosition = Vector3.zero;
            } else {
                _items[i].transform.localPosition -= _items[i].transform.localPosition.normalized * speed;

                if (finish) {
                    finish = false;
                }
            }
        }

        if (finish) {
            for (int i = 0; i < _removeItems.Count; i++) {
                GameObject.Destroy(_removeItems[i]);
            }

            _isFalling = false;
            _checkFallingChips();
        }
    }

    /**
     *  Устанавливает сетку и callback функцию. 
     */
    public void start(Grid grid, Callback completeCallback)
    {
        if (_isStarting) {
            throw new System.NullReferenceException("FallingManager::start: falling already started");
        }

        if (grid == null) {
            throw new System.NullReferenceException("FallingManager::start: grid is null");
        }

        _isStarting = true;

        _grid = grid;
        _completeCallback = completeCallback;

        _checkFallingChips();
    }

    /**
     * Запущен ли процесс падения фишек и заполнения пустых ячеек.
     */
    public bool isStarting()
    {
        return _isStarting;
    }

    private void _checkFallingChips()
    {
        _items.Clear();
        _removeItems.Clear();

        Cell cell;

        int i, j;

        for (i = _grid.getRowCount() - 1; i > 0; i--) {
            for (j = 0; j < _grid.getColCount(); j++) {
                cell = _grid.getCell(i, j);

                if (cell != null && cell.isEmpty() && cell.canEnter()) {
                    cell.takeChip(0);
                }
            }
        }

        for (i = 0; i < _grid.getRowCount(); i++) {
            for (j = 0; j < _grid.getColCount(); j++) {
                cell = _grid.getCell(i, j);

                if (cell != null && !cell.isEmpty() && cell.chip.transform.localPosition != Vector3.zero) {
                    _items.Add(cell.chip.gameObject);

                    if (!cell.canEnter()) {
                        _removeItems.Add(cell.chip.gameObject);
                    }
                }
            }
        }

        Debug.LogError("ITEMS:" + _items.Count);
        Debug.LogError("REMOVE_ITEMS:" + _removeItems.Count);

        if (_items.Count <= 0) {
            _isStarting = false;

            if (_completeCallback != null) {
                _completeCallback();
            }
        } else {
            _isFalling = true;
        }
    }
}