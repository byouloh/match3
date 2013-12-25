using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Основной класс игры
 * 
 * @author Timur Bogotov timur@e-magic.org
 */
public class Game : MonoBehaviour
{
	/** Контейнер для ячеек. */
	public GameObject cellsRoot;

	/** Шаблон ячейки. */
	public GameObject cellPrefab;

	/** Матрица ячеек. */
	private Grid grid;

	/** Инициализация. */
	void Start()
	{
		grid = new Grid(9, 9);
		testInitialize(cellsRoot, cellPrefab);
		cellsRoot.transform.position = new Vector3(Grid.CELL_WIDTH*0.5f - Grid.CELL_WIDTH * grid.getColCount()*0.5f,
		                                           -Grid.CELL_HEIGHT*0.5f + Grid.CELL_HEIGHT * grid.getRowCount()*0.5f, 0);
	}

	/**
	 * Создает тестовую матрицу и инициализирует ячейки.
	 * 
	 * @param cellsRoot контейнер для всех ячеек
	 * @param cellPrefab шаблон ячейки
	 */
	public void testInitialize(GameObject cellsRoot, GameObject cellPrefab)
	{
		for (int i = 0; i < grid.getRowCount(); i++) {
			for (int j = 0; j < grid.getColCount(); j++) {
				GameObject cell = (GameObject)UnityEngine.Object.Instantiate(cellPrefab);
				cell.transform.parent   = cellsRoot.transform;
				cell.transform.position = new Vector3(Grid.CELL_WIDTH * j, -Grid.CELL_HEIGHT * i, 0);
				
				Cell c = cell.GetComponent<Cell>();
				
				CellBlocker blocker = BlockFactory.createNew(BlockerType.NONE, c.gameObject);
				Chip chip = ChipFactory.createNew(Random.Range(0, 5), BonusType.NONE, c.gameObject);
				c.initialize(blocker, chip);

				grid.setCell(i, j, c);
			}
		}
	}

	void Update()
	{
		
	}

	void OnGUI()
	{
		
	}
}
