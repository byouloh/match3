using UnityEngine;

public class BuilderCell: CellBehaviour
{
    private uint _chipTypes;

    public BuilderCell(Cell cell, Grid grid, uint chipTypes)
    {
        if (cell == null) {
            throw new System.NullReferenceException("BuilderCell::BuilderCell: cell can not be null");
        }

        if (grid == null) {
            throw new System.NullReferenceException("BuilderCell::BuilderCell: grid can not be null");
        }

        _cell      = cell;
        _grid      = grid;
        _chipTypes = chipTypes;
    }

    public override Chip takeChip(Cell caller)
    {
        if (_grid == null || _cell == null) {
            return null;
        }

        Chip chip = null;

        try {
            chip = _grid.createChipRandom(_chipTypes, _cell.gameObject);
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