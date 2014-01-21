using UnityEngine;
using System.Collections;

/** Класс-структура, в которой хранится результат перестановки двух фишек. */
public class SwapResult
{
    /** Сделани ли ход. */
    public bool chipMoved = false;
    
    /** Список найденных линий. */
    public Lines lines = null;
    
    /** Ячейка перемещенной фишки. */
    public Cell currentCell = null;
    
    /** Ячейка в которую переместили фишку. */
    public Cell targetCell = null;
}
