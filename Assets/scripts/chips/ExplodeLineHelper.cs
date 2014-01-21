using System.Collections.Generic;
using UnityEngine;

/**
 * Возвращает ячейки в строке.
 * 
 * @author Timur Bogotov timur-emagic.org
 */
public class ExplodeLineHelper: IExplodeHelper
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
        
        if (currentCell == null) {
            return res;
        }
        
        Grid grid = Game.getInstance().getGrid();
        
        int n = (_bonusType == BonusType.HORIZONTAL_STRIP) ? grid.getColCount() : grid.getRowCount();
        
        for (int i = 0; i < n; i++) {
            Cell cell = (_bonusType == BonusType.HORIZONTAL_STRIP)
                        ? grid.getCell(currentCell.position.x, i)
                        : grid.getCell(i, currentCell.position.y);
            
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
