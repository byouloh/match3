using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Класс для работы и хранения матрицы ячеек.
 * 
 * @author Timur Bogotov timur@e-magic.org
 */
public class Grid
{
	/** Ширина одной ячейки. */
	public static float CELL_WIDTH  = 0.5f;

	/** Высота одной ячейки. */
	public static float CELL_HEIGHT = 0.5f;

	/** Матрица ячеек. */
	private List<List<Cell>> _cells;

	/** Количество строк в матрице. */
	private int _rowCount;

	/** Количество столбцов в матрице. */
	private int _colCount;

	/**
	 * Создает пустую матрицу.
	 * 
	 * @param rowCount количество строк
	 * @param colCount количество столбцов
     */
	public Grid(int rowCount, int colCount)
	{
		_rowCount = rowCount;
		_colCount = colCount;

		_cells = new List<List<Cell>>();
		
		for (int i = 0; i < rowCount; i++) {
			_cells.Add(new List<Cell>());

			for (int j = 0; j < colCount; j++) {
				_cells[i].Add(null);
			}
		}
	}

	/**
	 * Возвращает указатель на ячейку по заданным индексам.
	 * 
	 * @param row номер строки
	 * @param col номер столбца
	 */
	public Cell getCell(int row, int col)
	{
		return _cells[row][col];
	}

	/**
	 * Задает ячейку по заданным индексам row и col.
	 * 
	 * @param row номер строки
	 * @param col номер столбца
	 * @param cell ячейка
	 */
	public void setCell(int row, int col, Cell cell)
	{
		_cells[row][col] = cell;
	}

	/** Возвращает количество строк. */
	public int getRowCount()
	{
		return _rowCount;
	}

	/** Возвращает количество столбцов. */
	public int getColCount()
	{
		return _colCount;
	}
}
