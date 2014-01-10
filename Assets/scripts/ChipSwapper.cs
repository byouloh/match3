using UnityEngine;
using System.Collections;

/** Состояние перестановки фишек. */
enum SwapState
{
    /** Ожидание событий от пользователя. */
    MS_READY,
    
    /** Перемещение мыши(выбор двух фишек). */
    MS_FIND_TARGET,
    
    /** Смена фишек. */
    MS_SWAP,
    
    /** Переставляем фишки обратно, в свои начальные позиции. */
    MS_REVERT
}

/**
 * Класс, которые меняет местами две фишки.
 * 
 * @autchor Timur Bogotov timur@e-magic.org
 */
public class ChipSwapper
{
    /** Радиус, на который нужно отодвинуть мышку от начальной нажатой позиции, чтобы переместить фишку. */
    private float MOVE_MOUSE_RADIUS;
    
    /** Скорость перемещения фишек. */
    private float SWAP_SPEED = 0.1f;
    
    /** Состояние перестановки фишек. */
    private SwapState _state;
    
    /** Матрица ячеек. */
    private Grid _grid;
    
    /** Смещение левой верхней фишки относительно экрана. */
    private IntVector2 _offset;
    
    /** Ширина ячейки в пикселях. */
    private int _cellWidth;
    
    /** Высота ячейки в пикселях. */
    private int _cellHeight;
    
    /** Позиция перемещаемой ячейки в пикселях. */
    private Vector3 currentCellPosition;
    
    /** Ячейка перемещаемой фишки. */
    private Cell _currentCell;
    
    /** Ячейка в которую перемещается фишка. */
    private Cell _targetCell;
    
    /**
     * Конструктор.
     * 
     * @param grid матрица ячеек
     * @param leftTopOffset смещение левой верхней фишки относительно экрана
     * @param cellWidth ширина ячейки в пикселях
     * @param cellHeight высота ячейки в пикселях
     */
    public ChipSwapper(Grid grid, IntVector2 leftTopOffset, int cellWidth, int cellHeight)
    {
        MOVE_MOUSE_RADIUS = cellWidth * 0.8f;
        
        this._state      = SwapState.MS_READY;
        this._grid       = grid;
        this._offset     = leftTopOffset;
        this._cellWidth  = cellWidth;
        this._cellHeight = cellHeight;
    }
    
    /**
     * Обновление состояния передвигаемых фишек.
     * 
     * @param deltaTime см. Time.deltaTime
     * 
     * @return Lines массив найденных линий после перемещения фишки, либо null, если нет линий
     */
    public SwapResult step(float deltaTime)
    {
        SwapResult res = new SwapResult();
        res.chipMoved = false;
        
        if (_state == SwapState.MS_READY && Input.GetMouseButtonDown(0)) {
            // Перехват нажатия мыши
            Cell cell = getCellAtCursor((int)Input.mousePosition.x, (int)Input.mousePosition.y);
            
            if (cell != null && cell.canLeave() && cell.chip != null) {
                // попали по ячейке
                _currentCell = cell;
                _state = SwapState.MS_FIND_TARGET;
                currentCellPosition = Camera.main.WorldToScreenPoint(cell.transform.position);
            }
        } else
        if (_state == SwapState.MS_FIND_TARGET) {
            // Ожидание движения курсора в какую-то сторону
            Vector3 offset = Input.mousePosition - currentCellPosition;
            
            if (offset.sqrMagnitude > MOVE_MOUSE_RADIUS * MOVE_MOUSE_RADIUS) {
                IntVector2 dir = getMoveDir(offset);
                
                int i = _currentCell.position.y + dir.y;
                int j = _currentCell.position.x + dir.x;
                
                if (i >= 0 && j >= 0 && i < _grid.getRowCount() && j < _grid.getColCount()) {
                    Cell cell = _grid.getCell(i, j);
                    
                    if (cell != null && cell.canEnter() && cell.chip != null) {
                        _targetCell = cell;
                        _state = SwapState.MS_SWAP;
                        res.chipMoved = true;
                    } else {
                        // Недопустимый ход
                        _currentCell = null;
                        _state = SwapState.MS_READY;
                    }
                }
            }
        } else
        if (_state == SwapState.MS_SWAP) {
            // Перемещение фишек
            _currentCell.chip.transform.position = Vector3.Lerp(_currentCell.chip.transform.position,
                                                                _targetCell.transform.position, SWAP_SPEED);
            
            _targetCell.chip.transform.position  = Vector3.Lerp(_targetCell.chip.transform.position,
                                                                _currentCell.transform.position, SWAP_SPEED);
            
            if (Vector3.SqrMagnitude(_currentCell.chip.transform.position - _targetCell.transform.position) < 0.0005f) {
                _currentCell.chip.transform.position = _targetCell.transform.position;
                _targetCell.chip.transform.position  = _currentCell.transform.position;
                
                Chip t            = _currentCell.chip;
                _currentCell.chip = _targetCell.chip;
                _targetCell.chip  = t;
                
                _currentCell.chip.transform.parent = _currentCell.transform;
                _targetCell.chip.transform.parent  = _targetCell.transform;
                
                MatchDetector matchDetector = new MatchDetector();
                matchDetector.setGrid(_grid);
                matchDetector.findMatches();
                
                if (matchDetector.explosionLines.Count > 0) {
                    _state = SwapState.MS_READY;
                    
                    res.chipMoved   = true;
                    res.currentCell = _currentCell;
                    res.targetCell  = _targetCell;
                    res.lines       = matchDetector.explosionLines;
                    
                    return res;
                } else {
                    _state = SwapState.MS_REVERT;
                }
            }
        } else
        if (_state == SwapState.MS_REVERT) {
            // Перемещение фишек в начальные ячейки
            _currentCell.chip.transform.position = Vector3.Lerp(_currentCell.chip.transform.position,
                                                                _targetCell.transform.position, SWAP_SPEED);
            
            _targetCell.chip.transform.position  = Vector3.Lerp(_targetCell.chip.transform.position,
                                                                _currentCell.transform.position, SWAP_SPEED);
            
            if (Vector3.SqrMagnitude(_currentCell.chip.transform.position - _targetCell.transform.position) < 0.0005f) {
                _currentCell.chip.transform.position = _targetCell.transform.position;
                _targetCell.chip.transform.position  = _currentCell.transform.position;
                
                Chip t            = _currentCell.chip;
                _currentCell.chip = _targetCell.chip;
                _targetCell.chip  = t;
                
                _currentCell.chip.transform.parent = _currentCell.transform;
                _targetCell.chip.transform.parent  = _targetCell.transform;
                
                res.chipMoved = true;
                _state = SwapState.MS_READY;
            }
        }
        
        if (Input.GetMouseButtonUp(0)) {
            if (_state == SwapState.MS_FIND_TARGET) {
                _state = SwapState.MS_READY;
            }
        }
        
        return res;
    }
    
    /**
     * Возвращает ячейку под заданным экранным координатам.
     * 
     * @param x смещение по горизонтали
     * @param y смещение по вертикали
     * 
     * @return Cell ячейка по указанным координатам, либо null, если по заданным координатам нет ячейки
     */
    private Cell getCellAtCursor(int x, int y)
    {
        IntVector2 index = screenToIndex(x, y);
        
        if (index.x >= 0 && index.y >= 0 && index.x < _grid.getColCount() && index.y < _grid.getRowCount()) {
            return _grid.getCell(index.y, index.x);
        } else {
            return null;
        }
    }
    
    /**
     * Преобразует экранные координаты на матричные координаты(номер строки, номер столбца).
     * 
     * @param x смещение по горизонтали
     * @param y смещение по вертикали
     * 
     * @return IntVector2 матричные координаты, где x - номер столбца, y - номер строки
     */
    private IntVector2 screenToIndex(int x, int y)
    {
        int dx = (x - _offset.x) / _cellWidth;
        int dy = (_offset.y - y) / _cellHeight;
        
        return new IntVector2(dx, dy);
    }
    
    /**
     * Возвращает направление передвижения мыши.
     */
    private IntVector2 getMoveDir(Vector3 dir)
    {
        if (Vector3.Angle(dir, Vector3.right) <= 45) {
            return new IntVector2(1, 0);
        } else
        if (Vector3.Angle(dir, Vector3.up) <= 45) {
            return new IntVector2(0, -1);
        } else
        if (Vector3.Angle(dir, Vector3.left) <= 45) {
            return new IntVector2(-1, 0);
        } else {
            return new IntVector2(0, 1);
        }
    }
}
