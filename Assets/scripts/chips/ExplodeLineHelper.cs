using System.Collections.Generic;
using UnityEngine;

/**
 * Возвращает ячейки в строке.
 */
public class ExplodeLineHelper : IExplodeHelper
{
    /** Тип бонуса. */
    private BonusType _bonusType;
    
    /**
     * Конструктор.
     * 
     * @param bonusType тип бонуса
     */
    public ExplodeLineHelper(BonusType bonusType)
    {
        _bonusType = bonusType;
    }
    
    public Match affectCells(Cell currentCell, BonusInfo targetCellInfo)
    {
        if (currentCell == null) {
            return null;
        }
        
        Match res = new Match();
        
        Grid grid = Game.getInstance().getGrid();
        
        int n = (_bonusType == BonusType.HORIZONTAL_STRIP) ? grid.getColCount() : grid.getRowCount();
        
        for (int i = 0; i < n; i++) {
            Cell cell = (_bonusType == BonusType.HORIZONTAL_STRIP)
                        ? grid.getCell(currentCell.position.y, i)
                        : grid.getCell(i, currentCell.position.x);
            
            if (cell != null && cell != currentCell) {
                if (cell.chip != null) {
                    if (_bonusType == cell.chip.bonusType) {
                        if (_bonusType == BonusType.HORIZONTAL_STRIP) {
                            grid.changeChipType(cell, cell.chip.type, BonusType.VERTICAL_STRIP);
                        } else {
                            grid.changeChipType(cell, cell.chip.type, BonusType.HORIZONTAL_STRIP);
                        }
                    }
                }
                
                res.Add(cell);
            }
        }
        
        return res;
    }
}
