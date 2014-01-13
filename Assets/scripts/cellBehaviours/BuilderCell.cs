using UnityEngine;

public class BuilderCell: CellBehaviour
{
    private uint _chipTypes;

    public BuilderCell(IntVector2 cellPosition, Grid grid, uint chipTypes)
    {
        _cellPosition = cellPosition;
        _grid         = grid;
        _chipTypes    = chipTypes;
    }

    public override Chip takeChip(int depth)
    {
        if (_grid == null) {
            return null;
        }

        Chip chip = null;

        try {
            chip = _grid.createChipRandom(_chipTypes, _grid.getCell(_cellPosition.x, _cellPosition.y).gameObject);
        } catch (System.Exception e) {
            Debug.LogError("Chip create error in BuilderCell::takeChip(), eror message: " + e.Message);
        }

        return chip;
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