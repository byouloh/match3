using System.Collections.Generic;

/**
 * Возвращает ячейки в строке.
 */
public class ExplodeSameHelper : IExplodeHelper
{
    /**
     * Конструктор.
     * 
     * @param bonusType тип бонуса
     */
    public ExplodeSameHelper()
    {
        
    }
    
    public Match affectCells(Cell currentCell, BonusInfo targetCellInfo)
    {
        if (currentCell == null || targetCellInfo == null) {
            return null;
        }
        
        Match res = new Match();
        Grid grid = Game.getInstance().getGrid();
        
        ChipType cType  = targetCellInfo.cType;
        BonusType bType = targetCellInfo.bType;
        
        int i;
        int j;
        Cell cell;
        
        if (bType == BonusType.SAME_TYPE) {
            for (i = 0; i < grid.getRowCount(); i++) {
                for (j = 0; j < grid.getColCount(); j++) {
                    cell = grid.getCell(i, j);
                    
                    if (cell != null && cell != currentCell) {
                        res.Add(cell);
                    }
                }
            }
            
            return res;
        }
        
        for (i = 0; i < grid.getRowCount(); i++) {
            for (j = 0; j < grid.getColCount(); j++) {
                cell = grid.getCell(i, j);
                
                if (cell != null && cell != currentCell && cell.chip != null &&
                    cell.chip.bonusType != BonusType.SAME_TYPE && cell.chip.type == cType
                ) {
                    if (cell.chip.bonusType != bType) {
                        grid.changeChipType(cell, cType, bType);
                    }
                    
                    res.Add(cell);
                }
            }
        }
        
        return res;
    }
}
