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

    private const float GRAVITY_Y = 9.8f;
    
	private List<GameObject> _items;
    private List<GameObject> _removeItems;
    private Grid _grid;

    /**
     * 
     */
    private bool _isStarting;

    /**
     * 
     */
    private bool _isFalling;
    
    private float _fallingSpeed    = 1f;
    private float _fallingMaxSpeed = 1f;
    
    private Callback _completeCallback;

    public FallingManager()
    {
        _isStarting = false;
        _isFalling  = false;
        _grid       = null;
        _items      = new List<GameObject>();

        _completeCallback = null;
    }

    /**
     * Метод реализует падение и вызывается при взрыве фишек.
     */
    public void step(float deltaTime)
    {
        if (!_isFalling || !isStarting) {
            return;
        }

        bool finish = true;

        float speed = _fallingSpeed * deltaTime;

        for (int i = 0; i < _items.Count; i++) {
            if (_items[i].transform.localPosition != Vector3.zero) {
                _items[i].transform.localPosition = Vector3.Lerp(_items[i].transform.localPosition,
                                                                 Vector3.zero, speed);

                if (_items[i].transform.localPosition == Vector3.zero) {
                    if (_removeItems.Contains(_items[i])) {
                        GameObject.Destroy(_items[i]);
                    }
                } else {
                    finish = false;
                }
            }
        }

        if (finish) {
            _isFalling = false;
            _checkFallingChips();
        }
    }
    
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
        Chip chip;

        int i, j;

        for (i = _grid.getRowCount() - 1; i > 0; i--) {
            for (j = 0; j < _grid.getColCount(); j++) {
                cell = _grid.getCell(i, j);

                if (cell.isEmpty() && cell.canEnter()) {
                    chip = cell.takeChip();
                }
            }
        }

        for (i = 1; i < _grid.getRowCount(); i++) {
            for (j = 0; j < _grid.getColCount(); j++) {
                cell = _grid.getCell(i, j);

                if (!cell(i, j).isEmpty() && cell.chip.transform.localPosition != Vector3.zero) {
                    _items.Add(cell.chip.gameObject);

                    if (!cell.canEnter()) {
                        _removeItems.Add(cell.chip.gameObject);
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