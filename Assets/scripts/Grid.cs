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
     * Заполняет пустые ячейки случайными фишками.
     * 
     * @param chipTypes битовая маска типов фишек, которые присутствуют в уровне
     * Маска имеет вид в двоичном формате 00POYBGR,
     * где позиция буквы соответсвует значению типа фишке в структуре ChipType
     */
    public IEnumerator generateChips(uint chipTypes)
    {
        int i;
        int j;
        List<IntVector2> createdCells = new List<IntVector2>();
        
        for (i = 0; i < _rowCount; i++) {
            for (j = 0; j < _colCount; j++) {
                Cell cell = getCell(i, j);
                
                if (cell != null && cell.chip == null) {
                    Chip chip;
                    Debug.Log("create:" + i + ", " + j);
                    
                    uint ignored = getIgnoredTypes(i, j);
                    uint usingMask = chipTypes & (~ignored);
                    
                    if (usingMask == 0) {
                        usingMask = (int)ChipType.RED;
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
                                usingMask = (int)ChipType.RED;
                            }
                        }
                    }
                    
                    if (usingMask == 0) {
                        usingMask = (int)ChipType.RED;
                        
                        while (Game.blockTime) {
                            //yield WaitForSeconds(1);
                            yield return new WaitForSeconds(1.5f);
                        }
                    }
                    
                    chip = createChipRandom(usingMask, cell.gameObject);
                    
                    cell.chip = chip;
                    
                    createdCells.Add(new IntVector2(j, i));
                    
                    /*while (Game.blockTime) {
                        //yield WaitForSeconds(1);
                        yield return new WaitForSeconds(0.5f);
                    }
                    */
                    Game.blockTime = true;
                }
            }
        }
        
        // Проверка на существование хотя бы одного хода
        if (true) {
            for (i=0; i<3; i++) {
                while (Game.blockTime) {
                    //yield WaitForSeconds(1);
                    yield return new WaitForSeconds(1.5f);
                }
                
                makeOneMove(createdCells, chipTypes);
            }
        }
        
        yield break;
    }
    
    /**
     * Меняет фишки, чтобы на у пользователя был хотя бы один ход.
     */
    private void makeOneMove(List<IntVector2> createdCells, uint chipTypes)
    {
        if (createdCells.Count == 0) {
            Debug.LogError("Ошибка! Нет созданных ячеек для изменения фишки");
        }
        
        List<IntVector2> unCheckedCells = new List<IntVector2>();
        unCheckedCells.AddRange(createdCells.GetRange(0, createdCells.Count));
        
        while (unCheckedCells.Count > 0) {
            int item = Random.Range(0, unCheckedCells.Count - 1);
            int i = unCheckedCells[item].y;
            int j = unCheckedCells[item].x;
            
            uint typeMask = (uint)(1 << (int)_cells[i][j].chip.type);
            List<ChipType> types = getChipTypesFromMask(chipTypes & (~typeMask));
            ChipType type = types[Random.Range(0, types.Count - 1)];
            
            /**
               1 2 3
               4 5 6
               7 8 9
            */
            
            
            List<List<IntVector2>> coordList = new List<int, int>();
            List<int, int> coords = new List<IntVector2>();
            
            coords.Add(new IntVector2(j, i + 1));
            coords.Add(new IntVector2(j - 1, i - 1));
            coords.Add(new IntVector2(j + 1, i - 1));
            coords.Add(new IntVector2(j, i - 2));
            
            coordList.Add(coords);
            
            coords.Add(new IntVector2(j, i - 1));
            coords.Add(new IntVector2(j - 1, i + 1));
            coords.Add(new IntVector2(j + 1, i + 1));
            coords.Add(new IntVector2(j, i + 2));
            
            coordList.Add(coords);
            
            coords.Add(new IntVector2(j - 1, i));
            coords.Add(new IntVector2(j + 1, i - 1));
            coords.Add(new IntVector2(j + 1, i + 1));
            coords.Add(new IntVector2(j + 2, i));
            
            coordList.Add(coords);
            
            coords.Add(new IntVector2(j + 1, i));
            coords.Add(new IntVector2(j - 1, i - 1));
            coords.Add(new IntVector2(j - 1, i + 1));
            coords.Add(new IntVector2(j - 2, i));
            
            coordList.Add(coords);
            
            bool needType;
            
            for (int ii=0; ii < coordList.Count; ii++) {
                coords = coordList[ii];
                
                needType = _cells[i + 1][j] != null && _cells[i + 1][j].chip != null &&
                           _cells[i + 1][j].chip.type == type;
            }
            
            if (i > 0 && i < _rowCount-1) {
                // 5 8 1|3
                needType = _cells[i + 1][j] != null && _cells[i + 1][j].chip != null &&
                                _cells[i + 1][j].chip.type == type;
                
                if ((needType || existInList(createdCells, j, i+1)) &&
                    _cells[i - 1][j] != null && _cells[i - 1][j].canEnter()
                ) {
                    if (!needType) {
                        changeChipType(_cells[i + 1][j], type);
                        
                        if (canMove()) {
                            return;
                        }
                    }
                    
                    if (j > 0 && existInList(createdCells, j - 1, i - 1) &&
                        _cells[i - 1][j - 1].chip.type != type
                    ) {
                        changeChipType(_cells[i - 1][j - 1], type);
                        
                        if (canMove()) {
                            return;
                        }
                    } else
                    if (j < _colCount - 1 && existInList(createdCells, j + 1, i - 1) &&
                        _cells[i + 1][j - 1].chip.type != type
                    ) {
                        changeChipType(_cells[i + 1][j - 1], type);
                        
                        if (canMove()) {
                            return;
                        }
                    } else
                    if (i > 1 && existInList(createdCells, j, i - 2) &&
                        _cells[i - 2][j].chip.type != type
                    ) {
                        changeChipType(_cells[i - 2][j], type);
                        
                        if (canMove()) {
                            return;
                        }
                    }
                }
                
                // 5 2 7|9
                needType = _cells[i + 1][j] != null && _cells[i + 1][j].chip != null &&
                    _cells[i + 1][j].chip.type == type;
                
                if ((needType || existInList(createdCells, j, i+1)) &&
                    _cells[i - 1][j] != null && _cells[i - 1][j].canEnter()
                    ) {
                    if (!needType) {
                        changeChipType(_cells[i + 1][j], type);
                        
                        if (canMove()) {
                            return;
                        }
                    }
                    
                    if (j > 0 && existInList(createdCells, j - 1, i - 1) &&
                        _cells[i - 1][j - 1].chip.type != type
                        ) {
                        changeChipType(_cells[i - 1][j - 1], type);
                        
                        if (canMove()) {
                            return;
                        }
                    } else
                        if (j < _colCount - 1 && existInList(createdCells, j + 1, i - 1) &&
                            _cells[i + 1][j - 1].chip.type != type
                           ) {
                        changeChipType(_cells[i + 1][j - 1], type);
                        
                        if (canMove()) {
                            return;
                        }
                    } else
                        if (i > 1 && existInList(createdCells, j, i - 2) &&
                            _cells[i - 2][j].chip.type != type
                           ) {
                        changeChipType(_cells[i - 2][j], type);
                        
                        if (canMove()) {
                            return;
                        }
                    }
                }
            }
        }
    }
    
    /**
     * Может ли пользователь сделать ход.
     */
    private bool canMove()
    {
        return false;
    }
    
    /**
     * Меняет тип фишки в ячейке.
     * 
     * @param cell изменяемая ячейка
     * @param type нужный тип фишки
     */
    private void changeChipType(Cell cell, ChipType type)
    {
        GameObject parent = cell.chip.transform.parent.gameObject;
        UnityEngine.Object.Destroy(cell.chip);
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
    private uint getIgnoredTypes(int i, int j)
    {
        int res = 0;
        
        if (_cells[i][j] == null) {
            return 0;
        }
        
        if (j > 1 && _cells[i][j-2] != null && _cells[i][j-1] != null &&
            _cells[i][j-2].chip != null && _cells[i][j-1].chip != null &&
            _cells[i][j-2].chip.bonusType != BonusType.SAME_TYPE &&
            _cells[i][j-1].chip.bonusType != BonusType.SAME_TYPE &&
            _cells[i][j-2].chip.type == _cells[i][j-1].chip.type
        ) {
            //Debug.Log("ignore left: " + (int)_cells[i][j-1].chip.type);
            res |= 1 << (int)_cells[i][j-1].chip.type;
        }
        
        if (j < _colCount - 2 && _cells[i][j+2] != null && _cells[i][j+1] != null &&
            _cells[i][j+2].chip != null && _cells[i][j+1].chip != null &&
            _cells[i][j+2].chip.bonusType != BonusType.SAME_TYPE &&
            _cells[i][j+1].chip.bonusType != BonusType.SAME_TYPE &&
            _cells[i][j+2].chip.type == _cells[i][j+1].chip.type
        ) {
            //Debug.Log("ignore right: " + (int)_cells[i][j+1].chip.type);
            res |= 1 << (int)_cells[i][j+1].chip.type;
        }
        
        if (i > 1 && _cells[i-2][j] != null && _cells[i-1][j] != null &&
            _cells[i-2][j].chip != null && _cells[i-1][j].chip != null &&
            _cells[i-2][j].chip.bonusType != BonusType.SAME_TYPE &&
            _cells[i-1][j].chip.bonusType != BonusType.SAME_TYPE &&
            _cells[i-2][j].chip.type == _cells[i-1][j].chip.type
        ) {
            //Debug.Log("ignore top: " + (int)_cells[i-1][j].chip.type);
            res |= 1 << (int)_cells[i-1][j].chip.type;
        }
        
        if (i < _rowCount - 2 && _cells[i+2][j] != null && _cells[i+1][j] != null &&
            _cells[i+2][j].chip != null && _cells[i+1][j].chip != null &&
            _cells[i+2][j].chip.bonusType != BonusType.SAME_TYPE &&
            _cells[i+1][j].chip.bonusType != BonusType.SAME_TYPE &&
            _cells[i+2][j].chip.type == _cells[i+1][j].chip.type
        ) {
            //Debug.Log("ignore bottom: " + (int)_cells[i+1][j].chip.type);
            res |= 1 << (int)_cells[i+1][j].chip.type;
        }
        
        if (j > 0 && j < _colCount - 1 && _cells[i][j-1] != null && _cells[i][j+1] != null &&
            _cells[i][j-1].chip != null && _cells[i][j+1].chip != null &&
            _cells[i][j-1].chip.bonusType != BonusType.SAME_TYPE &&
            _cells[i][j+1].chip.bonusType != BonusType.SAME_TYPE &&
            _cells[i][j-1].chip.type == _cells[i][j+1].chip.type
        ) {
            //Debug.Log("ignore left: " + (int)_cells[i][j-1].chip.type);
            res |= 1 << (int)_cells[i][j-1].chip.type;
        }
        
        if (i > 0 && i < _rowCount - 1 && _cells[i-1][j] != null && _cells[i+1][j] != null &&
            _cells[i-1][j].chip != null && _cells[i+1][j].chip != null &&
            _cells[i-1][j].chip.bonusType != BonusType.SAME_TYPE &&
            _cells[i+1][j].chip.bonusType != BonusType.SAME_TYPE &&
            _cells[i-1][j].chip.type == _cells[i+1][j].chip.type
        ) {
            //Debug.Log("ignore left: " + (int)_cells[i][j-1].chip.type);
            res |= 1 << (int)_cells[i-1][j].chip.type;
        }
        
        return (uint)res;
    }
    
    private Chip createChipRandom(uint chipTypes, GameObject parent)
    {
        List<ChipType> usingTypes = getChipTypesFromMask(chipTypes);
        
        if (usingTypes.Count == 0) {
            return null;
        } else {
            
            return ChipFactory.createNew(usingTypes[Random.Range(0, usingTypes.Count - 1)],
                                         BonusType.NONE, parent);
        }
    }
    
    private List<ChipType> getChipTypesFromMask(uint chipsMask)
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
