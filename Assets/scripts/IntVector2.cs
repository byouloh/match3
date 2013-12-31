using UnityEngine;
using System;
using System.Collections;

/**
 * Структура для хранения двух целых чисел. 
 * 
 * @author Islam Zhambeev islam@e-magic.org
 */
public class IntVector2
{
    public int x;
    public int y;

    public static IntVector2 up
    {
        get
        {
            return new IntVector2(0, -1);
        }
    }

    public static IntVector2 down 
    {
        get 
        {
            return new IntVector2 (0, 1);
        }
    }

    public static IntVector2 left 
    {

        get 
        {
            return new IntVector2(-1, 0);
        }
    }

    public static IntVector2 rigth
    {
        get 
        {
            return new IntVector2(1, 0);
        }
    }

    public IntVector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public int this[int index]
    {
        get
        {
            if (index == 0) {
                return this.x;
            }

            if (index != 1) {
                throw new IndexOutOfRangeException("Invalid Vector2 index!");
            }

            return this.y;
        }

        set
        {
            if (index != 0) {
                if (index != 1) {
                    throw new IndexOutOfRangeException("Invalid Vector2 index!");
                }

                this.y = value;
            } else {
                this.x = value;
            }
        }
    }
}
