
public abstract class CellBehaviour: IChipTaker, ICellInfluence
{
    protected IntVector2 _cellPosition;
    protected Grid _grid;

    public abstract Chip takeChip(int depth);

    /** Определяет возможность фишке покинуть ячейку. */
    public abstract bool canLeave();
    
    /** Определяет возможность фишке войти в ячейку. */
    public abstract bool canEnter();
    
    /** Определяет возможность создать фишку внутри ячейки. */
    public abstract bool canContainChip();
}