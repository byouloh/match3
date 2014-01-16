using System.Collections.Generic;

/**
 * Интерфейс для классов по вычислению взрывающихся ячеек.
 * 
 * @author Timur Bogotov timur@e-magic.org
 */
public interface IExplodeHelper
{
    /**
     * Меняет или манипулирует ячейками перед взрывом ячеек.
     * 
     * @param currentCell текущая ячейка
     * @param targetCellInfo информация о фишке, которую мы переставляем вместе с текущей
     * 
     * @return Match список всех затрагиваемых ячеек
     */
    Match affectCells(Cell currentCell, BonusInfo targetCellInfo);
}
