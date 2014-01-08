using UnityEngine;
using System.Collections;

/**
 * Пустой блоирующий элемент.
 */
public class BlockerNone: CellBlocker
{
    /** Может ли фишка покинуть ячейку. */
	public override bool canLeave()
	{
		return true;
	}
    
    /** Может ли фишка войти в ячейку. */
	public override bool canEnter()
	{
		return true;
	}
    
    /** Может ли фишка пройти сквозь ячейку. */
	public override bool canPass()
	{
		return true;
	}
    
    /** Защищает ли блокирующий элемент содержимое от взрыва. */
	public override bool isProtecting()
	{
		return false;
	}

}
