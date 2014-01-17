using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Генератор обводки для ячеек.
 * 
 * @author Timur Bogotov timur@e-magic.org
 */
public class CellEdgeGenerator
{
    /** Матрица ячеек. */
    private Grid _grid;
    
    /** Контейнер для элементов обводки. */
    private GameObject _edgeRoot;
    
    /** Шаблон(префаб) для элемента обводки. */
    private GameObject _edgePrefab;
    
    /**
     * Название внешней части текстуры обводки.
     * В ресурсах должны лежать текстуры с названиями {prefix + "_" + i}, где i=[0..8].
     */
    private string _outerPrefix;
    
    /**
     * Название внутренней части текстуры обводки.
     * В ресурсах должны лежать текстуры с названиями {prefix + "_" + i}, где i=[0..8].
     */
    private string _innerPrefix;
    
    /** Список спрайтов внешней обводки. */
    private List<Sprite> _outer;
    
    /** Список спрайтов внутренней обводки. */
    private List<Sprite> _inner;
    
    /**
     * Конструктор.
     * 
     * @param grid матрица ячеек
     * @param edgeRoot контейнер для элементов обводки
     * @param edgePrefab шаблон(префаб) для элемента обводки
     * @param edgeOuterPrefix название внешней части текстуры обводки.
     *        В ресурсах должны лежать текстуры с названиями {prefix + "_" + i}, где i=[0..8]
     * @param edgeInnerPrefix название внутренней части текстуры обводки.
     *        В ресурсах должны лежать текстуры с названиями {prefix + "_" + i}, где i=[0..8]
     */
    public CellEdgeGenerator(Grid grid, GameObject edgeRoot, GameObject edgePrefab,
                             string edgeOuterPrefix, string edgeInnerPrefix)
    {
        this._grid        = grid;
        this._edgeRoot    = edgeRoot;
        this._edgePrefab  = edgePrefab;
        this._outerPrefix = edgeOuterPrefix;
        this._innerPrefix = edgeInnerPrefix;
    }
    
    /**
     * Генерирует обводку для ячеек.
     */
    public void generate()
    {
        clear();
        
        int i;
        int j;
        
        _outer = new List<Sprite>();
        _inner = new List<Sprite>();
        
        Sprite[] innerTextures = Resources.LoadAll<Sprite>("textures/edgeInner");
        Sprite[] outerTextures = Resources.LoadAll<Sprite>("textures/edgeOuter");
        
        for (i = 0; i < 9; i++) {
            _outer.Add(outerTextures[i]);
            _inner.Add(innerTextures[i]);
        }
        
        List<Sprite> ltEdges = new List<Sprite>();
        ltEdges.Add(_outer[0]);
        ltEdges.Add(_outer[3]);
        ltEdges.Add(_outer[1]);
        ltEdges.Add(_inner[8]);
        
        ltEdges.Add(_outer[4]);
        ltEdges.Add(_outer[4]);
        ltEdges.Add(_outer[4]);
        ltEdges.Add(_outer[4]);
        
        List<Sprite> rtEdges = new List<Sprite>();
        rtEdges.Add(_outer[2]);
        rtEdges.Add(_outer[5]);
        rtEdges.Add(_outer[1]);
        rtEdges.Add(_inner[6]);
        
        rtEdges.Add(_outer[4]);
        rtEdges.Add(_outer[4]);
        rtEdges.Add(_outer[4]);
        rtEdges.Add(_outer[4]);
        
        List<Sprite> lbEdges = new List<Sprite>();
        lbEdges.Add(_outer[6]);
        lbEdges.Add(_outer[3]);
        lbEdges.Add(_outer[7]);
        lbEdges.Add(_inner[2]);
        
        lbEdges.Add(_outer[4]);
        lbEdges.Add(_outer[4]);
        lbEdges.Add(_outer[4]);
        lbEdges.Add(_outer[4]);
        
        List<Sprite> rbEdges = new List<Sprite>();
        rbEdges.Add(_outer[8]);
        rbEdges.Add(_outer[5]);
        rbEdges.Add(_outer[7]);
        rbEdges.Add(_inner[0]);
        
        rbEdges.Add(_outer[4]);
        rbEdges.Add(_outer[4]);
        rbEdges.Add(_outer[4]);
        rbEdges.Add(_outer[4]);
        
        for (i = 0; i < _grid.getRowCount(); i++) {
            for (j = 0; j < _grid.getColCount(); j++) {
                Cell cell = _grid.getCell(i, j);
                
                if (cell.canContainChip()) {
                    Vector3 pos = cell.transform.position;
                    
                    createEdge(ltEdges[(int)getLTMask(i, j)],
                               pos + new Vector3(-Grid.CELL_WIDTH * 0.25f, Grid.CELL_HEIGHT * 0.25f, -0.5f));
                    
                    createEdge(rtEdges[(int)getRTMask(i, j)],
                               pos + new Vector3(Grid.CELL_WIDTH * 0.25f, Grid.CELL_HEIGHT * 0.25f, -0.5f));
                    
                    createEdge(lbEdges[(int)getLBMask(i, j)],
                               pos + new Vector3(-Grid.CELL_WIDTH * 0.25f, -Grid.CELL_HEIGHT * 0.25f, -0.5f));
                    
                    createEdge(rbEdges[(int)getRBMask(i, j)],
                               pos + new Vector3(Grid.CELL_WIDTH * 0.25f, -Grid.CELL_HEIGHT * 0.25f, -0.5f));
                }
            }
        }
    }
    
    /**
     * Создает спрайт по заданным координатам.
     * 
     * @param sprite шаблон спрайта, который нужно создать
     * @param pos позиция объекта
     */
    private void createEdge(Sprite sprite, Vector3 pos)
    {
        GameObject edge = (GameObject)UnityEngine.Object.Instantiate(_edgePrefab);
        edge.GetComponent<SpriteRenderer>().sprite = sprite;
        
        edge.transform.parent   = _edgeRoot.transform;
        edge.transform.position = pos;
        
        if (sprite == _inner[0] || sprite == _inner[2] || sprite == _inner[6] || sprite == _inner[8]) {
            createEdge(_outer[4], pos);
        }
    }
    
    /**
     * Возвращает порядковый номер элемента массива в виде маски для левого верхнего угла ячейки.
     * 
     * @param i номер строки ячейки
     * @param j номер столбца ячейки
     * 
     * @return uint маска для левого верхнего угла ячейки
     */
    private uint getLTMask(int i, int j)
    {
        uint mask = 0;
        
        if (!isEmpty(i - 1, j)) {
            mask |= 1;
        }
        
        if (!isEmpty(i, j - 1)) {
            mask |= 1 << 1;
        }
        
        if (!isEmpty(i - 1, j - 1)) {
            mask |= 1 << 2;
        }
        
        return mask;
    }
    
    /**
     * Возвращает порядковый номер элемента массива в виде маски для правого верхнего угла ячейки.
     * 
     * @param i номер строки ячейки
     * @param j номер столбца ячейки
     * 
     * @return uint маска для правого верхнего угла ячейки
     */
    private uint getRTMask(int i, int j)
    {
        uint mask = 0;
        
        if (!isEmpty(i - 1, j)) {
            mask |= 1;
        }
        
        if (!isEmpty(i, j + 1)) {
            mask |= 1 << 1;
        }
        
        if (!isEmpty(i - 1, j + 1)) {
            mask |= 1 << 2;
        }
        
        return mask;
    }
    
    /**
     * Возвращает порядковый номер элемента массива в виде маски для левого нижнего угла ячейки.
     * 
     * @param i номер строки ячейки
     * @param j номер столбца ячейки
     * 
     * @return uint маска для левого нижнего угла ячейки
     */
    private uint getLBMask(int i, int j)
    {
        uint mask = 0;
        
        if (!isEmpty(i + 1, j)) {
            mask |= 1;
        }
        
        if (!isEmpty(i, j - 1)) {
            mask |= 1 << 1;
        }
        
        if (!isEmpty(i + 1, j - 1)) {
            mask |= 1 << 2;
        }
        
        return mask;
    }
    
    /**
     * Возвращает порядковый номер элемента массива в виде маски для правого нижнего угла ячейки.
     * 
     * @param i номер строки ячейки
     * @param j номер столбца ячейки
     * 
     * @return uint маска для правого нижнего угла ячейки
     */
    private uint getRBMask(int i, int j)
    {
        uint mask = 0;
        
        if (!isEmpty(i + 1, j)) {
            mask |= 1;
        }
        
        if (!isEmpty(i, j + 1)) {
            mask |= 1 << 1;
        }
        
        if (!isEmpty(i + 1, j + 1)) {
            mask |= 1 << 2;
        }
        
        return mask;
    }
    
    /** Проверяет, есть ли ячейка по заданным индексам матрицы */
    private bool isEmpty(int i, int j)
    {
        return (i < 0 || j < 0 || i >= _grid.getRowCount() || j >= _grid.getColCount() ||
                !_grid.getCell(i, j).canContainChip());
    }
    
    /**
     * Удаляет все элементы обводки.
     */
    public void clear()
    {
        while (_edgeRoot.transform.childCount > 0) {
            UnityEngine.Object.Destroy(_edgeRoot.transform.GetChild(0));
        }
    }
}