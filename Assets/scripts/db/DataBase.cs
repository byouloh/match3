using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/**
 * @file
 * Содержит класс для работы с базой данных.
 */

/**
 * Класс для работы с базой данных.
 * 
 * Позволяет выполнять запросы на выполнение и выборку к БД sqlite 3.x.
 * Работает напрямую с нативными методами динамически подгружаемой библиотеки.
 * 
 * @author Azamat Bogotov azamat@e-magic.org
 */
public class DataBase
{
    /**
     * @name Значения, возвращаемые нативными методами библиотеки sqlite.
     * @{
     */
    const int SQLITE_OK      = 0;    /**< Операция прошла успешно. */
    const int SQLITE_ROW     = 100;  /**< Результат является записью(строкой) таблицы. */
    const int SQLITE_DONE    = 101;  /**< SQL-запрос был выполнен успешно. */
    const int SQLITE_INTEGER = 1;    /**< Целочисленный тип поля. */
    const int SQLITE_FLOAT   = 2;    /**< Вещественный тип поля. */
    const int SQLITE_TEXT    = 3;    /**< Текстовый тип поля. */
    const int SQLITE_BLOB    = 4;    /**< Бинарный тип поля. */
    const int SQLITE_NULL    = 5;    /**< Пустое значение поля. */
    /** @} */

    /**
     * Экземпляр класса, который создается один раз при первом обращении.
     */
    private static DataBase _instance = null;

    /**
     * Путь к файлу базы данных.
     * 
     * Файл БД должен лежать в Assets/StreamingAssets с расширением *.bytes.
     * При первом обращении, этот файл будет скопирован в другое место, 
     * чтобы была возможность редактировать данные.
     */
    private string _dbPath;

    /** Соединение с БД. */
    private IntPtr _connection;
    
    /** Флаг установленности соединения с БД. */
    private bool _isConnectionOpen;

    /**
     * Конструктор класса.
     * 
     * Инициализирует файл базы данных, а если ее нет, то создает.
     * @throw DatabaseException
     */
    private DataBase()
    {
        // TODO необходимо получать имя БД из кофигурационного файла.
        if (!initialize("db.bytes")) {
            throw new DatabaseException("Database[db.bytes] not found");
        }
    }

    /**
     * Возвращает экземпляр класса.
     * 
     * Следит за тем, чтобы экземпляр класса был один во всем проекте.
     * 
     * @return DataBase
     */
    public static DataBase getInstance()
    {
        if (_instance == null) {
            _instance = new DataBase();
        }
        
        return _instance;
    }

    #region Public methods

    /**
     * Инициализирует файл БД.
     * 
     * Файл БД ищется в папке Assets/StreamingAssets и, если он найден, 
     * то копируется в папку persistentPath(в зависимости от платформы) для того, 
     * чтобы его можно было редактировать.
     * 
     * @param dbName Имя файла базы данных (*.bytes).
     * 
     * @return bool Возвращает true, если файл БД был инициализирован, иначе false
     */
    public bool initialize(string dbName) {
        _dbPath = Path.Combine(Application.streamingAssetsPath, dbName);

        if (!File.Exists(_dbPath)) {
            string sourcePath = Path.Combine(Application.streamingAssetsPath, dbName);
            
            if (sourcePath.Contains ("://")) {
                // Android  
                WWW www = new WWW(sourcePath);

                while (!www.isDone);
                
                if (String.IsNullOrEmpty(www.error)) {                  
                    File.WriteAllBytes(_dbPath, www.bytes);
                } else {
                    return false;
                }
            } else {
                // Mac, Windows, Iphone
                if (File.Exists(sourcePath)) {
                    File.Copy(sourcePath, _dbPath, true);
                } else {
                    return false;
                }
            }
        }

        return true;
    }

    /**
     * Открывает соединение с БД.
     * 
     * bool Возвращает true, если соединение прошло успешно, иначе false
     */
    public bool openConnection() {
        if (_isConnectionOpen) {
            return true;
        }
        
        if (sqlite3_open(_dbPath, out _connection) != SQLITE_OK) {
            return false;
        }
        
        _isConnectionOpen = true;

        return true;
    }

    /**
     * Закрывает соединение с БД.
     * 
     * @return void
     */
    public void closeConnection()
    {
        if (_isConnectionOpen) {
            sqlite3_close (_connection);
        }
        
        _isConnectionOpen = false;
    }

    /**
     * Выполняет одиночный SQL-запрос к БД на изменение данных.
     * 
     * @param sql Строка SQL-запроса
     * 
     * @return int Возвращает число затронутых записей в таблицах
     * @throw DatabaseException
     */
    public int execute(string sql)
    {
        if (!this.openConnection()) {
            throw new DatabaseException("SQLite database is not open.");
        }

        IntPtr stmHandle = _prepare(sql);

        if (sqlite3_step(stmHandle) != SQLITE_DONE) {
            this.closeConnection();
            throw new DatabaseException("Could not execute SQL statement.");
        }

        try {
            _finalize(stmHandle);
        } finally {
            this.closeConnection();
        }

        return 0;
    }

    /**
     * Выполняет SQL-запрос к БД на выборку данных.
     * 
     * @param sql Строка SQL-запроса
     * 
     * @return QueryResult Возвращает число затронутых записей в таблицах
     * @throw DatabaseException
     */
    public QueryResult query(string sql)
    {
        if (!this.openConnection()) {
            throw new DatabaseException("SQLite database is not open.");
        }

        IntPtr stmHandle = _prepare(sql);

        int i;
        int columnCount    = sqlite3_column_count(stmHandle);
        QueryResult result = new QueryResult();

        for (i = 0; i < columnCount; i++) {
            string columnName = Marshal.PtrToStringAnsi(sqlite3_column_name(stmHandle, i));
            result.addField(columnName);
        }

        while (sqlite3_step(stmHandle) == SQLITE_ROW) {
            object[] row = new object[columnCount];

            for (i = 0; i < columnCount; i++) {
                switch (sqlite3_column_type(stmHandle, i)) {
                    case SQLITE_INTEGER:
                        row[i] = sqlite3_column_int(stmHandle, i);
                        break;
                        
                    case SQLITE_TEXT:
                        IntPtr text = sqlite3_column_text(stmHandle, i);
                        row[i] = Marshal.PtrToStringAnsi(text);
                        break;
                        
                    case SQLITE_FLOAT:
                        row[i] = sqlite3_column_double(stmHandle, i);
                        break;
                        
                    case SQLITE_BLOB:
                        IntPtr blob = sqlite3_column_blob(stmHandle, i);
                        int size    = sqlite3_column_bytes(stmHandle, i);
                        byte[] data = new byte[size];

                        Marshal.Copy (blob, data, 0, size);

                        row[i] = data;

                        break;
                        
                    case SQLITE_NULL:
                        row[i] = null;
                        break;
                }
            }

            result.addRow(row);
        }
        
        try {
            _finalize (stmHandle);
        } finally {
            this.closeConnection();
        }
        
        return result;
    }

    /**
     * Выполняет множественный SQL-запрос к БД на изменение данных.
     * 
     * Запросы должны быть разделены символом ";".
     * 
     * @param sql Строка SQL-запроса
     * 
     * @return void
     * @throw DatabaseException
     */
    public void executeAll(string script)
    {
        string[] statements = script.Split(';');
        
        foreach (string statement in statements) {
            if (!string.IsNullOrEmpty(statement.Trim())) {
                execute(statement);
            }
        }
    }

    #endregion

    #region Private methods

    /**
     * Выполняет запрос к БД.
     * 
     * @param sql Строка SQL-запроса
     * 
     * @return Возвращает результат запроса
     * @throw DatabaseException
     */
    private IntPtr _prepare(string sql)
    {
        IntPtr stmHandle;
        
        if (sqlite3_prepare_v2(_connection, sql, sql.Length, out stmHandle, IntPtr.Zero) != SQLITE_OK) {
            IntPtr errorMsg = sqlite3_errmsg(_connection);
            throw new DatabaseException(Marshal.PtrToStringAnsi (errorMsg));
        }
        
        return stmHandle;
    }

    /**
     * Завершает работу с результатом SQL-запроса.
     * 
     * @param stmHandle Указатель на результат SQL-запроса
     * @throw DatabaseException
     */
    private void _finalize(IntPtr stmHandle)
    {
        if (sqlite3_finalize(stmHandle) != SQLITE_OK) {
            throw new DatabaseException ("Could not finalize SQL statement.");
        }
    }

    #endregion

    #region Native library methods
    /**
     * @name Импортируемые методы из библиотеки sqlite 3.
     * @{
     */
    [DllImport("sqlite3", EntryPoint = "sqlite3_open")]
    private static extern int sqlite3_open(string filename, out IntPtr db);
    
    [DllImport("sqlite3", EntryPoint = "sqlite3_close")]
    private static extern int sqlite3_close(IntPtr db);
    
    [DllImport("sqlite3", EntryPoint = "sqlite3_prepare_v2")]
    private static extern int sqlite3_prepare_v2(IntPtr db, string zSql, int nByte, out IntPtr ppStmpt, IntPtr pzTail);
    
    [DllImport("sqlite3", EntryPoint = "sqlite3_step")]
    private static extern int sqlite3_step(IntPtr stmHandle);
    
    [DllImport("sqlite3", EntryPoint = "sqlite3_finalize")]
    private static extern int sqlite3_finalize(IntPtr stmHandle);
    
    [DllImport("sqlite3", EntryPoint = "sqlite3_errmsg")]
    private static extern IntPtr sqlite3_errmsg(IntPtr db);
    
    [DllImport("sqlite3", EntryPoint = "sqlite3_column_count")]
    private static extern int sqlite3_column_count(IntPtr stmHandle);
    
    [DllImport("sqlite3", EntryPoint = "sqlite3_column_name")]
    private static extern IntPtr sqlite3_column_name(IntPtr stmHandle, int iCol);
    
    [DllImport("sqlite3", EntryPoint = "sqlite3_column_type")]
    private static extern int sqlite3_column_type(IntPtr stmHandle, int iCol);
    
    [DllImport("sqlite3", EntryPoint = "sqlite3_column_int")]
    private static extern int sqlite3_column_int(IntPtr stmHandle, int iCol);
    
    [DllImport("sqlite3", EntryPoint = "sqlite3_column_text")]
    private static extern IntPtr sqlite3_column_text(IntPtr stmHandle, int iCol);
    
    [DllImport("sqlite3", EntryPoint = "sqlite3_column_double")]
    private static extern double sqlite3_column_double(IntPtr stmHandle, int iCol);
    
    [DllImport("sqlite3", EntryPoint = "sqlite3_column_blob")]
    private static extern IntPtr sqlite3_column_blob(IntPtr stmHandle, int iCol);
    
    [DllImport("sqlite3", EntryPoint = "sqlite3_column_bytes")]
    private static extern int sqlite3_column_bytes(IntPtr stmHandle, int iCol);
    /** @} */

    #endregion
}