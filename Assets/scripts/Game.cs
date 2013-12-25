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

    /** Информация об уровне. */
    private Level level;

	/** Инициализация. */
	void Start()
	{
        loadLevel(1);
	}

	void Update()
	{
		
	}

	void OnGUI()
	{
		
	}

    /**
     * Загружает уровень.
     * 
     * @param levelId номер уровня
     */
    public void loadLevel(int levelId)
    {
        // Данные уровня
        this.level                      = new Level();
        this.level.levelId              = levelId;
        this.level.maxMoves             = 20;
        this.level.needPointsFirstStar  = 1000;
        this.level.needPointsSecondStar = 2000;
        this.level.needPointsThirdStar  = 3000;
        
        int i;
        int j;
        
        int rowCount = 4;
        int colCount = 5;
        
        string cellsString = "0,513,1,1,0,1,1,1,1,1,1,514,3,4,1,0,1,5,1,0";
        
        // Загружаем ячейки
        grid = new Grid(rowCount, colCount);
        
        string[] cellItems = cellsString.Split(',');
        
        if (cellItems.Length != grid.getRowCount() * grid.getColCount()) {
            Debug.LogError("Ошибка! Неправильные данные загружаемого уровня!");
            return;
        }
        
        int ii = 0;
        
        for (i = 0; i < grid.getRowCount(); i++) {
            for (j = 0; j < grid.getColCount(); j++) {
                int cellInfo = int.Parse(cellItems[ii++]);
                
                if (cellInfo == 0) {
                    grid.setCell(i, j, null);
                } else {
                    GameObject cell = (GameObject)UnityEngine.Object.Instantiate(cellPrefab);
                    cell.transform.parent   = cellsRoot.transform;
                    cell.transform.position = new Vector3(Grid.CELL_WIDTH * j, -Grid.CELL_HEIGHT * i, 0);
                    
                    Cell c = cell.GetComponent<Cell>();
                    
                    CellBlocker blocker;
                    
                    int cellType  = cellInfo & 0xFF;
                    int chipType  = (cellInfo >> 8) & 0xF;
                    int bonusType = (cellInfo >> 12) & 0xF;
                    
                    switch (cellType) {
                        case 1:
                            blocker = BlockFactory.createNew(BlockerType.NONE, c.gameObject);
                            break;
                        case 2:
                            blocker = BlockFactory.createNew(BlockerType.CHAIN, c.gameObject);
                            break;
                        case 3:
                            blocker = BlockFactory.createNew(BlockerType.CHAIN2, c.gameObject);
                            break;
                        case 4:
                            blocker = BlockFactory.createNew(BlockerType.WRAP, c.gameObject);
                            break;
                        case 5:
                            blocker = BlockFactory.createNew(BlockerType.WRAP2, c.gameObject);
                            break;
                        default:
                            Debug.LogError("Ошибка! Неверный тип ячейки");
                            blocker = BlockFactory.createNew(BlockerType.NONE, c.gameObject);
                            break;
                    }
                    
                    Chip chip = null;
                    
                    if (chipType != 0) {
                        BonusType bType;
                        
                        switch (bonusType) {
                            case 0:
                                bType = BonusType.NONE;
                                break;
                            case 1:
                                bType = BonusType.HORIZONTAL_STRIP;
                                break;
                            case 2:
                                bType = BonusType.VERTICAL_STRIP;
                                break;
                            case 3:
                                bType = BonusType.SAME_TYPE;
                                break;
                            default:
                                Debug.LogError("Ошибка! Неверный тип бонуса");
                                bType = BonusType.NONE;
                                break;
                        }
                        
                        switch (chipType) {
                            case 1:
                                chip = ChipFactory.createNew(ChipType.RED, bType, c.gameObject);
                                break;
                            case 2:
                                chip = ChipFactory.createNew(ChipType.GREEN, bType, c.gameObject);
                                break;
                            case 3:
                                chip = ChipFactory.createNew(ChipType.BLUE, bType, c.gameObject);
                                break;
                            case 4:
                                chip = ChipFactory.createNew(ChipType.YELLOW, bType, c.gameObject);
                                break;
                            case 5:
                                chip = ChipFactory.createNew(ChipType.ORANGE, bType, c.gameObject);
                                break;
                            case 6:
                                chip = ChipFactory.createNew(ChipType.PURPLE, bType, c.gameObject);
                                break;
                            default:
                                Debug.LogError("Ошибка! Неверный тип фишки");
                                break;
                        }
                    }
                    
                    c.initialize(blocker, chip);
                    
                    grid.setCell(i, j, c);
                }
            }
        }
        
        // Отцентровываем контейнер для ячеек
        cellsRoot.transform.position = new Vector3(Grid.CELL_WIDTH*0.5f - Grid.CELL_WIDTH * grid.getColCount()*0.5f,
                                                   -Grid.CELL_HEIGHT*0.5f + Grid.CELL_HEIGHT * grid.getRowCount()*0.5f, 0);
    }
}
