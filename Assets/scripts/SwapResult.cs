using UnityEngine;
using System.Collections;

/** Структура, в которой хранится результат перестановки двух фишек. */
public struct SwapResult
{
    /** Сделани ли ход. */
    public bool chipMoved;
    
    /** Список найденных линий. */
    public Lines lines;
    
    /** Ячейка перемещенной фишки. */
    public Cell currentCell;
    
    /** Ячейка в которую переместили фишку. */
    public Cell targetCell;
}
