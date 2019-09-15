using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Everest.PuzzleGame
{
    public interface IGameUserService
    {
        void SavePlayer(string userName, PlayerData data, Action<string, PlayerData> onSuccess, Action<Exception> onFailure);
        void LoadPlayer(string userName, Action<PlayerData> onSuccess, Action<Exception> onFailure);
        void LoadLeaderBoard(Action<Dictionary<string, LeaderBoardUserData>> onSuccess, Action<Exception> onFailure);
        void UpdateUserInLeaderBoard(LeaderBoardUserData data, Action<LeaderBoardUserData> onSuccess, Action<Exception> onFailure);
    }

    public class GameUserService : IGameUserService
    {
        private string m_DbPath = UnityEngine.Application.persistentDataPath + "/db/";
        private string m_Extension = ".json";

        public void SavePlayer(string userName, PlayerData data, Action<string, PlayerData> onSuccess, Action<Exception> onFailure)
        {
            FileStream stream = null;
            try
            {
                TryCreateDBDirectory();
                if (!IsFileExist(userName))
                {
                    stream = CreateUser(userName);
                }
                else
                {
                    stream = new FileStream(m_DbPath + userName + m_Extension, FileMode.Open, FileAccess.ReadWrite);
                }

                WriteUser(data, stream);
                onSuccess?.Invoke(userName, data);
            }
            catch (Exception ex) { onFailure?.Invoke(ex); }

            finally
            {
                stream?.Close();
            }
        }

        public void LoadPlayer(string userName, Action<PlayerData> onSuccess, Action<Exception> onFailure)
        {
            FileStream stream = null;
            try
            {
                TryCreateDBDirectory();
                if (!IsFileExist(userName))
                {
                    stream = CreateUser(userName);
                }
                else
                {
                    stream = new FileStream(m_DbPath + userName + m_Extension, FileMode.Open, FileAccess.ReadWrite);
                }

                onSuccess?.Invoke(ReadUser<PlayerData>(stream));
            }
            catch(Exception ex) { onFailure?.Invoke(ex); }
            finally
            {
                stream?.Close();
            }
        }

        //Start - Leaderboard Service methods
        public void LoadLeaderBoard(Action<Dictionary<string, LeaderBoardUserData>> onSuccess, Action<Exception> onFailure)
        {
            FileStream stream = null;
            try
            {
                TryCreateDBDirectory();
                string filename = "leaderboard";
                if (!IsFileExist(filename))
                {
                    stream = CreateUser(filename);
                }
                else
                {
                    stream = new FileStream(m_DbPath + filename + m_Extension, FileMode.Open, FileAccess.ReadWrite);
                }

                var users = ReadUser<LeaderBoard>(stream);
                onSuccess?.Invoke(users.data);//pass back the userdata
            }
            catch(Exception ex)
            {
                onFailure?.Invoke(ex);
            }
            finally
            {
                stream?.Close();
            }
        }

        public void UpdateUserInLeaderBoard(LeaderBoardUserData data, Action<LeaderBoardUserData> onSuccess, Action<Exception> onFailure)
        {
            FileStream stream = null;
            try
            {
                TryCreateDBDirectory();
                string filename = "leaderboard";
                if (!IsFileExist(filename))
                    stream = CreateUser(filename);
                else
                    stream = new FileStream(m_DbPath + filename + m_Extension, FileMode.Open, FileAccess.ReadWrite);

                var users = ReadUser<LeaderBoard>(stream);

                //if no leaderboard create one
                if (users == null)
                    users = new LeaderBoard();

                if (users.data.ContainsKey(data.Username))
                    users.data[data.Username] = data;
                else
                    users.data.Add(data.Username, data);

                //closing read stream
                stream.Close();
                stream = new FileStream(m_DbPath + filename + m_Extension, FileMode.Open, FileAccess.ReadWrite);
                //now write the update leaderboard to file
                WriteUser(users, stream);
                onSuccess?.Invoke(data);//pass back the userdata
            }
            catch(Exception ex)
            {
                onFailure?.Invoke(ex);
            }
            finally
            {
                stream?.Close();
            }
        }
        //End - Leaderboard Service methods

        private bool IsFileExist(string filename)
        {
            return File.Exists(m_DbPath + filename + m_Extension);
        }

        private FileStream CreateUser(string filename)
        {
            return File.Create(m_DbPath + filename + m_Extension);
        }

        private void TryCreateDBDirectory()
        {
            if (!Directory.Exists(m_DbPath))
                Directory.CreateDirectory(m_DbPath);
        }

        private void WriteUser<T>(T data, FileStream stream)
        {
            try
            {
                using (var sr = new StreamWriter(stream))
                using (var reader = new JsonTextWriter(sr))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(reader, data);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unhandled exception : " + ex.Message);
            }
        }

        private T ReadUser<T>(FileStream stream)
        {
            try
            {
                using (var sr = new StreamReader(stream))
                using (var reader = new JsonTextReader(sr))
                {
                    var serializer = new JsonSerializer();
                    return serializer.Deserialize<T>(reader);
                }
            }
            catch(Exception ex) {
                throw new Exception("Unhandled exception : " + ex.Message);
            }
        }
    }
}
