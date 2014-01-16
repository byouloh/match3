using UnityEngine;

public class NormalCell: CellBehaviour
{
    public NormalCell(Cell cell, Grid grid)
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
        if (_grid == null || _cell == null || caller == null || 
            (!_cell.isEmpty() && _cell.position.x == caller.position.x)
        ) {
            return null;
        }

        Chip chip = null;

        if (!_cell.isEmpty() && _cell.canLeave()) {
            chip       = _cell.chip;
            _cell.chip = null;
        } else if (_cell.position.x == caller.position.x) {
            // Запрос фишки у верхней ячейки
            if (_cell.position.x > 0) {
                Cell cell = _grid.getCell(_cell.position.x - 1, _cell.position.y);

                chip = cell.takeChip(caller);

                if (chip == null && 
                    cell.isEmpty() && 
                    cell.canEnter() && 
                    cell.canLeave() && 
                    cell.canContainChip()
                ) {
                    return null;
                }

                // Запрос фишки у верхней ячейки слева
                if (chip == null && _cell.position.y > 0) {
                    chip = _grid.getCell(_cell.position.x - 1, _cell.position.y - 1).takeChip(caller);
                }

                // Запрос фишки у верхней ячейки справа
                if (chip == null && _cell.position.y < _grid.getColCount() - 1) {
                    cell = _grid.getCell(_cell.position.x, _cell.position.y + 1);

                    if (!cell.isEmpty() && cell.canContainChip()) {
                        chip = _grid.getCell(_cell.position.x - 1, _cell.position.y + 1).takeChip(caller);
                    }
                }
            }

            if (chip != null) {
                // Забираем фишку у ячейки сверху.
                _cell.setChip(chip);
            }
        }
    
        return chip;
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