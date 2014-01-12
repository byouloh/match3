
/**
 * Блокирующий элемент(одинарная цепь).
 */
public class BlockerChain: CellBlocker
{
    /** Может ли фишка покинуть ячейку. */
    public override bool canLeave()
    {
        return false;
    }
    
    /** Может ли фишка войти в ячейку. */
    public override bool canEnter()
    {
        return false;
    }
    
    /** Определяет возможность создать фишку внутри ячейки. */
    public override bool canContainChip()
    {
        return true;
    }

    /** Защищает ли блокирующий элемент содержимое от взрыва. */
    public override bool isProtecting()
    {
        return true;
    }
}
