using UnityEngine;
using System.Collections;

/**
 * Фабрика для создания блокирующих элементов.
 * 
 * @author Timur Bogotov timur@e-magic.org
 * @author Azamat Bogotov azamat@e-magic.org
 */
public static class CellFactory
{
    /**
     * Создает блокирующий элемент по типу.
     * 
     * @param type   тип блокирующего элемента
     * @param parent контейнер для создаваемого объекта ячейки
     * 
     * @return CellBlocker класс блокирующего элемента
     * @throw System.ArgumentException, System.NullReferenceException
     */
    public static Cell createNew(CellType type, Grid grid, uint chipTypes, IntVector2 position, GameObject parent)
    {
        // Создание ячейки
        GameObject prefab = Resources.Load<GameObject>("prefabs/cellPrefab");
        
        if (prefab == null) {
            throw new System.NullReferenceException("Ошибка! Не удалось загрузить префаб ячеки");
        }
        
        GameObject cellObject = (GameObject)UnityEngine.Object.Instantiate(prefab);

        Cell cell = cellObject.GetComponent<Cell>();
        
        if (cell == null) {
            UnityEngine.Object.Destroy(cellObject);
            throw new System.NullReferenceException("Ошибка! На префабе нет компоненты Cell");
        }

        // Инициализация ячейки
        CellBehaviour behaviour = null;

        switch (type) {
            case CellType.EMPTY:
                behaviour = new EmptyCell();
                break;

            case CellType.NORMAL:
                behaviour = new NormalCell(cell, grid);

                Sprite sprite = Resources.Load<Sprite>("textures/cellBackground");
                
                if (sprite == null) {
                    throw new System.NullReferenceException("Ошибка! Не удалось загрузить спрайт: textures/cellBackground");
                }
                
                cell.GetComponent<SpriteRenderer>().sprite = sprite;

                break;

            case CellType.BUILDER:
                behaviour = new BuilderCell(cell, grid, chipTypes);
                break;

            case CellType.TELEPORTER:
                behaviour = new TeleporterCell(cell, grid);
                break;

            default:
                throw new System.ArgumentException("Ошибка! Неизвестный параметр: " + (int)type, "type");
        }

        cell.initialize(behaviour, position);

        if (parent != null) {
            cell.transform.parent        = parent.transform;
            cell.transform.localPosition = Vector3.zero;
        }

        return cell;
    }
}
