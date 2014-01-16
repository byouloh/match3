using UnityEngine;

public class TeleporterCell: CellBehaviour
{
    public TeleporterCell(Cell cell, Grid grid)
    {
        if (cell == null) {
            throw new System.NullReferenceException("BuilderCell::BuilderCell: cell can not be null");
        }
        
        if (grid == null) {
            throw new System.NullReferenceException("BuilderCell::BuilderCell: grid can not be null");
        }

        _cell = cell;
        _grid = grid;
    }

    public override Chip takeChip(Cell caller)
    {
        if (_grid == null || caller == null || 
            _cell.position.x <= 0 || 
            _cell.position.y != caller.position.y
        ) {
            return null;
        }

        // Запрос фишки только у верхней ячейки, т.е. бо бокам не смотрим.
        Cell cell = _grid.getCell(_cell.position.x - 1, _cell.position.y);
        Chip chip = cell.takeChip(caller);

        if (chip != null) {
            if (!(cell.cellBehaviour is TeleporterCell)) {
                _cell.setChip(chip);

                Chip cloneChip = chip.clone();

                cloneChip.transform.parent        = _cell.transform;
                cloneChip.transform.localPosition = Vector3.zero;

                return cloneChip;
            } else {
                chip.transform.parent        = _cell.transform;
                chip.transform.localPosition = Vector3.zero;
            }
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