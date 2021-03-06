﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//#define HELPER

/** Тип прохода. */
public enum PassType: int
{
    TOP_LEFT = 0,
    TOP_RIGHT,
    BOTTOM_LEFT,
    BOTTOM_RIGHT
}

/**
 * 
 * 
 * @author 
 */
public class MatchDetector
{
    /** 
     * Ссылки на обрабатываемую сетку клеток.
     */
    private Grid _grid;

    /** 
     * Массив взрывающихся линий . 
     */
    public Lines explosionLines = null;

    /**
     * Вводим сетку.
     * 
     * @param grid
     */
	public void setGrid(Grid grid) 
    {
        _grid = grid;
	}


    /** Метод проверяет сетку на наличие рядов.*/ 
	public void findMatches() 
    {
        int i, j, g;
        int chainLength;
        Cell cell1;
        Cell cell2;

        if (explosionLines != null) {
            explosionLines.Clear();
        } else {
            explosionLines = new Lines();
        }

        // Проверяем по строкам. 
        for (i = 0; i < _grid.getRowCount(); i++) {
            for (j = 0; j < (_grid.getColCount() - 2); j++) {
                chainLength = 1;

                while ((j + chainLength) < _grid.getColCount())  {
                    cell1 = _grid.getCell(i, j);
                    cell2 = _grid.getCell(i, j + chainLength);

                    if ((cell1 != null) && (cell2 != null) && 
                        (cell1.chip != null) && (cell2.chip != null) &&
                        (cell1.chip.compareTo(cell2.chip))
                    ) {
                        chainLength++;
                    } else {     
                        break;
                    }
                }

                //Debug.LogError();
                if (chainLength >= 3) {
                    // Запихиваем все в возвращаемый массив
                    Match tmpMatch = new Match();

                    for (g = j; g < (j + chainLength); g++){
                        tmpMatch.Add(_grid.getCell(i, g));   
                    }

                    explosionLines.Add(tmpMatch);
                }

                j += (chainLength - 1);
            }
        }

        // Проверяем по столбцам.
        for (j = 0; j < _grid.getColCount(); j++) {
            for (i = 0; i < (_grid.getRowCount() - 2); i++) {
                chainLength = 1;
                
                while ((i + chainLength) < _grid.getRowCount())  {
                    cell1 = _grid.getCell(i, j);
                    cell2 = _grid.getCell(i + chainLength, j);
                    
                    if ((cell1 != null) && (cell2 != null) && 
                        (cell1.chip != null) && (cell2.chip != null) &&
                        (cell1.chip.compareTo(cell2.chip))
                    ) {
                        chainLength++;
                    } else {     
                        break;
                    }
                }

                //Debug.LogError();
                if (chainLength >= 3) {
                    // Запихиваем все в возвращаемый массив
                    Match tmpMatch = new Match();

                    for (g = i; g < (i + chainLength); g++){
                        tmpMatch.Add(_grid.getCell(g, j));                 
                    }

                    explosionLines.Add(tmpMatch);
                }

                i += (chainLength - 1);
            }
        }
    }

    /** 
     * Метод ищет ряды для двух перестановленных фишек. 
     */
	void permutationCalculate(IntVector2 cell1, IntVector2 cell2)
	{
        explosionLines = null;
		
		if (_grid.getCell(cell1.x, cell1.y).chip.bonusType == BonusType.SAME_TYPE) {
		
        } else {
            //this.canCellExplosion(cell1);
            //this.canCellExplosion(cell2);
        }
	}

    /** 
     * Метод находит случайный возможный ход и возвращает его. 
     */
    public Match findHelpMatch()
    {
        System.Random r   = new System.Random();
        PassType passType = (PassType)r.Next(4); // Случайный тип прохода
        int i, j;
        Lines tmpLines;

        switch (passType) {
            case PassType.TOP_LEFT:

                #if HELPER
                Debug.Log("Вошли TOP_LEFT");
                #endif

                for (i = 0; i < _grid.getRowCount(); i++) {
                    for (j = 0; j < _grid.getColCount(); j++) {

                        #if HELPER
                        Debug.Log("Обрабатываемая клетка i = " + i + " j = " + j);
                        #endif

                        tmpLines = getFormedLines(new IntVector2(i, j));
                        
                        if (tmpLines.Count > 0) {
                            return getUniqueCells(tmpLines);
                        }
                    }
                }

                break;
            
            case PassType.TOP_RIGHT:

                #if HELPER
                Debug.Log("Вошли TOP_RIGHT");
                #endif

                for (j = _grid.getRowCount() - 1; j > 0; j--) {
                    for (i = 0; i < _grid.getColCount(); i++) {

                        #if HELPER
                        Debug.Log("Обрабатываемая клетка i = " + i + " j = " + j);
                        #endif

                        tmpLines = getFormedLines(new IntVector2(i, j));
                        
                        if (tmpLines.Count > 0) {
                            return getUniqueCells(tmpLines);
                        } 
                    }
                }

                break;
            
            case PassType.BOTTOM_LEFT:

                #if HELPER
                Debug.Log("Вошли BOTTOM_LEFT");
                #endif

                for (j = 0; j < _grid.getRowCount(); j++) {
                    for (i = _grid.getColCount() - 1; i > 0 ; i--) {

                        #if HELPER
                        Debug.Log("Обрабатываемая клетка i = " + i + " j = " + j);
                        #endif

                        tmpLines = getFormedLines(new IntVector2(i, j));
                        
                        if (tmpLines.Count > 0) {
                            return getUniqueCells(tmpLines);
                        }
                    }
                }

                break;
            
            case PassType.BOTTOM_RIGHT:

                #if HELPER
                Debug.Log("Вошли BOTTOM_RIGHT");
                #endif


                for (i = _grid.getColCount() - 1; i > 0; i--) {
                    for (j = _grid.getRowCount() - 1; j > 0; j--) {

                        #if HELPER
                        Debug.Log("Обрабатываемая клетка i = " + i + " j = " + j);
                        #endif

                        tmpLines = getFormedLines(new IntVector2(i, j));
                        
                        if (tmpLines.Count > 0) {
                            return getUniqueCells(tmpLines);
                        }
                    }
                }

                break;
            
            default:
                Debug.LogError("Ошибка! Выбран неправильный тип прохода");
                break;
        }

        return new Match();
    }

    /**
     * Из линий возвращает массив уникальных точек.
     * 
     * @param lines 
     */
    public Match getUniqueCells(Lines lines)
    {
        Match match = new Match();

        for (int i = 0; i < lines.Count; i++) {
            for (int j = 0; j < lines[i].Count; j++) {
                if (!match.Contains(lines[i][j])) {
                    match.Add(lines[i][j]);
                }
            }
        }

        return match;
    }

    /**
     *  Метод проверяет все возможные сочетания линий для данной клетки и 
     *  возвращает массив линий которые образуют mathc.
     * 
     * @param cell координаты ячейки которую обрабатываем.
     */
    Lines getFormedLines(IntVector2 cell)
    {
        Lines lines = null;
        IntVector2[] directions = {IntVector2.up, IntVector2.down, IntVector2.left, IntVector2.rigth};

        // Если фишки нету или клетки
        if ((_grid.getCell(cell.x, cell.y) == null) ||
            (_grid.getCell(cell.x, cell.y).chip == null)
        ) {

            #if HELPER
            Debug.Log("клетки или фишки нет [" + cell.x + ", " + cell.y + "]");
            #endif

            return new Lines();
        }

        for (int i = 0; i < 4; i++) {

            #if HELPER
            Debug.Log("направление " + i + "        0 - up, 1 - down, 2 - left, 3 - rigth   для клетки[" + cell.x + ", " + cell.y + "]");
            #endif

            lines = getFormedLinesForSide(cell, directions[i]);

            if (lines.Count > 0) {
                return lines;
            }
        }

        return new Lines();
    }

    Lines getFormedLinesForSide(IntVector2 cell, IntVector2 offset) 
    {
        #if HELPER
        Debug.Log(" << slide , cell [" + cell.x + "][" + cell.y + "] offset  = (" + offset.x + ", " + offset.y + ") ");
        #endif

        Lines foundMatch = new Lines();
        
        int verticalMatch;
        int horisontalMatch;

        IntVector2 cellCoordinates;

        cellCoordinates.x = cell.x + offset.x;
        cellCoordinates.y = cell.y + offset.y;

        // Если куда мы можем переставить выходит за поле по х
        if ((cellCoordinates.x < 0) || (cellCoordinates.x > (_grid.getColCount() - 1))) {
            #if HELPER
            Debug.Log("Клетка вышла за сетку по x < cell [" + cell.x + "][" + cell.y + "]");
            #endif

            return foundMatch;
        }

        // Если куда мы можем переставить выходит за поле по y
        if ((cellCoordinates.y < 0) || (cellCoordinates.y > (_grid.getRowCount() - 1))) {
            #if HELPER
            Debug.Log("Клетка вышла за сетку по y < cell [" + cell.x + "][" + cell.y + "]");
            #endif

            return foundMatch;
        }

        // Если клетка не существует или на ней нет фишки
        if (_grid.getCell(cellCoordinates.x, cellCoordinates.y) == null) {
            #if HELPER
            Debug.Log("Клетка не существует < cell [" + cellCoordinates.x + "][" + cellCoordinates.y + "] для клетки [" + cell.x + "][" + cell.y + "]");
            #endif  

            return foundMatch;
        } else if (_grid.getCell(cellCoordinates.x, cellCoordinates.y).chip == null) {
            #if HELPER
            Debug.Log("фишки на клетке нет < cell [" + cellCoordinates.x + "][" + cellCoordinates.y + "] для клетки [" + cell.x + "][" + cell.y + "]");
            #endif

            return foundMatch;
        } else {
            #if HELPER
            Debug.Log("клетка существует и на ней есть фишка < cell [" + cellCoordinates.x + "][" + cellCoordinates.y + "] для клетки [" + cell.x + "][" + cell.y + "]");
            #endif   
        }

        // Если клетки одинаковые и не бонусные
        if ((_grid.getCell(cell.x, cell.y).chip.compareTo(_grid.getCell(cellCoordinates.x, cellCoordinates.y).chip)) && 
            ((_grid.getCell(cell.x, cell.y).chip.bonusType == BonusType.NONE) && 
         (_grid.getCell(cellCoordinates.x, cellCoordinates.y).chip.bonusType == BonusType.NONE))
        ) {
            #if HELPER
            Debug.Log("Клетки одинаковые < cell [" + cellCoordinates.x + "][" + cellCoordinates.y + "] для клетки [" + cell.x + "][" + cell.y + "]");
            #endif

            return foundMatch;
        }

        if (!_grid.getCell(cell.x, cell.y).canLeave() || 
            !_grid.getCell(cellCoordinates.x, cellCoordinates.y).canEnter()
        ) {
            return foundMatch;
        }

        Chip useChip = _grid.getCell(cell.x, cell.y).chip;

        int topX    = cellCoordinates.x;
        int bottomX = cellCoordinates.x;
        int leftY   = cellCoordinates.y;
        int rightY  = cellCoordinates.y;
        
        bool canUp    = true;
        bool canDown  = true;
        bool canLeft  = true;
        bool canRight = true;


        if ((offset.x == 1) && (offset.y == 0)) {

            #if HELPER
            Debug.Log("клетка которую мы передвигаем с верху cell [" + cell.x + "][" + cell.y + "]");
            #endif

            canUp = false;
        }

        if ((offset.x == -1) && (offset.y == 0)) {

            #if HELPER
            Debug.Log("клетка которую мы передвигаем с низу cell [" + cell.x + "][" + cell.y + "]");
            #endif

            canDown = false;
        }

        if ((offset.x == 0) && (offset.y == 1)) {

            #if HELPER
            Debug.Log("клетка которую мы передвигаем слева cell [" + cell.x + "][" + cell.y + "]");
            #endif

            canLeft = false;
        }

        if ((offset.x == 0) && (offset.y == -1)) {

            #if HELPER
            Debug.Log("клетка которую мы передвигаем с права cell [" + cell.x + "][" + cell.y + "]");
            #endif

            canRight = false;
        }
        
        while (true) {
            // Смотрим вверх
            if (canUp) { 
                if (((topX - 1) >= 0) && 
                    (_grid.getCell((topX - 1), cellCoordinates.y) != null) &&
                    (_grid.getCell((topX - 1), cellCoordinates.y).chip != null) && 
                    (_grid.getCell((topX - 1), cellCoordinates.y).chip.compareTo(useChip))
                ) {
                    topX -= 1;
                } else {
                    canUp = false;
                }
            }

            // Смотрим вниз 
            if (canDown) { 
                if (((bottomX + 1) < _grid.getRowCount()) &&
                    (_grid.getCell((bottomX + 1), cellCoordinates.y) != null) &&
                    (_grid.getCell((bottomX + 1), cellCoordinates.y).chip != null) &&
                    (_grid.getCell((bottomX + 1), cellCoordinates.y).chip.compareTo(useChip))
                ) {
                    bottomX += 1;
                } else {
                    canDown = false;
                }
            }

            // Смотрим влево
            if (canLeft) { 
                if (((leftY - 1) >= 0) && 
                    (_grid.getCell(cellCoordinates.x, (leftY - 1)) != null) &&
                    (_grid.getCell(cellCoordinates.x, (leftY - 1)).chip != null) &&
                    (_grid.getCell(cellCoordinates.x, (leftY - 1)).chip.compareTo(useChip))
                ) {
                    leftY -= 1;
                } else {
                    canLeft = false;
                } 
            }
            // Смотрим  вправо 
            if (canRight) { 
                if (((rightY + 1) < _grid.getColCount()) && 
                    (_grid.getCell(cellCoordinates.x, (rightY + 1)) != null) &&
                    (_grid.getCell(cellCoordinates.x, (rightY + 1)).chip != null) &&
                    (_grid.getCell(cellCoordinates.x, (rightY + 1)).chip.compareTo(useChip))
                ) {
                    rightY += 1;
                } else {
                    canRight = false;
                }
            }

            if (!canUp && !canDown && !canLeft && !canRight) {
                break;
            }
        }

        // Теперь надо вернуть строки которые должны будут взрыватся
        verticalMatch = bottomX - topX + 1; 

        // если у нас есть три или более в ряд по вертикали
        if (verticalMatch >= 3) {
            Match tmpMatch = new Match();

            for (int i = topX; i <= bottomX ; i++ ) {
                if (i != cellCoordinates.x) {

                    #if HELPER
                    Debug.Log("клетка i вертикали " + (i - topX + 1));
                    Debug.Log("x = " + i + " y = " + (cell.y + offset.y));
                    #endif

                    tmpMatch.Add(_grid.getCell(i, (cell.y + offset.y)));
                }
            }

            #if HELPER
            Debug.Log("x = " + cell.x + " y = " + cell.y);
            #endif

            tmpMatch.Add(_grid.getCell(cell.x, cell.y));
            foundMatch.Add(tmpMatch);
        }

        // если у нас есть три или более в ряд по горизонтали 
        horisontalMatch = rightY - leftY + 1; 

        // если у нас есть три или более в ряд по вертикали
        if (horisontalMatch >= 3) {
            Match tmpMatch = new Match();

            for (int i = leftY; i <= rightY ; i++ ) {
                if (i != cellCoordinates.y) {

                    #if HELPER
                    Debug.Log("клетка i по горизонтали " + (i - leftY + 1));
                    Debug.Log("x = " + (cell.x + offset.x) + " y = " + i);
                    #endif

                    tmpMatch.Add(_grid.getCell((cell.x + offset.x), i));
                }
            }

            #if HELPER
            Debug.Log("x = " + cell.x + " y = " + cell.y);
            #endif

            tmpMatch.Add(_grid.getCell(cell.x, cell.y));
            foundMatch.Add(tmpMatch);
        }

        // Если обе фишки бонусные
        if ((_grid.getCell(cell.x, cell.y).chip.bonusType != BonusType.NONE) && 
            (_grid.getCell(cellCoordinates.x, cellCoordinates.y).chip.bonusType != BonusType.NONE)
           ) {

            Match tmpMatch = new Match();
            tmpMatch.Add(_grid.getCell(cell.x, cell.y));
            tmpMatch.Add(_grid.getCell(cellCoordinates.x, cellCoordinates.y));

            foundMatch.Add (tmpMatch);
        }

        // Если хоть одна фишка цветная.
        if ((_grid.getCell(cell.x, cell.y).chip.bonusType == BonusType.SAME_TYPE) || 
            (_grid.getCell(cellCoordinates.x, cellCoordinates.y).chip.bonusType == BonusType.SAME_TYPE)
            ) {
            
            Match tmpMatch = new Match();
            tmpMatch.Add(_grid.getCell(cell.x, cell.y));
            tmpMatch.Add(_grid.getCell(cellCoordinates.x, cellCoordinates.y));
            
            foundMatch.Add (tmpMatch);
        }

        return foundMatch;
     }
}