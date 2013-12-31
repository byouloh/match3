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
    
    public static bool blockTime = true;
    
	/** Инициализация. */
	void Start()
	{
        loadLevel(1);
        StartCoroutine(grid.generateChips(3));
        //grid.generateChips(level.chipTypes);
	}
    
	void Update()
	{
		if (Input.GetMouseButtonUp(0)) {
            blockTime = false;
        }
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
        QueryResult res = DataBase.getInstance().query(
            "SELECT " +
                "l.maxMoves, l.existChips, " +
                "l.starPoints1, l.starPoints2, l.starPoints3, " +
                "c.rows, c.cols, c.cells " +
            "FROM levels AS l " +
            "INNER JOIN level_cells AS c ON (c.levelNum = l.num) " +
            "WHERE (l.num = " + levelId + ") " +
            "LIMIT 1"
        );
        
        if (res.numRows() <= 0) {
            Debug.LogError("Ошибка! Не удалось загрузить данные из базы данных");
            return;
        }
        
        DataRow info = res[0];
        
        // Данные уровня
        this.level                      = new Level();
        this.level.levelId              = levelId;
        this.level.maxMoves             = info.asInt("maxMoves");
        this.level.chipTypes            = (uint)info.asInt("existChips");
        this.level.needPointsFirstStar  = info.asInt("starPoints1");
        this.level.needPointsSecondStar = info.asInt("starPoints2");
        this.level.needPointsThirdStar  = info.asInt("starPoints3");
        
        int i;
        int j;
        
        int rowCount = info.asInt("rows");
        int colCount = info.asInt("cols");
        
        string cellsString = info.asString("cells");
        
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
                    
                    int blockerType = cellInfo & 0xFF;
                    int chipType    = (cellInfo >> 8) & 0xF;
                    int bonusType   = (cellInfo >> 12) & 0xF;
                    
                    switch (blockerType) {
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
                    
                    if (chipType > 0) {
                        try {
                            chip = ChipFactory.createNew((ChipType)(chipType - 1), (BonusType)bonusType, c.gameObject);
                        } catch(System.Exception) {
                            Debug.LogError("Ошибка! Неверный тип фишки");
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
