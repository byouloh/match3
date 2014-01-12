
public class NormalCell: CellBehaviour
{
    public override Chip takeChip()
    {
        if (_grid == null || _cellPosition.x <= 0) {
            return null;
        }
        
        // Запрос фишки у верхней ячейки
        Chip chip = _grid.getCell(_cellPosition.x - 1, _cellPosition.y).takeChip();
        
        if (chip == null) {
            if (_cellPosition.y > 0) {
                // Запрос фишки у верхней ячейки слева
                chip = _grid.getCell(_cellPosition.x - 1, _cellPosition.y - 1).takeChip();
            }
            
            if (chip == null && _cellPosition.y < _grid.getColCount() - 1) {
                // Запрос фишки у верхней ячейки справа
                chip = _grid.getCell(_cellPosition.x - 1, _cellPosition.y + 1).takeChip();
            }
        }
        
        return _moveTakedAndGetSelfChip(chip);
    }
    
    /** Определяет возможность фишке покинуть ячейку. */
    public override bool canLeave()
    {
        return true;
    }
    
	/** Определяет возможность фишке войти в ячейку. */
    public override bool canEnter()
    {
        return true;
    }
    
	/** Определяет возможность создать фишку внутри ячейки. */
    public override bool canContainChip()
    {
        return true;
    }
}