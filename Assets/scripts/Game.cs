using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Основной класс игры.
 * 
 * @author Timur Bogotov timur@e-magic.org
 */
public class Game: MonoBehaviour
{
    /** Время ожидания подсказски. */
    const int SHOW_HELP_INTERVAL = 4;
    
    /** Интервал между ходом и показом подсказски. */
    const int HELP_TIMEOUT = 13;
    
    /** Экземпляр класса. */
    private static Game _instance = null;
    
	/** Контейнер для ячеек. */
	public GameObject cellsRoot;
    
    /** Контейнер для UI. */
    public GameObject uiRoot;
    
	/** Шаблон ячейки. */
	public GameObject cellPrefab;
    
    /** Текстовая метка для отображения количества очков. */
    public UILabel scoreLabel;
    
    /** Текстовая метка для отображения текущего уровня. */
    public UILabel levelLabel;
    
    /** Текстовая метка для отображения количества ходов. */
    public UILabel movesLabel;
    
	/** Матрица ячеек. */
	private Grid _grid;
    
    /** Информация об уровне. */
    private Level level;

    /**
     * Нахождение подсказки, поиск взрывающихся линий.
     */
    private MatchDetector _matchDetector;

    /**
     * Массив фишек, являющиеся подсказской.
     */
    private Match _helpMatch;

    /**
     * Время последнего хода.
     */
    private float _strokeTime;

    /**
     * Время последнего показа подсказки.  
     */
    private float _lastHelpTime;

    /** Класс, которорый перемешает фишки на уровне. */
    private GridReshuffler _gridReshuffler;
    
    /** Класс, которые переставляет две фишки. */
    private ChipSwapper _chipSwapper;
    
    /** Класс, который взрывает фишки. */
    private LinesExploder _linesExploder;
    
    /**
     * Возвращает экземпляр класса.
     * 
     * Следит за тем, чтобы экземпляр класса был один во всем проекте.
     * 
     * @return Game экземпляр класса
     */
    public static Game getInstance()
    {
        return _instance;
    }
    
    /** Инициализация. */
	private void Start()
	{
        _instance = this;
        
        _gridReshuffler = new GridReshuffler();
        _linesExploder  = new LinesExploder(uiRoot);
        
        loadLevel(1);
        _grid.generateChips(level.chipTypes);
        _matchDetector = new MatchDetector();
        _matchDetector.setGrid(_grid);
        _strokeTime = Time.time;
        _lastHelpTime = 0;
        
        Vector3 offset = Camera.main.WorldToScreenPoint(cellsRoot.transform.position + new Vector3(-Grid.CELL_WIDTH * 0.5f, Grid.CELL_HEIGHT * 0.5f, 0));
        Vector3 cellSize = offset - Camera.main.WorldToScreenPoint(cellsRoot.transform.position + new Vector3(Grid.CELL_WIDTH * 0.5f, -Grid.CELL_HEIGHT * 0.5f, 0));
        
        _chipSwapper = new ChipSwapper(_grid, new IntVector2((int)offset.x, (int)offset.y), (int)Mathf.Abs(cellSize.x), (int)Mathf.Abs(cellSize.y));
    }
    
    /**
     * Обработка текущего кадра(состояния).
     */
	private void Update()
	{
		if (Input.GetKey("p")) {
            remixGrid();
        }
        
        if (_gridReshuffler.isShuffle()) {
            if (_gridReshuffler.step(Time.deltaTime)) {
                //Debug.Log("Mix Complete");
            }
        } else {
            SwapResult swapResult = _chipSwapper.step(Time.deltaTime);
            
            if (swapResult.chipMoved) {
                _lastHelpTime = Time.time;
                
                if (swapResult.lines != null) {
                    onMoved();
                    _linesExploder.start(swapResult);
                }
            }
        }
        
        if (((Time.time - _strokeTime) > HELP_TIMEOUT) && ((Time.time - _lastHelpTime) > SHOW_HELP_INTERVAL)) {
            if (_helpMatch != null) {
                for (int i = 0; i < _helpMatch.Count; i++) {
                    _helpMatch[i].chip.GetComponent<Animator>().SetTrigger("flicker");
                }
                
                _lastHelpTime = Time.time;
            } else {
                //Debug.LogError ("Линий нет"); // TODO убрать 
            }
        }
    }
    
    /**
     * Прорисовка элементов GUI.
     */
    void OnGUI()
	{
        if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 200, 100, 40), "сделали ход")) {
            // Запуск поиска подсказки
            _strokeTime = Time.time;
            _helpMatch  = _matchDetector.findHelpMatch();
            
            if (_helpMatch == null) {
                Debug.LogError("Линий нет надо вызвать перетасовку"); // TODO убрать
            } 
        }
	}
  
    /**
     * Загружает уровень.
     * 
     * @param levelId номер уровня
     */
    private void loadLevel(int levelId)
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
        
        this.level.remainingMoves = this.level.maxMoves;
        this.level.points = 0;
        
        scoreLabel.text = "Score: 0";
        movesLabel.text = "Moves: " + this.level.remainingMoves;
        levelLabel.text = "Level "  + levelId;
        
        int i;
        int j;
        
        int rowCount = info.asInt("rows");
        int colCount = info.asInt("cols");
        
        string cellsString = info.asString("cells");
        
        // Загружаем ячейки
        _grid = new Grid(rowCount, colCount);
        
        string[] cellItems = cellsString.Split(',');
        
        if (cellItems.Length != _grid.getRowCount() * _grid.getColCount()) {
            Debug.LogError("Ошибка! Неправильные данные загружаемого уровня!");
            return;
        }
        
        int ii = 0;
        
        for (i = 0; i < _grid.getRowCount(); i++) {
            for (j = 0; j < _grid.getColCount(); j++) {
                int cellInfo = int.Parse(cellItems[ii++]);
                
                if (cellInfo == 0) {
                    _grid.setCell(i, j, null);
                } else {
                    GameObject cell = (GameObject)UnityEngine.Object.Instantiate(cellPrefab);
                    cell.transform.parent   = cellsRoot.transform;
                    cell.transform.localPosition = new Vector3(Grid.CELL_WIDTH * j, -Grid.CELL_HEIGHT * i, 0);
                    
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
                            Debug.LogError("Ошибка! Неверный тип ячейки. Номер ячейки : i = " + i + ", j = " + j );
                            blocker = BlockFactory.createNew(BlockerType.NONE, c.gameObject);
                            break;
                    }
                    
                    Chip chip = null;
                    
                    if (chipType > 0) {
                        try {
                            chip = ChipFactory.createNew((ChipType)(chipType - 1), (BonusType)bonusType, c.gameObject);
                        } catch (System.Exception e) {
                            Debug.LogError("Ошибка! Неверный тип фишки");
                        }
                    }
                    
                    c.initialize(blocker, chip, new IntVector2(j, i));
                    
                    _grid.setCell(i, j, c);
                }
            }
        }
        
        // Отцентровываем контейнер для ячеек
        cellsRoot.transform.position = new Vector3(Grid.CELL_WIDTH*0.5f - Grid.CELL_WIDTH * _grid.getColCount()*0.5f,
                                                   -Grid.CELL_HEIGHT*0.5f + Grid.CELL_HEIGHT * _grid.getRowCount()*0.5f, 0);
    }
    
    /**
     * Начинает процесс перетасовки фишек.
     */
    private void remixGrid()
    {
        if (!_gridReshuffler.isShuffle()) {
            _gridReshuffler.start(_grid, level.chipTypes, new Vector3(0, 0, 0));
        }
    }
    
    /**
     * Добавляет очки за взрыв фишек.
     * 
     * @param количество очков
     */
    public void addPoints(int pointsCount)
    {
        this.level.points += pointsCount;
        scoreLabel.text = "Score: " + this.level.points;
    }
    
    /** Вызывается когда пользователь сделал ход. */
    private void onMoved()
    {
        this.level.remainingMoves--;
        movesLabel.text = "Moves: " + this.level.remainingMoves;
    }
}
