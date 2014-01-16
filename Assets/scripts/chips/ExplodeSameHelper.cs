using System.Collections.Generic;

/**
 * Возвращает ячейки в строке.
 * 
 * @author Timur Bogotov timur@e-magic.org
 */
public class ExplodeSameHelper: IExplodeHelper
{
    /**
     * Меняет или манипулирует ячейками перед взрывом ячеек.
     * 
     * @param currentCell текущая ячейка
     * @param targetCellInfo информация о фишке, которую мы переставляем вместе с текущей
     * 
     * @return Match список всех затрагиваемых ячеек
     */
    public Match affectCells(Cell currentCell, BonusInfo targetCellInfo)
    {
        Match res = new Match();
        
        if (currentCell == null || targetCellInfo == null) {
            return res;
        }
        
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
