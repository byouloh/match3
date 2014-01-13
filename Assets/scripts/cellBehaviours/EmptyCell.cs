
public class EmptyCell: CellBehaviour
{
    public override Chip takeChip(int depth)
    {
        return null;
    }
    
    /** Определяет возможность фишке покинуть ячейку. */
    public override bool canLeave()
    {
        return false;
    }
    
	/** Определяет возможность фишке войти в ячейку. */
    public override bool canEnter()
    {
        return false;
    }
    
	/** Определяет возможность создать фишку внутри ячейки. */
	public override bool canContainChip()
    {
        return false;
    }
}