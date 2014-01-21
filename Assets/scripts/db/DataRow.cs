using System;
using System.Collections;
using System.Collections.Generic;

/**
 * @file
 * Содержит класс для хранения данных одной записи таблицы БД.
 */

/**
 * Класс для хранения данных одной записи таблицы БД.
 * 
 * @author Azamat Bogotov azamat@e-magic.org
 */
public class DataRow: Dictionary<string, object>
{
	/**
	 * Задание и получение значений полей записи.
	 */
	public new object this[string column]
	{
		get
        {
			if (ContainsKey(column)) {
				return base[column];
			}

			return null;
		}

        set
        {
			if (ContainsKey(column)) {
				base[column] = value;
			} else {
				Add(column, value);
			}
		}
    }

    /**
     * Возвращает значение поля как строку.
     * 
     * @param string Название поля.
     * 
     * @return string 
     */
	public string asString(string column)
    {
		object v = this[column];

		return (v == null) ? null : v.ToString();
	}

    /**
     * Возвращает значение поля как целое число.
     * 
     * @param string Название поля.
     * 
     * @return long
     */
	public long asLong(string column)
    {
		object v = this[column];

		if (v == null) {
			return 0;
		} else if (v is long) {
			return (long)v;
		} else if (v is int) {
			return (long)(int)v;
		} else if (v is double) {
			return (long)(double)v;
		} else if (v is string) {
			return long.Parse((string)v);
		}
		
        return int.Parse(v.ToString());
	}
	
    /**
     * Возвращает значение поля как целое число.
     * 
     * @param string Название поля.
     * 
     * @return int 
     */
	public int asInt(string column)
    {
		return asInt(column, 0);
	}
	
    /**
     * Возвращает значение поля как целое число.
     * 
     * @param string Название поля.
     * 
     * @return int 
     */
	public int asInt(string column, int defaultValue)
    {
		object v = this[column];

		if (v == null) {
			return defaultValue;
		} else if (v is long) {
			return (int)(long)v;
		} else if (v is int) {
			return (int)v;
		} else if (v is double) {
			return (int)(double)v;
		} else if (v is string) {
			return int.Parse((string)v);
		}

		return int.Parse(v.ToString());
	}
	
    /**
     * Возвращает значение поля как вещественное число.
     * 
     * @param string Название поля.
     * 
     * @return double 
     */
	public double asDouble(string column)
    {
		return asDouble(column, 0);
	}
	
    /**
     * Возвращает значение поля как вещественное число.
     * 
     * @param string Название поля.
     * 
     * @return double 
     */
	public double asDouble(string column, double defaultValue)
    {
		object v = this[column];

		if (v == null) {
			return defaultValue;
		} else if(v is long) {
			return (double)(long)v;
		} else if( v is int) {
			return (double)(int)v;
		} else if(v is double) {
			return (double)v;
		} else if(v is string) {
			return double.Parse((string)v);
		}

		return double.Parse(v.ToString());
	}

    /**
     * Возвращает флаг проверки поля на пустое значение(null).
     * 
     * @param column Имя поля
     * 
     * @return bool Возвращает true, если значение поля равено null, иначе false
     */
    public bool isNull(string column)
    {
        return (this[column] == null);
    }
}