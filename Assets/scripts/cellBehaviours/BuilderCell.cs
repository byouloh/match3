using UnityEngine;

public class BuilderCell: CellBehaviour
{
    public override Chip takeChip()
    {
        Chip chip = null;

        try {
            chip = _grid.createChipRandom(_chipTypes, _grid.getCell(_cellPosition.x, _cellPosition.y).gameObject);
        } catch (System.Exception e) {
            Debug.LogError("Chip create error in BuilderCell::takeChip(), eror message: " + e.Message);
        }
        
        if (chip != null) {
            chip.transform.parent = _grid.getCell(_cellPosition.x, _cellPosition.y).transform;
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