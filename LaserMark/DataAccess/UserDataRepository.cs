using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserMark.DataAccess
{
    public static class UserDataRepository
    {
        private static string db = @"laster.db";

        private static SQLiteConnectionStringBuilder connectionStringBuilder;

        static UserDataRepository()
        {
            connectionStringBuilder = new SQLiteConnectionStringBuilder();

            connectionStringBuilder.DataSource = db;
        }

        public static void Insert(UserDataDto user)
        {
            using (var connection = new SQLiteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();

                command.CommandText = $@"DELETE FROM [UserData]
WHERE Sequence = {user.Sequence} and EXISTS (select * from [UserData] where Sequence = {user.Sequence})";

                command.ExecuteNonQuery();

                command.CommandText = $@"INSERT INTO [UserData]
([Token], [BgImagePosX], [BgImagePosY], [EzdImagePosX], [EzdImagePosY], [Sequence], [Login], [Password], [Url], [FullImage], [BgImage], [EzdImage])
VALUES ('{user.Token}'
,{user.BgImagePosX}
,{user.BgImagePosY}
,{user.EzdImagePosX}
,{user.EzdImagePosY}
,{user.Sequence},
'{user.Login}',
'{user.Password}', 
'{user.Url}', 
'{user.FullImage}', 
'{user.BgImage}', 
'{user.EzdImage}');";

                command.ExecuteNonQuery();
            }
        }

        public static List<UserDataDto> GetAllUser()
        {
            List<UserDataDto> user = new List<UserDataDto>();
            try
            {
                using (var connection = new SQLiteConnection(connectionStringBuilder.ConnectionString))
                {
                    connection.Open();

                    var command = connection.CreateCommand();

                    command.CommandText = "SELECT * FROM UserData";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user.Add(new UserDataDto
                            {
                                Id = (long)reader["Id"],
                                Token = (string)reader["Token"],
                                Login = (string)reader["Login"],
                                Password = (string)reader["Password"],
                                Url = (string)reader["Url"],
                                FullImage = (string)reader["FullImage"],
                                BgImage = (string)reader["BgImage"],
                                EzdImage = (string)reader["EzdImage"],
                                Sequence = (long)reader["Sequence"],
                                BgImagePosX = (long)reader["BgImagePosX"],
                                BgImagePosY = (long)reader["BgImagePosY"],
                                EzdImagePosX = (long)reader["EzdImagePosX"],
                                EzdImagePosY = (long)reader["EzdImagePosY"],
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return user;
        }

        public static UserDataDto GetByTabIndex(long index)
        {
            UserDataDto user = new UserDataDto();
            try
            {
                using (var connection = new SQLiteConnection(connectionStringBuilder.ConnectionString))
                {
                    connection.Open();

                    var command = connection.CreateCommand();

                    command.CommandText = $@"SELECT * FROM UserData Where Sequence = {index}";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user = new UserDataDto
                            {
                                Id = (long)reader["Id"],
                                Token = (string)reader["Token"],
                                Login = (string)reader["Login"],
                                Password = (string)reader["Password"],
                                Url = (string)reader["Url"],
                                FullImage = (string)reader["FullImage"],
                                BgImage = (string)reader["BgImage"],
                                EzdImage = (string)reader["EzdImage"],
                                Sequence = (long)reader["Sequence"],
                                BgImagePosX = (long)reader["BgImagePosX"],
                                BgImagePosY = (long)reader["BgImagePosY"],
                                EzdImagePosX = (long)reader["EzdImagePosX"],
                                EzdImagePosY = (long)reader["EzdImagePosY"],
                            };
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return user;
        }

        public static byte CheckSquence(long index)
        {
            long ifExist = 0;
            try
            {
                using (var connection = new SQLiteConnection(connectionStringBuilder.ConnectionString))
                {
                    connection.Open();

                    var command = connection.CreateCommand();

                    command.CommandText = $@"select EXISTS (select * from [UserData] where Sequence = {index})";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ifExist = (long)reader[0];
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return (byte)ifExist;
        }

        public static void DeleteByTabIndex(long index)
        {
            using (var connection = new SQLiteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();

                command.CommandText = $@"DELETE FROM [UserData] WHERE Sequence = {index}";

                command.ExecuteNonQuery();
            }
        }
    }
}