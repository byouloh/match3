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
    /**
     * Координата z маски ячейки.
     */
    public const float CELL_MASK_Z_INDEX = -0.2F;

    /**
     * Координата z всех видимых в любом случае элементов.
     */
    public const float TOP_Z_INDEX = -0.3F;


    /** Время ожидания подсказски. */
    const int SHOW_HELP_INTERVAL = 2;
    
    /** Интервал между ходом и показом подсказски. */
    const int HELP_TIMEOUT = 3;
    
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

    /** Класс ответственный за падение фишек. */
    private FallingManager _fallingManager;
    
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

        _strokeTime   = Time.time;
        _lastHelpTime = 0;

        _fallingManager = new FallingManager();
        
        Vector3 offset   = Camera.main.WorldToScreenPoint(cellsRoot.transform.position + new Vector3(-Grid.CELL_WIDTH * 0.5f, Grid.CELL_HEIGHT * 0.5f, 0));
        Vector3 cellSize = offset - Camera.main.WorldToScreenPoint(cellsRoot.transform.position + new Vector3(Grid.CELL_WIDTH * 0.5f, -Grid.CELL_HEIGHT * 0.5f, 0));
        
        _chipSwapper = new ChipSwapper(_grid, new IntVector2((int)offset.x, (int)offset.y), (int)Mathf.Abs(cellSize.x), (int)Mathf.Abs(cellSize.y));

        _helpMatch = new Match();
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
                _findHelpMatch();
            }
        } else if (_fallingManager.isStarting()) {
            _fallingManager.step(Time.deltaTime);
        } else {
            SwapResult swapResult = _chipSwapper.step(Time.deltaTime);
            
            if (swapResult.chipMoved) {
                _strokeTime = Time.time;
                
                if (swapResult.lines != null) {
                    onMoved();

                    _helpMatch.Clear();

                    _linesExploder.start(swapResult);

                    // Вызываем падение фишек
                    _fallingManager.start(_grid, _onFallingComplete);
                }
            }
        }

        if (_helpMatch.Count > 0 && 
            ((Time.time - _strokeTime) > HELP_TIMEOUT) && 
            ((Time.time - _lastHelpTime) > SHOW_HELP_INTERVAL)
        ) {
            for (int i = 0; i < _helpMatch.Count; i++) {
                _helpMatch[i].chip.GetComponent<Animator>().SetTrigger("flicker");
                _helpMatch[i].chip.GetComponent<Animator>().speed = 1.0F;
            }
            
            _lastHelpTime = Time.time;
        }
    }

    /**
     * Событие по завершению заполнения пустых клеток после взрыва.
     */
    private void _onFallingComplete()
    {
        _matchDetector.findMatches();
        
        if (_matchDetector.explosionLines.Count > 0) {
            SwapResult swapResult = new SwapResult();

            swapResult.chipMoved = false;
            swapResult.lines     = _matchDetector.explosionLines;

            _linesExploder.start(swapResult);

            // Вызываем падение фишек
            _helpMatch.Clear();
            _fallingManager.start(_grid, _onFallingComplete);
        } else {
            _findHelpMatch();
        }
    }

    /**
     * Прорисовка элементов GUI.
     */
    void OnGUI()
	{

	}

    /**
     * Запускает поиск подсказки(поиск возможного хода).
     */
    private void _findHelpMatch()
    {
        _strokeTime   = Time.time;
        _lastHelpTime = 0;
        _helpMatch    = _matchDetector.findHelpMatch();

        if (_helpMatch.Count <= 0) {
            remixGrid();
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
        int k;
        
        int rowCount = info.asInt("rows");
        int colCount = info.asInt("cols");
        
        string cellsString = info.asString("cells");
        
        // Загружаем ячейки
        _grid = new Grid(rowCount, colCount);
        
        string[] cellItems = cellsString.Split(';');

        if (cellItems.Length != _grid.getRowCount() * _grid.getColCount()) {
            Debug.LogError("Ошибка! Неправильные данные загружаемого уровня! [" + cellItems.Length + "]");
            return;
        }

        int itemIndex = 0;
        
        for (i = 0; i < _grid.getRowCount(); i++) {
            for (j = 0; j < _grid.getColCount(); j++) {
                string[] cellItem = cellItems[itemIndex++].Split(':');

                if (cellItem.Length <= 0) {
                    Debug.LogError("Ошибка при загрузке уровня! cellItems.Length <= 0");
                    return;
                }

                int cellInfo = 0;

                try {
                    cellInfo = int.Parse(cellItem[0]);
                } catch (System.Exception e) {
                    Debug.LogError("Ошибка при загрузке уровня!" + e.Message);
                    return;
                }

                int cellType  = cellInfo & 0xF;
                int chipType  = (cellInfo >> 4) & 0xF;
                int bonusType = (cellInfo >> 8) & 0xFF;

                Cell cell = null;

                try {
                    cell = CellFactory.createNew((CellType)cellType, _grid, level.chipTypes, 
                                                 new IntVector2(i, j), cellsRoot);
                } catch (System.Exception e) {
                    Debug.LogError("Ошибка при загрузке уровня! Ошибка при создании ячейки[" 
                                   + i + "," + j + "]: " + e.Message);
                }

                cell.transform.localPosition = new Vector3(Grid.CELL_WIDTH * j, -Grid.CELL_HEIGHT * i, 0);

                // Добавление блокирующих элементов
                if (cellItem.Length > 1) {
                    string[] blockerItems = cellItem[1].Split(',');

                    for (k = 0; k < blockerItems.Length; k++) {
                        int blockerType = 0;

                        try {
                            blockerType = int.Parse(blockerItems[k]);
                        } catch (System.Exception e) {
                            Debug.LogError("Ошибка при загрузке уровня! " + e.Message);
                            return;
                        }

                        if ((BlockerType)blockerType != BlockerType.NONE) {
                            CellBlocker blocker = BlockerFactory.createNew((BlockerType)blockerType, cell.gameObject);

                            if (blocker != null) {
                                cell.addBlocker(blocker);
                            } else {
                                Debug.LogError("Ошибка! Неверный тип блокирующего элемента. [" + i + "," + j + "]");
                            }
                        }
                    }
                }

                // Добавление фишки
                Chip chip = null;

                try {
                    if (chipType > 0) {
                        chip = ChipFactory.createNew((ChipType)(chipType), (BonusType)bonusType, cell.gameObject);
                    }
                } catch (System.Exception e) {
                    Debug.LogError("Ошибка! Неверный тип фишки: " + e.Message);
                }

                if (chip != null) {
                    Debug.LogError("chip != null");
                    cell.setChip(chip);
                } else {
                    //Debug.LogError("Ошибка! Неверный тип фишки: [" + i + "," + j + "]: " + e.Message);
                }

                _grid.setCell(i, j, cell);
            }
        }
        
        // Отцентровываем контейнер для ячеек
        cellsRoot.transform.position = new Vector3(Grid.CELL_WIDTH * 0.5f - Grid.CELL_WIDTH * _grid.getColCount() * 0.5f,
                                                   -Grid.CELL_HEIGHT * 0.5f + Grid.CELL_HEIGHT * _grid.getRowCount() * 0.5f, 
                                                   0);
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
        // Перерасчет очков.
        this.level.remainingMoves--;
        movesLabel.text = "Moves: " + this.level.remainingMoves;
    }

    /**
     * 
     */
    public void onResize()
    {
        Vector3 offset   = Camera.main.WorldToScreenPoint(cellsRoot.transform.position + new Vector3(-Grid.CELL_WIDTH * 0.5f, Grid.CELL_HEIGHT * 0.5f, 0));
        Vector3 cellSize = offset - Camera.main.WorldToScreenPoint(cellsRoot.transform.position + new Vector3(Grid.CELL_WIDTH * 0.5f, -Grid.CELL_HEIGHT * 0.5f, 0));
        
        _chipSwapper.changeSize(new IntVector2((int)offset.x, (int)offset.y), (int)Mathf.Abs(cellSize.x), (int)Mathf.Abs(cellSize.y));
    }
}
