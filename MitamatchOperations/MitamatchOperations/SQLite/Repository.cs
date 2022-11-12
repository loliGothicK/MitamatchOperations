using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Reflection;
using ColorCode.Compilation.Languages;
using static System.Environment;

namespace mitama.SQLite;

internal class Repository
{
    private static readonly SQLiteConnectionStringBuilder Builder = new()
    {
        DataSource = @$"{Path.Combine($"{GetFolderPath(SpecialFolder.LocalApplicationData)}", "MitamatchOperations")}\mitamatch.sqlite"
    };

    public static class Migrate
    {
        public static void Up()
        {
            Directory.CreateDirectory(Path.Combine($"{GetFolderPath(SpecialFolder.LocalApplicationData)}", "MitamatchOperations"));

            using var conn = new SQLiteConnection(Builder.ToString());
            ExecuteNoneQueryWithTransaction(conn, new[]{
            $$"""
                create table if not exists cache(
                    login_user_id INTEGER,
                    login_project_id INTEGER
                );
            """,
            $$"""
                create table if not exists project(
                    id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    name TEXT NOT NULL,
                    created_at TEXT NOT NULL DEFAULT (DATETIME('now', 'localtime')),
                    updated_at TEXT NOT NULL DEFAULT (DATETIME('now', 'localtime'))
                );
            """,
            $$"""
                create table if not exists member(
                    id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    project_id INTEGER NOT NULL,
                    name TEXT NOT NULL,
                    created_at TEXT NOT NULL DEFAULT (DATETIME('now', 'localtime')),
                    updated_at TEXT NOT NULL DEFAULT (DATETIME('now', 'localtime')),
                    foreign key (project_id) references project(id)
                );
            """,
            $$"""
                create table if not exists decks(
                    id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    project_id INTEGER NOT NULL,
                    created_at TEXT NOT NULL DEFAULT (DATETIME('now', 'localtime')),
                    updated_at TEXT NOT NULL DEFAULT (DATETIME('now', 'localtime')),
                    foreign key (project_id) references project(id)
                );
            """,
            $$"""
                create table if not exists deck(
                    deck_id INTEGER NOT NULL,
                    order_id INTEGER NOT NULL,
                    created_at TEXT NOT NULL DEFAULT (DATETIME('now', 'localtime')),
                    updated_at TEXT NOT NULL DEFAULT (DATETIME('now', 'localtime')),
                    foreign key (deck_id) references decks(id)
                );
            """
            });
        }
    }

    public static class UseCase
    {
        public static List<string> Regions
        {
            get
            {
                var result = new List<string>();

                using var conn = new SQLiteConnection(Builder.ToString());

                const string query = "SELECT name from project";
                var command = new SQLiteCommand(query, conn);
                var sdr = command.ExecuteReader();
                while (sdr.Read())
                {
                    result.Add((string)sdr["name"]);
                }
                return result;
            }
        }

        //public static void SaveCache(MemberInfo user)
        //{
        //    using var conn = new SQLiteConnection(Builder.ToString());

        //    ExecuteNoneQuery(conn, $"INSERT INTO cache(login_user_id) VALUES({user})");
        //}
    }

    private static void ExecuteNoneQuery(SQLiteConnection connection, string query)
    {
        connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = query;
        cmd.ExecuteNonQuery();
    }

    private static void ExecuteNoneQueryWithTransaction(SQLiteConnection connection, IEnumerable<string> queries)
    {
        connection.Open();
        var transaction = connection.BeginTransaction();

        try
        {
            foreach (var query in queries)
            {
                using var cmd = connection.CreateCommand();
                cmd.Transaction = transaction;

                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
            }
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}