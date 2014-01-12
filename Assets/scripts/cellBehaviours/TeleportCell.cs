
public class TeleportCell: CellBehaviour
{
    public override Chip takeChip()
    {
        if (_grid == null || _cellPosition.x <= 0) {
            return null;
        }
        
        // Запрос фишки только у верхней ячейки, т.е. бо бокам не смотрим.
        Chip chip = _grid.getCell(_cellPosition.x - 1, _cellPosition.y).takeChip();
        
        return _moveTakedAndGetSelfChip(chip);
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