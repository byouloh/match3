using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

/**
 * Класс для хранения результата SQL-запроса на выборку данных.
 * 
 * @code
 * QueryResult res = DataBase.query("SELECT id, field FROM table");
 * 
 * for (int i = 0; i < res.numRows()) {
 *     Debug.Log(res[i].asInt("id"));
 *     Debug.Log(res[i].asString("field"));
 * }
 * @endcode
 * 
 * @author Azamat Bogotov azamat@e-magic.org
 */
public class QueryResult
{
    /**
     * Список полей(столбцов) в каждой записи.
     */
    private List<string> _fields;

    /**
     * Список записей в результате запроса.
     */
    private List<DataRow> _rows;

    /**
     * Конструктор класса.
     */
    public QueryResult()
    {
        _fields = new List<string>();
        _rows   = new List<DataRow>();
    }

    /**
     * Позволяет получить доступ к записи по его индексу.
     * 
     * @param row индекс записи в списке.
     * 
     * @return DataRow Запись, содержащий данные полей.
     */
    public DataRow this[int row]
    {
        get
        {
            return _rows[row];
        }
    }

    /**
     * Добавляет новое поле в таблицу.
     * 
     * Перед тем, как добавлять записи(строки), сначала нужно сформировать список полей.
     * 
     * @param fieldName Имя поля
     * 
     * @note Имя поля не может быть пустым.
     * 
     * @return void
     * @throw Exception
     */
    public void addField(string fieldName)
    {
        if (fieldName.Length <= 0) {
            throw new Exception("Field name can't be empty");
        }

        _fields.Add(fieldName);
    }

    /**
     * Добавляет новую запись(строку) в таблицу.
     * 
     * @param values Массив значений полей записи.
     * 
     * @return void
     * @throw IndexOutOfRangeException
     */
    public void addRow(object[] values)
    {
        if (values.Length != _fields.Count || values.Length <= 0) {
            throw new IndexOutOfRangeException("The number of values in the row must match the number of column");
        }
        
        DataRow row = new DataRow();

        for (int i = 0; i < values.Length; i++) {
            row[_fields[i]] = values[i];
        }
        
        _rows.Add(row);
    }

    /**
     * Добавляет новую запись(строку) в таблицу.
     * 
     * @param values Массив значений полей записи.
     * 
     * @return void
     * @throw IndexOutOfRangeException
     */
    public void addRow(DataRow row)
    {
        if (row.Count != _fields.Count) {
            throw new IndexOutOfRangeException("The number of values in the row must match the number of column");
        }

        _rows.Add(row);
    }

    /**
     * Возвращает запись таблицы по индексу.
     * 
     * @param row Индекс строки в таблице
     * 
     * @return DataRow
     * @throw IndexOutOfRangeException
     */
    public DataRow getRow(int row)
    {
        if (row < 0 || row >= _rows.Count) {
            throw new IndexOutOfRangeException("The number of values in the row must match the number of column");
        }

        return _rows[row];
    }

    /**
     * Возвращает список полей таблицы результата запроса.
     * 
     * @return List<string> Массив строк со значениями названий полей
     */
    public List<string> getFields()
    {
        return _fields;
    }

    /**
     * Возвращает количество записей(строк) в результате запроса.
     * 
     * @return int Возвращает количество записей(строк) в результате запроса.
     */
    public int numRows()
    {
        return _rows.Count;
    }
}