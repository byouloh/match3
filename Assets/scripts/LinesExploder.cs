using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** 
 * Информация для создания бонусной фишки.
 * 
 * @author Timur Bogotov timur@-emagic.org
 */
public class BonusInfo
{
    /** Ячейка в которую нужно создать бонусную фишку. */
    public Cell cell;
    
    /** Тип фишки. */
    public ChipType cType;
    
    /** Тип бонуса. */
    public BonusType bType;
    
    /**
     * Конструктор.
     * 
     * @param cell ячейка в которую нужно создать бонусную фишку
     * @param chipType тип фишки
     * @param bonusType тип бонуса
     */
    public BonusInfo(Cell cell, ChipType chipType, BonusType bonusType)
    {
        this.cell  = cell;
        this.cType = chipType;
        this.bType = bonusType;
    }
}

/**
 * Класс, который взрывает все найденные линии и создает бонусные фишки.
 * 
 * @author Timur Bogotov timur@e-magic.org
 */
public class LinesExploder
{
    /** Контейнер UI для создания текстовых очков. */
    private GameObject _uiRoot;
    
    /** Префаб всплывающего текста. */
    private GameObject _scorePrefab;
    
    /* Информация о текущей переставляемой фишке. */
    private BonusInfo _currentInfo = null;
    
    /* Информация о текущей переставляемой фишке. */
    private BonusInfo _targetInfo  = null;
    
    /** Информация о перестановке двух фишек. */
    private SwapResult _swapResult;
        
    /**
     * Конструктор.
     * 
     * @param uiRoot контейнер UI для создания текстовых очков
     */
    public LinesExploder(GameObject uiRoot)
    {
        _uiRoot = uiRoot;
        _scorePrefab = Resources.Load<GameObject>("prefabs/scoreLabel");
    }
    
    /**
     * Взрывает все найденные линии и создает бонусные фишки в нужных ячейках.
     * 
     * @param swapResult информация о перестановке двух фишек
     */
    public void start(SwapResult swapResult)
    {
        int i;
        int j;
        
        _currentInfo = null;
        _targetInfo  = null;
        
        _swapResult = swapResult;
        
        if (swapResult.chipMoved) {
            _currentInfo = new BonusInfo(null, swapResult.currentCell.chip.type, swapResult.currentCell.chip.bonusType);
            _targetInfo  = new BonusInfo(null, swapResult.targetCell.chip.type, swapResult.targetCell.chip.bonusType);
        }
        
        List<BonusInfo> bonusChips = getBonusChipsInfo(swapResult);
        
        for (i = 0; i < swapResult.lines.Count; i++) {
            Match line = swapResult.lines[i];
            
            recurciveExplosion(swapResult.lines[i]);
        }
        
        // Создаем бонусные фишки
        for (i = 0; i < bonusChips.Count; i++) {
            if (bonusChips[i].cell.chip == null) {
                Chip chip = ChipFactory.createNew(bonusChips[i].cType, bonusChips[i].bType, bonusChips[i].cell.gameObject);
                bonusChips[i].cell.chip = chip;
            }
        }
    }
    
    /** Рекурсивно взрывает все фишки в линии. */
    private void recurciveExplosion(Match line)
    {
        int i = 0;
        int j;
        
        uint explodePoints = 0;
        
        while (i < line.Count) {
            Match list;
            
            if (line[i] == _swapResult.currentCell) {
                list = line[i].affectCells(_targetInfo);
            } else
            if (line[i] == _swapResult.targetCell) {
                list = line[i].affectCells(_currentInfo);
            } else {
                list = line[i].affectCells(null);
            }
            
            explodePoints += line[i].getExplodePoints();
            
            line[i].explode(null);
            
            if (list != null) {
                for (j = list.Count - 1; j >= 0; j--) {
                    if (line.IndexOf(list[j]) >= 0) {
                        list.RemoveAt(j);
                    }
                }
                
                recurciveExplosion(list);
            }
            
            i++;
        }
        
        if (explodePoints > 0 && line.Count > 0) {
            Vector3 lineCenter = getLineCenter(line);
            
            GameObject scoreLabel = (GameObject)UnityEngine.Object.Instantiate(_scorePrefab);
            scoreLabel.transform.parent   = _uiRoot.transform;
            scoreLabel.transform.position = lineCenter;
            scoreLabel.GetComponent<UILabel>().text = "" + explodePoints;
            
            Game.getInstance().addPoints((int)explodePoints);
        }
        
    }
    
    /**
     * Возвращает информацию о списке создаваемых бонусных фишек.
     * 
     * @param swapResult информация о перестановке фишек
     * 
     * @return List<BonusInfo> информация о списке создаваемых бонусных фишек
     */
    private List<BonusInfo> getBonusChipsInfo(SwapResult swapResult)
    {
        int i;
        int j;
        
        List<BonusInfo> bonusChips = new List<BonusInfo>();
        
        if (swapResult.lines.Count > 0) {
            for (i = 0; i < swapResult.lines.Count; i++) {
                Match line = swapResult.lines[i];
                
                if (line.Count > 0) {
                    BonusType bType = getBonusType(line);
                    
                    if (bType != BonusType.NONE) {
                        ChipType cType  = ChipType.RED;
                        
                        for (j = 0; j < line.Count; j++) {
                            if (line[j].chip != null) {
                                cType = line[j].chip.type;
                                break;
                            }
                        }
                        
                        Cell bonusCell  = null;
                        
                        if (swapResult.chipMoved) {
                            for (j = 0; j < line.Count; j++) {
                                if (line[j] == swapResult.currentCell || line[j] == swapResult.targetCell) {
                                    bonusCell = line[j];
                                }
                            }
                        }
                        
                        if (bonusCell == null) {
                            bonusCell = line[(int)Mathf.Round(line.Count * 0.5f) - 1];
                        }
                        
                        j = 0;
                        while (j < bonusChips.Count) {
                            if (bonusChips[j].cell == bonusCell) {
                                if (bType <= bonusChips[j].bType) {
                                    bType = BonusType.NONE;
                                } else {
                                    bonusChips.RemoveAt(j);
                                }
                                
                                break;
                            } else {
                                j++;
                            }
                        }
                        
                        if (bType != BonusType.NONE) {
                            bonusChips.Add(new BonusInfo(bonusCell, cType, bType));
                        }
                    }
                }
            }
        }
        
        return bonusChips;
    }
    
    /**
     * Возвращает центр линии(списка ячеек).
     * 
     * @param line линия из фишек
     * 
     * @return Vector3 центр линии
     */
    private Vector3 getLineCenter(Match line)
    {
        Vector3 ltPos = new Vector3(line[0].transform.position.x, line[0].transform.position.y, 0);
        Vector3 rbPos = new Vector3(ltPos.x, ltPos.y, 0);
        
        for (int i = 0; i < line.Count; i++) {
            if (line[i].transform.position.x < ltPos.x) {
                ltPos.x = line[i].transform.position.x;
            } else
            if (line[i].transform.position.x > rbPos.x) {
                rbPos.x = line[i].transform.position.x;
            }
            
            if (line[i].transform.position.y < ltPos.y) {
                ltPos.y = line[i].transform.position.y;
            } else
            if (line[i].transform.position.y > rbPos.y) {
                rbPos.y = line[i].transform.position.y;
            }
        }
        
        ltPos.x = (ltPos.x + rbPos.x) / 2;
        ltPos.y = (ltPos.y + rbPos.y) / 2;
        
        return ltPos;
    }
    
    /**
     * Возвращает тип бонуса, который должен образоваться после уничтожения линии.
     * 
     * @param line уничтожаемая линия
     * 
     * @return BonusType тип бонуса, который должен образоваться после уничтожения линии.
     */
    private BonusType getBonusType(Match line)
    {
        if (line.Count < 4) {
            return BonusType.NONE;
        } else
        if (line.Count > 4) {
            return BonusType.SAME_TYPE;
        } else
        if (line[0].position.x == line[1].position.x) {
            return BonusType.HORIZONTAL_STRIP;
        } else {
            return BonusType.VERTICAL_STRIP;
        }
        
    }
}
