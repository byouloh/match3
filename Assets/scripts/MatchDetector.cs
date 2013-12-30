using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MatchDetector 
{
    private Grid _grid;
    public Lines explosionLines = null;
	
    // Use this for initialization
	void Start ()
    {
		
	}

	public void setGrid (Grid grid) 
    {
		_grid = grid;
	}


    /** Метод проверяет сетку на наличие рядов.*/ 
	public void findMatch() 
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
                         (cell1.chip.type == cell2.chip.type)
                    ) {
                        chainLength++;
                    } else {     
                        break;
                    }
                }

                //Debug.LogError();
                if (chainLength >= 3) {
                    // Запихиваем все в возвращаемый массив
                    Match tmpMatch = new Match ();
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
                        (cell1.chip.type == cell2.chip.type)
                    ) {
                        chainLength++;
                    } else {     
                        break;
                    }
                }

                //Debug.LogError();
                if (chainLength >= 3) {
                    // Запихиваем все в возвращаемый массив
                    Match tmpMatch = new Match ();

                    for (g = i; g < (i + chainLength); g++){
                        tmpMatch.Add(_grid.getCell(g, j));                 
                    }

                    explosionLines.Add(tmpMatch);
                }

                i += (chainLength - 1);
            }
        }
    }

	void permutationCalculate(IntVector2 cell1, IntVector2 cell2)
	{
        explosionLines = null;
		
		if (_grid.getCell(cell1.x, cell1.y).chip.bonusType == BonusType.SAME_TYPE) {
		
        } else {
            this.canCellExplosion(cell1);
            this.canCellExplosion(cell2);
        }
	}

    /**
     *  Метод проверяет все возможные сочетания линий для данной клетки и 
     *  возвращает массив линий которые должны взорватся.
     * 
     * @param cell координаты ячейки которую обрабатываем.
     */
    void canCellExplosion(IntVector2 cell) 
    {
        int verticalMatch;
        int horisontalMatch;

        int topX    = cell.x;
        int bottomX = cell.x;
        int leftY   = cell.x;
        int rightY  = cell.x;

        bool canUp    = true;
        bool canDown  = true;
        bool canLeft  = true;
        bool canRight = true;

        // Понял что сделано не по феншую, позже переделать так что бы не было повторного повторения кода
        while (true) {
            // Смотрим по вверх
            if (canUp) { // Если в цепочке небыло прерываний
                if ((topX - 1) >= 0) { // Если мы не дошли до границы сетки
                    if (_grid.getCell((topX - 1), cell.y).chip.type == _grid.getCell(cell.x, cell.y).chip.type) {
                        topX -= 1;
                    } else {
                        canUp = false;
                    }
                }
            }

            // Смотрим по вниз 
            if (canDown) { // Если в цепочке небыло прерываний
                if ((bottomX + 1) <= _grid.getRowCount()) { // Если мы не дошли до границы сетки
                    if (_grid.getCell((bottomX + 1), cell.y).chip.type == _grid.getCell(cell.x, cell.y).chip.type) {
                        bottomX += 1;
                    } else {
                        canDown = false;
                    }
                }
            }

            // Смотрим в лево
            if (canLeft) { // Если в цепочке небыло прерываний
                if ((leftY - 1) >= 0) { // Если мы не дошли до границы сетки
                    if (_grid.getCell(cell.x, (leftY - 1)).chip.type == _grid.getCell(cell.x, cell.y).chip.type) {
                        leftY -= 1;
                    } else {
                        canLeft = false;
                    } 
                }
            }
            
            // Смотрим  в право 
            if (canRight) { // Если в цепочке небыло прерываний
                if ((rightY + 1) <= _grid.getColCount()) { // Если мы не дошли до границы сетки
                    if (_grid.getCell(cell.x, (rightY + 1)).chip.type == _grid.getCell(cell.x, cell.y).chip.type) {
                        rightY += 1;
                    } else {
                        canRight = false;
                    }
                }
            }

            if ((canUp && canDown) && (canLeft && canRight)) {
                break;
            }
        }

        // Теперь надо вернуть строки которые должны будут взрыватся
        verticalMatch = bottomX - topX + 1; 

        // если у нас есть три или более в ряд по вертикали
        if (verticalMatch >= 3) {
            Match tmpMatch = new Match();

            for (int i = topX; i <= bottomX ; i++ ) {
                tmpMatch.Add(_grid.getCell(i, cell.y));
            }

            explosionLines.Add(tmpMatch);
        }

        // если у нас есть три или более в ряд по горизонтали 
        horisontalMatch = bottomX - topX + 1; 

        // если у нас есть три или более в ряд по вертикали
        if (horisontalMatch >= 3) {
            Match tmpMatch = new Match();

            for (int i = topX; i <= bottomX ; i++ ) {
                tmpMatch.Add( _grid.getCell(i, cell.y));
            }

            explosionLines.Add(tmpMatch);
        }

        //return explosionLines;
    }
}