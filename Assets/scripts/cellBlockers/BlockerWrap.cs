
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
}