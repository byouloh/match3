
public abstract class CellBehaviour: IChipTaker, ICellInfluence
{
    protected IntVector2 _cellPosition;
    protected Grid _grid;
    protected uint _chipTypes;

    public CellBehaviour()
    {
        _cellPosition = IntVector2.zero;
        _grid         = null;
        _chipTypes    = 0;
    }

    public CellBehaviour(IntVector2 cellPosition, Grid grid, uint chipTypes)
    {
        _cellPosition = cellPosition;
        _grid         = grid;
        _chipTypes    = chipTypes;
    }
    
    public virtual Chip takeChip()
    {
        return null;
    }
    
    protected Chip _moveTakedAndGetSelfChip(Chip chip)
    {
        if (chip == null) {
            return null;
        }
        
        // Забираем фишку у ячейки сверху.
        Cell ownerCell        = _grid.getCell(_cellPosition.x, _cellPosition.y);
        Chip selfChip         = ownerCell.chip;
        ownerCell.chip        = chip;
        chip.transform.parent = ownerCell.transform;
        
        return selfChip;
    }
    
    /** Определяет возможность фишке покинуть ячейку. */
	public abstract bool canLeave();
    
	/** Определяет возможность фишке войти в ячейку. */
	public abstract bool canEnter();
    
	/** Определяет возможность создать фишку внутри ячейки. */
	public abstract bool canContainChip();
}