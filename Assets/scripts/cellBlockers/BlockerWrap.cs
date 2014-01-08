using UnityEngine;
using System.Collections;

/**
 * Блокирующий элемент(одинарная обводка).
 */
public class BlockerWrap: CellBlocker
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
		return false;
	}
    
    /** Есть ли у блокирующего элемента следующий за ним блокирующий элемент. */
	public override bool hasNext()
	{
		return true;
	}
    
    /** Защищает ли блокирующий элемент содержимое от взрыва. */
	public override bool isProtecting()
	{
		return false;
	}
    
    /** Возвращает следующий блокирующий элемент, который нужно создать после текущего. */
	public override BlockerType getNext()
	{
		return BlockerType.NONE;
	}
}
