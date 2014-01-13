using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FallingManager
{
    private const float GRAVITY_Y = 9.8f;
    
	private List<GameObject> _items;
    private Grid _grid;
    private bool _isStarting;       // Запуск менеджера.
    private bool _isFalling;        // Проверка на обновление.
    
    private float _fallingSpeed = 1f;
    private float _fallingMaxSpeed = 1f;
    
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

    public FallingManager()
    {
        _isStarting = false;
        _isFalling  = false;
        _grid       = null;
        _items      = new List<GameObject>();
    }

    /**
     * Метод реализует падение и вызывается при взрыве фишек.
     */
    public void step(float deltaTime)
    {
        if (!_isFalling) {
            return;
        }

        bool finish = true;

        for (int i = 0; i < _items.Count; i++) {
            if (_items[i].transform.localPosition != Vector3.zero) {
                //...
                finish = false;
            }
        }

        if (finish) {
            _isFalling = false;
            _checkFallingChips();
        }
    }
    
    public void start(Grid grid, Callback completeCallback)
    {
        if (grid == null) {
            throw new System.NullReferenceException("FallingManager::start: grid is null");
        }

        _grid = grid;
        //_completeCallback = completeCallback;


        _checkFallingChips();
    }
    
    private void _checkFallingChips()
    {
        _items.Clear();
        Cell cell;
        Chip chip;

        for (int i = _grid.getColCount() - 1; i > 0; i--) {
            for (int j = 0; j < 0; j++) {
                cell = _grid.getCell(i, j);

                if (cell.isEmpty() && cell.canEnter()) {
                    chip = cell.takeChip();

                    if (chip != null) {
                        _items.Add(chip.gameObject);
                    }
                }
            }
        }

        if (_items.Count <= 0 && _completeCallback != null) {
            _completeCallback();
        } else {
            _isFalling = true;
        }
    }
}