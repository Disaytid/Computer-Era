﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Computer_Era.Game
{
    class DataBase
    {
        public string Name;

        public DataBase (string name)
        {
            this.Name = name;
        }

        public SQLiteConnection ConnectDB()
        {
            string baseName = "ComputerEra.db3";

            if (System.IO.File.Exists(Name)) //Проверка наличия файла базы
            {
                //Подключение при наличии файла
                SQLiteConnection connection = new SQLiteConnection("Data Source = " + Name);
                connection.Open();
                return connection;
            }
            else
            {
                //Создание фала базы данных и подключение
                SQLiteConnection.CreateFile(baseName);

                SQLiteConnection.CreateFile(baseName);

                SQLiteFactory factory = (SQLiteFactory)DbProviderFactories.GetFactory("System.Data.SQLite");
                using (SQLiteConnection connection = (SQLiteConnection)factory.CreateConnection())
                {
                    connection.ConnectionString = "Data Source = " + baseName;
                    connection.Open();

                    //Создание таблиц
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = @"CREATE TABLE [saves] (
                    [id] integer PRIMARY KEY AUTOINCREMENT NOT NULL,
                    [name] char(100) NOT NULL,
                    [date] datetime NOT NULL
                    );";
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }
                    return connection;
                }
            }
        }

    }
}
