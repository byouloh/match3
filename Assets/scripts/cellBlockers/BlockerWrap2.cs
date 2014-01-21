
/**
 * Блокирующий элемент(двойная обводка).
 * 
 * @author Timur Bogotov timur@e-magic.org
 * @author Azamat Bogotov azamat@e-magic.org
 */
public class BlockerWrap2: CellBlocker
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
    
    /** Определяет возможность создать фишку внутри ячейки. */
    public override bool canContainChip()
    {
        return true;
    }

    /** Защищает ли блокирующий элемент содержимое от взрыва. */
    public override bool isProtecting()
    {
        return false;
    }

    /** Есть ли у блокирующего элемента следующий за ним блокирующий элемент. */
    public override bool hasNext()
    {
        return true;
    }

    /** Возвращает следующий блокирующий элемент, который нужно создать после текущего. */
    public override BlockerType getNext()
    {
        return BlockerType.WRAP;
    }
}
