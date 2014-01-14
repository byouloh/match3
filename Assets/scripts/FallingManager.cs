using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FallingManager
{
    /**
     * Обрабатываемые объекты, которые должны упасть.
     */
    private List<GameObject> _items;

    /**
     * Обьекты, которые должны удалиться после падения.
     * 
     * Те объекты, которые после падения окажутся в ячейках, которые не могут содержать фишку в принципе.
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
    private float _fallingSpeed = 1.0f;

    /**
     * Callback  функция вызываемая после падения фишек.
     */
    private Callback _fallingCompleteCallback;

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

        _fallingCompleteCallback = null;
    }

    /**
     * Метод реализует падение и вызывается при взрыве фишек.
     * 
     * @param deltaTime Разница по времени между текущим и последним кадром
     */
    public void step(float deltaTime)
    {
        if (!_isFalling || !_isStarting) {
            return;
        }

        bool isFallingComplete = true;

        float verticalSpeed  = _fallingSpeed * deltaTime;
        float diagonalSpeed  = verticalSpeed * (float)Math.Sqrt(2);
        float sqrSpeed       = verticalSpeed * verticalSpeed;

        for (int i = 0; i < _items.Count; i++) {
            if (_items[i].transform.localPosition.sqrMagnitude < sqrSpeed) {
                _items[i].transform.localPosition = Vector3.zero;
            } else {
                float speed = ((_items[i].transform.localPosition.x != 0) ? diagonalSpeed : verticalSpeed);

                _items[i].transform.localPosition -= _items[i].transform.localPosition.normalized * speed;

                if (isFallingComplete) {
                    isFallingComplete = false;
                }
            }
        }

        if (isFallingComplete) {
            // После падения всех фишек, удаляются те, которые находятся на ячейках, 
            // которые не могут содержать фишки.
            for (int i = 0; i < _removeItems.Count; i++) {
                GameObject.Destroy(_removeItems[i]);
            }

            _isFalling = false;

            // переход к следующей итерации
            _checkFallingChips();
        }
    }

    /**
     * Устанавливает сетку и callback функцию.
     * 
     * @param grid             Обрабатываемая матрица ячеек.
     * @param completeCallback Метод-делегат, который будет вызван после завершения падения.
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
        _fallingCompleteCallback = completeCallback;

        // Запуск первой итерации
        _checkFallingChips();
    }

    /**
     * Запущен ли процесс падения фишек и заполнения пустых ячеек.
     * 
     * @return bool Возвращает true, если идет процесс падения фишек, иначе false
     */
    public bool isStarting()
    {
        return _isStarting;
    }

    /**
     * Определяет фишки, которые должны упасть и запускает процесс падения.
     */
    private void _checkFallingChips()
    {
        _items.Clear();
        _removeItems.Clear();

        Cell cell;

        int i, j;

        // Для пустых ячеек, которые нужно заполнить, вызывается метод, 
        // который берет фишку от соседних верхних, если имеется возможность.
        for (i = _grid.getRowCount() - 1; i > 0; i--) {
            for (j = 0; j < _grid.getColCount(); j++) {
                cell = _grid.getCell(i, j);

                if (cell != null && cell.isEmpty() && cell.canEnter()) {
                    cell.takeChip(cell);
                }
            }
        }

        // Все фишки, которые находится не на своих местах заносятся в массив для обработки падения.
        for (i = 0; i < _grid.getRowCount(); i++) {
            for (j = 0; j < _grid.getColCount(); j++) {
                cell = _grid.getCell(i, j);

                if (cell != null && !cell.isEmpty() && cell.chip.transform.localPosition != Vector3.zero) {
                    _items.Add(cell.chip.gameObject);

                    // После падения, фишка дожна удалиться, т.к. упадет в клетку, которая не может содержать фишку.
                    if (!cell.canEnter()) {
                        _removeItems.Add(cell.chip.gameObject);
                    }
                }
            }
        }

        if (_items.Count <= 0) {
            _isStarting = false;

            if (_fallingCompleteCallback != null) {
                _fallingCompleteCallback();
            }
        } else {
            // Запуск процесса падения фишек
            _isFalling = true;
        }
    }
}
