using UnityEngine;
using System;
using System.Collections;

/**
 * @file
 * Содержит класс исключения для ошибок, связанных с БД.
 */

/**
 * Исключение для ошибок, связанных с sqlite.
 * 
 * @author Azamat Bogotov azamat@e-magic.org
 */
public class DatabaseException: Exception
{
	/**
	 * Конструктор.
	 * 
	 * @param message Текст ошибки
	 */
    public DatabaseException(string message): base(message)
	{
		// nothing
	}
}