
/** макс. 16 значений. */
public enum CellType
{
    EMPTY = 0,
    NORMAL,
    BUILDER,
    TELEPORTER
}

public abstract class CellBehaviour: IChipTaker, ICellInfluence
{
    protected Cell _cell;
    protected Grid _grid;

    public abstract Chip takeChip(Cell caller);

    /** Определяет возможность фишке покинуть ячейку. */
    public abstract bool canLeave();
    
    /** Определяет возможность фишке войти в ячейку. */
    public abstract bool canEnter();
    
    /** Определяет возможность создать фишку внутри ячейки. */
    public abstract bool canContainChip();
}
