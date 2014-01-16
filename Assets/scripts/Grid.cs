using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

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
        if (cell == null) {
            throw new System.NullReferenceException("Grid::setCell: cell can not be null");
        }

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
    
    /**
     * Удаляет все фишки и ячейки.
     */
    public void clear()
    {
        for (int i = 0; i < _rowCount; i++) {
            for (int j = 0; j < _colCount; j++) {
                if (_cells[i][j] != null) {
                    if (_cells[i][j].chip != null) {
                        UnityEngine.Object.Destroy(_cells[i][j].chip.gameObject);
                    }
                    
                    UnityEngine.Object.Destroy(_cells[i][j].gameObject);
                }
            }
        }
    }
    
    /**
     * Заполняет пустые ячейки случайными фишками.
     * 
     * @param chipTypes битовая маска типов фишек, которые присутствуют в уровне. 
     * Маска имеет вид в двоичном формате 00POYBGR,
     * где позиция буквы соответсвует значению типа фишке в структуре ChipType
     */
    public void generateChips(uint chipTypes)
    {
        // Массив смещений
        List<ThreeLine> offset          = ThreeLine.getOffsetList();
        List<IntVector2> emptyCells     = getEmptyCells();
        List<IntVector2> unCheckedCells = getEmptyCells();
        
        // Случайный тип фишки для формирования тройки(хода)
        ChipType type = getRandomChipType(chipTypes);
        
        MatchDetector matchDetector = new MatchDetector();
        matchDetector.setGrid(this);
        
        while (unCheckedCells.Count > 0) {
            int itemIndex = Random.Range(0, unCheckedCells.Count);
            int i = unCheckedCells[itemIndex].y;
            int j = unCheckedCells[itemIndex].x;
            
            if (_cells[i][j] == null || _cells[i][j].chip != null || 
                !_cells[i][j].canEnter() || !_cells[i][j].canContainChip()
            ) {
                unCheckedCells.RemoveAt(itemIndex);
            } else {
                for (int ii = 0; ii < offset.Count; ii++) {
                    if (existInList(emptyCells, offset[ii].aj + j, offset[ii].ai + i) &&
                        existInList(emptyCells, offset[ii].bj + j, offset[ii].bi + i) &&
                        existInList(emptyCells, offset[ii].moveJ + j, offset[ii].moveI + i) &&
                       _cells[offset[ii].moveI + i][offset[ii].moveJ + j].canLeave()
                    ) {
                        changeChipType(_cells[offset[ii].ai + i][offset[ii].aj + j], type);
                        changeChipType(_cells[offset[ii].bi + i][offset[ii].bj + j], type);
                        changeChipType(_cells[offset[ii].moveI + i][offset[ii].moveJ + j], type);
                        
                        fillRandomChips(chipTypes);
                        
                        matchDetector.findMatches();
                        
                        if (matchDetector.explosionLines != null && matchDetector.explosionLines.Count == 0) {
                            return;
                        } else {
                            clearTempChips(emptyCells);
                        }
                    }
                }
                
                unCheckedCells.RemoveAt(itemIndex);
            }
        }
        
        // На карте нет возможного хода, заполняем как получается
        Debug.LogWarning("Нет хода!");
        fillRandomChips(chipTypes);
    }
    
    /**
     * Заполняет пустные ячейки случайными фишками.
     * 
     * @param chipTypes битовая маска типов фишек, которые присутствуют в уровне
     */
    private void fillRandomChips(uint chipTypes)
    {
        List<IntVector2> createdCells = new List<IntVector2>();
        int i;
        int j;
        
        for (i = 0; i < _rowCount; i++) {
            for (j = 0; j < _colCount; j++) {
                Cell cell = getCell(i, j);
                
                if (cell != null && cell.chip == null && cell.canContainChip()) {
                    uint ignored = getIgnoredTypes(i, j);
                    uint usingMask = chipTypes & (~ignored);
                    
                    if (usingMask == 0) {
                        usingMask = (uint)getRandomChipType(chipTypes);
                    } else {
                        List<ChipType> usingTypes = getChipTypesFromMask(usingMask);
                        if (usingTypes.Count != 1) {
                            if (j < _colCount - 1 && getCell(i, j + 1) != null && getCell(i, j + 1).chip == null) {
                                uint rightIgnored = getIgnoredTypes(i, j + 1);
                                
                                uint usingRightChipMask = chipTypes & (~rightIgnored);
                                
                                usingTypes = getChipTypesFromMask(usingRightChipMask);
                                
                                if (usingTypes.Count == 1) {
                                    usingMask &= (~usingRightChipMask);
                                }
                            }
                            
                            if (usingMask == 0) {
                                usingMask = (uint)getRandomChipType(chipTypes);
                            }
                        }
                    }
                    
                    if (usingMask == 0) {
                        usingMask = (uint)getRandomChipType(chipTypes);
                    }
                    
                    cell.chip = createChipRandom(usingMask, cell.gameObject);
                    
                    createdCells.Add(new IntVector2(j, i));
                }
            }
        }
    }
    
    /**
     * Удаляет фишки в ячейках в указанных ячейках list.
     * 
     * @param list список координат(индексы ячеек) ячеек
     */
    private void clearTempChips(List<IntVector2> list)
    {
        for (int i = 0; i < list.Count; i++) {
            if (_cells[list[i].y][list[i].x].chip != null) {
                UnityEngine.Object.Destroy(_cells[list[i].y][list[i].x].chip.gameObject);
                _cells[list[i].y][list[i].x].chip = null;
            }
        }
    }
    
    /**
     * Возвращает список пустых ячеек.
     */
    public List<IntVector2> getEmptyCells()
    {
        List<IntVector2> res = new List<IntVector2>();
        
        for (int i = 0; i < _rowCount; i++) {
            for (int j = 0; j < _colCount; j++) {
                if (_cells[i][j].isEmpty() && _cells[i][j].canContainChip()) {
                    res.Add(new IntVector2(j, i));
                }
            }
        }
        
        return res;
    }
    
    /**
     * Может ли пользователь сделать ход.
     */
    private bool canMove()
    {
        // TODO: вставить здесь алгоритм, который сделал Ислам
        return (Random.Range(0, 100) > 90);
    }
    
    /**
     * Меняет тип фишки в ячейке.
     * 
     * @param cell изменяемая ячейка
     * @param type нужный тип фишки
     */
    private void changeChipType(Cell cell, ChipType type)
    {
        GameObject parent;
        
        if (cell.chip == null) {
            parent = cell.gameObject;
        } else {
            parent = cell.chip.transform.parent.gameObject;
            UnityEngine.Object.Destroy(cell.chip.gameObject);
            cell.chip = null;
        }
        
        cell.chip = ChipFactory.createNew(type, BonusType.NONE, parent);
    }
    
    /**
     * Проверяет есть ли в списке векторок IntVector2 элемент с координатами (x,y)
     * 
     * @param list исходным массив векторов
     * @param x координата x нужного элемента
     * @param y координата y нужного элемента
     * 
     * @return bool возвращает true, если элемента найден, иначе false
     */
    private bool existInList(List<IntVector2> list, int x, int y)
    {
        for (int i = 0; i < list.Count; i++) {
            if (list[i].x == x && list[i].y == y) {
                return true;
            }
        }
        
        return false;
    }
    
    /**
     * Возвращает битовую маску недопустимых типов фишек для данной ячейки.
     * 
     * @param i номер строки ячейки, для которого нужно определить маску недопустимых фишек
     * @param i номер столбца ячейки, для которого нужно определить маску недопустимых фишек
     */
    public uint getIgnoredTypes(int i, int j)
    {
        int res = 0;
        
        if (_cells[i][j] == null) {
            return 0;
        }
        
        if (j > 1 && _cells[i][j - 2] != null && _cells[i][j - 1] != null &&
            _cells[i][j - 2].chip != null && _cells[i][j - 1].chip != null &&
            _cells[i][j - 2].chip.compareTo(_cells[i][j - 1].chip)
        ) {
            res |= 1 << (int)_cells[i][j - 1].chip.type;
        }
        
        if (j < _colCount - 2 && _cells[i][j + 2] != null && _cells[i][j + 1] != null &&
            _cells[i][j + 2].chip != null && _cells[i][j + 1].chip != null &&
            _cells[i][j + 2].chip.compareTo(_cells[i][j + 1].chip)
        ) {
            res |= 1 << (int)_cells[i][j + 1].chip.type;
        }
        
        if (i > 1 && _cells[i - 2][j] != null && _cells[i - 1][j] != null &&
            _cells[i - 2][j].chip != null && _cells[i - 1][j].chip != null &&
            _cells[i - 2][j].chip.compareTo(_cells[i - 1][j].chip)
        ) {
            res |= 1 << (int)_cells[i - 1][j].chip.type;
        }
        
        if (i < _rowCount - 2 && _cells[i + 2][j] != null && _cells[i + 1][j] != null &&
            _cells[i + 2][j].chip != null && _cells[i + 1][j].chip != null &&
            _cells[i + 2][j].chip.compareTo(_cells[i + 1][j].chip)
        ) {
            res |= 1 << (int)_cells[i + 1][j].chip.type;
        }
        
        if (j > 0 && j < _colCount - 1 && _cells[i][j - 1] != null && _cells[i][j + 1] != null &&
            _cells[i][j - 1].chip != null && _cells[i][j + 1].chip != null &&
            _cells[i][j - 1].chip.compareTo(_cells[i][j + 1].chip)
        ) {
            res |= 1 << (int)_cells[i][j - 1].chip.type;
        }
        
        if (i > 0 && i < _rowCount - 1 && _cells[i - 1][j] != null && _cells[i + 1][j] != null &&
            _cells[i - 1][j].chip != null && _cells[i + 1][j].chip != null &&
            _cells[i - 1][j].chip.compareTo(_cells[i + 1][j].chip)
        ) {
            res |= 1 << (int)_cells[i - 1][j].chip.type;
        }
        
        return (uint)res;
    }
    
    /**
     * Создает фишку со случайным типом.
     * 
     * @param chipTypes маска используемых фишек
     * @parent контейнер для фишки
     */
    public Chip createChipRandom(uint chipTypes, GameObject parent)
    {
        List<ChipType> usingTypes = getChipTypesFromMask(chipTypes);
        
        if (usingTypes.Count > 0) {
            return ChipFactory.createNew(usingTypes[Random.Range(0, usingTypes.Count)],
                                         BonusType.NONE, parent);
        }
        
        return null;
    }
    
    /**
     * Возвращает случайный тип фишки по заданной маске.
     * 
     * @param chipTypes маска используемых фишек
     * 
     * @return ChipType случайный тип фишки
     */
    private ChipType getRandomChipType(uint chipTypes)
    {
        List<ChipType> usingTypes = getChipTypesFromMask(chipTypes);
        
        return usingTypes[Random.Range(0, usingTypes.Count)];
    }
    
    /**
     * Формирует массив типов фишек из маски.
     * 
     * @param chipsMask маска фишек
     */
    public List<ChipType> getChipTypesFromMask(uint chipsMask)
    {
        List<ChipType> usingTypes = new List<ChipType>();
        
        uint usingMask = chipsMask;
        int bitPos = 0;
        
        while (usingMask > 0) {
            
            if ((usingMask & 1) == 1) {
                usingTypes.Add((ChipType)bitPos);
            }
            
            bitPos++;
            usingMask >>= 1;
        }
        
        return usingTypes;
    }
}
