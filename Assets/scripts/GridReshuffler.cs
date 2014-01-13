using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** Состояние перемешивания. */
enum MixState
{
    /** Ничего не происходит. */
    MS_NONE,
    
    /** Все фишки собираются в центре. */
    MS_MIXING,
    
    /** Все фишки раскидываются на свои места. */
    MS_SPREADING
}

/**
 * Класс, которорый перемешает фишки на уровне.
 * 
 * @author Timur Bogotov timur@e-magic.org
 */
public class GridReshuffler
{
    /** Скорость передвижения фишек при перетасовке. */
    private const float MOVE_SPEED = 10.0f;
    
    /** Матрица ячеек текущего уровня. */
    private Grid _grid;
    
    /** Массив перемешиваемых фишек. */
    private List<Chip> _chips;
    
    /** Массив перемешиваемых бонусных фишек. */
    private List<Chip> _bonuses;
    
    /** Состояние перемешивания. */
    private MixState _state;
    
    /** Точка на экране, куда сходятся все фишки. */
    private Vector3 _centerPoint;
    
    /** Битовая маска используемых типов фишек в текущем уровне. */
    private uint _usingTypes;
    
    /**
     * Инициализирует алгоритм перетасовки.
     * 
     * @param grid матрица ячеек
     * @param usingTypes битовая маска используемых фишек
     * @param centerPoint точка на экране, куда будут сходится все фишки
     */
    public void start(Grid grid, uint usingTypes, Vector3 centerPoint)
    {
        this._grid        = grid;
        this._state       = MixState.MS_NONE;
        this._centerPoint = centerPoint;
        this._usingTypes  = usingTypes;
        
        _chips   = new List<Chip>();
        _bonuses = new List<Chip>();
        
        for (int i = 0; i < _grid.getRowCount(); i++) {
            for (int j = 0; j < _grid.getColCount(); j++) {
                Cell cell = _grid.getCell(i, j);
                
                if (cell != null && cell.chip != null && cell.canLeave()) {
                    if (cell.chip.bonusType == BonusType.NONE) {
                        _chips.Add(cell.chip);
                    } else {
                        _bonuses.Add(cell.chip);
                    }
                }
            }
        }
        
        _state = MixState.MS_MIXING;
    }
    
    /**
     * Обновление позиции для каждой фишки.
     * 
     * @param deltaTime см. Time.deltaTime
     * 
     * @return возвращает состояние выполнения перемешивания:
     * true - перемешивание завершено, false - в процессе перемешивания
     */
    public bool step(float deltaTime)
    {
        int i;
        int k;
        Vector3 dir;
        
        float speed    = MOVE_SPEED * deltaTime;
        float sqrSpeed = speed * speed;
        
        if (_state == MixState.MS_MIXING) {
            k = 0;
            
            for (i = 0; i < _chips.Count; i++) {
                if (Vector3.SqrMagnitude(_chips[i].transform.position - _centerPoint) < sqrSpeed) {
                    _chips[i].transform.position = _centerPoint;
                    k++;
                } else {
                    dir = _centerPoint - _chips[i].transform.position;
                    _chips[i].transform.position += dir.normalized * speed;
                }
            }
            
            for (i = 0; i < _bonuses.Count; i++) {
                if (Vector3.SqrMagnitude(_bonuses[i].transform.position - _centerPoint) < sqrSpeed) {
                    _bonuses[i].transform.position = _centerPoint;
                    k++;
                } else {
                    dir = _centerPoint - _bonuses[i].transform.position;
                    _bonuses[i].transform.position += dir.normalized * speed;
                }
            }
            
            if (k == _chips.Count + _bonuses.Count) {
                generateChips();
                _state = MixState.MS_SPREADING;
            }
        } else if (_state == MixState.MS_SPREADING) {
            k = 0;
            
            for (i = 0; i < _chips.Count; i++) {
                if (_chips[i].transform.localPosition.sqrMagnitude < sqrSpeed) {
                    _chips[i].transform.localPosition = Vector3.zero;
                    k++;
                } else {
                    _chips[i].transform.localPosition -= _chips[i].transform.localPosition.normalized * speed;
                }
            }
            
            if (k == _chips.Count) {
                _state = MixState.MS_NONE;
                return true;
            }
        }
        
        return false;
    }
    
    /**
     * Раскидывает бонусные фишки в случайные ячейки и заполняет пустые ячейчи случайными фишками.
     */
    private void generateChips()
    {
        int i;
        int j;
        int k;
        int itemIndex;
        
        for (i = 0; i < _grid.getRowCount(); i++) {
            for (j = 0; j < _grid.getColCount(); j++) {
                // Удаляем все обычные фишки
                for (k = 0; k < _chips.Count; k++) {
                    Cell cell = _grid.getCell(i, j);
                    
                    if (cell != null && cell.chip == _chips[k]) {
                        UnityEngine.Object.Destroy(_chips[k].gameObject);
                        cell.chip = null;
                    }
                }
                
                // Удаляем ссылки на бонусные фишки
                for (k = 0; k < _bonuses.Count; k++) {
                    Cell cell = _grid.getCell(i, j);
                    
                    if (cell != null && cell.chip == _bonuses[k]) {
                        cell.chip = null;
                    }
                }
            }
        }
        
        _chips.Clear();
        
        List<IntVector2> emptyCells     = _grid.getEmptyCells();
        List<IntVector2> uncheckedCells = _grid.getEmptyCells();
        
        // Раскидываем бонусные фишки в случайные ячейки
        while (uncheckedCells.Count > 0 && _bonuses.Count > 0) {
            itemIndex = Random.Range(0, uncheckedCells.Count);
            
            i = uncheckedCells[itemIndex].y;
            j = uncheckedCells[itemIndex].x;
            
            uint ignored   = _grid.getIgnoredTypes(i, j);
            uint usingMask = _usingTypes & (~ignored);
            
            List<ChipType> usingTypes = _grid.getChipTypesFromMask(usingMask);
            
            if (usingTypes.Count == 0) {
                uncheckedCells.RemoveAt(itemIndex);
            } else {
                for (k = 0; k < _bonuses.Count; k++) {
                    uint m = (uint)(1 << (int)_bonuses[k].type);
                    
                    if (_bonuses[k].bonusType == BonusType.SAME_TYPE || (usingMask & m) == m) {
                        _grid.getCell(i, j).chip = _bonuses[k];
                        _bonuses[k].transform.parent = _grid.getCell(i, j).gameObject.transform;
                        _bonuses.RemoveAt(k);
                        break;
                    }
                }
                
                uncheckedCells.RemoveAt(itemIndex);
            }
        }
        
        // Заполнение пустых ячеек
        _grid.generateChips(_usingTypes);
        
        // Обновление списка перемешиваемых фишек
        for (i = 0; i < emptyCells.Count; i++) {
            Chip chip = _grid.getCell(emptyCells[i].y, emptyCells[i].x).chip;
            
            if (chip == null) {
                Debug.LogError("Ошибка! В ячейке нет фишки");
            } else {
                _chips.Add(chip);
                chip.transform.position = _centerPoint;
            }
        }
    }
    
    /**
     * Происходит ли сейчас перетасовка.
     * 
     * @return bool если перетасовка включена возвращает true, иначе false
     */
    public bool isShuffle()
    {
        return _state != MixState.MS_NONE;
    }
}
