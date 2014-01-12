using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FallingManger
{
    private const float GRAVITY_Y = 9.8f;
    
	private List<GameObject> _items;
    private Grid _grid;
    private bool _isStarting;
    private bool _isFalling;
    
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

    public FallingManger()
    {
        _isStarting = false;
        _isFalling  = false;
        _grid       = null;
        _items      = new List<GameObject>();
    }
    
    public void step(float deltaTime)
    {
        /*
        if (!_isFalling) {
            return;
        }
        
        for (int i = 0; i < _items.Count; i++) {
            if (_items[i]) {
                
            }
        }*/
    }
    
    public bool startFalling(Grid grid, Callback completeCallback)
    {
        _items.Clear();
        
        //for (i = 0; i < _ )
        return false;
    }
    
    public bool checkFallingChips(Grid grid)
    {
        int i, j;
        
        //for (int i = grid.Col) 
        return false;
    }
}