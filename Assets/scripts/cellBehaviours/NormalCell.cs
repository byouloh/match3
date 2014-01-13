using UnityEngine;

public class NormalCell: CellBehaviour
{
    public NormalCell(IntVector2 cellPosition, Grid grid)
    {
        _cellPosition = cellPosition;
        _grid         = grid;
    }

    public override Chip takeChip(int depth)
    {
        if (_grid == null || _cellPosition.x <= 0) {
            return null;
        }

        Cell cell = _grid.getCell(_cellPosition.x, _cellPosition.y);

        if (cell == null || (depth == 0 && !cell.isEmpty()) || depth > 1) {
            return null;
        }

        Chip chip = null;

        if (depth == 1) {
            if (!cell.isEmpty() && cell.canLeave()) {
                chip      = cell.chip;
                cell.chip = null;
            }
        } else {
            // Запрос фишки у верхней ячейки
            cell = _grid.getCell(_cellPosition.x - 1, _cellPosition.y);

            if (cell != null) {
                chip = cell.takeChip(depth + 1);
            }

            if (chip == null) {
                if (_cellPosition.y > 0) {
                    // Запрос фишки у верхней ячейки слева
                    cell = _grid.getCell(_cellPosition.x - 1, _cellPosition.y - 1);

                    if (cell != null) {
                        chip = cell.takeChip(depth + 1);
                    }
                }
                
                if (chip == null && _cellPosition.y < _grid.getColCount() - 1) {
                    // Запрос фишки у верхней ячейки справа

                    cell = _grid.getCell(_cellPosition.x - 1, _cellPosition.y + 1);

                    if (cell != null) {
                        chip = cell.takeChip(depth + 1);
                    }
                }
            }

            if (chip != null) {
                // Забираем фишку у ячейки сверху.
                _grid.getCell(_cellPosition.x, _cellPosition.y).setChip(chip);
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