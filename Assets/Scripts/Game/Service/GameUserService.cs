using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Everest.PuzzleGame
{
    public interface IGameUserService
    {
        void SavePlayer(string userName, PlayerData data, Action<string, PlayerData> onSuccess, Action<Exception> onFailure);
        void LoadPlayer(string userName, Action<PlayerData> onSuccess, Action<Exception> onFailure);
    }

    public class GameUserService : IGameUserService
    {
        private string m_DataPath = UnityEngine.Application.dataPath + "/db/users.db";
        private string m_DbPath = UnityEngine.Application.dataPath + "/db/";
        private string m_Extension = ".db";

        public GameUserService()
        {
        }

        public void TryLoadUser(string userName)
        {

        }

        public void LoadAllUsers()
        {
           var files =  Directory.GetFiles(UnityEngine.Application.dataPath + "/db/");
            UnityEngine.Debug.Log(files.Length);
        }

        public void SaveUserScore(int movesTaken)
        {

        }

        public void SavePlayer(string userName, PlayerData data, Action<string, PlayerData> onSuccess, Action<Exception> onFailure)
        {
            try
            {
                if (IsFileExist(userName))
                {
                    using (var fStream = new FileStream(m_DbPath + userName + m_Extension, FileMode.Open))
                    {
                        WriteUser(data, fStream);
                        onSuccess?.Invoke(userName, data);
                    }
                }
                else
                {
                    //create one
                    var stream = CreateUser(userName);
                    stream.Close();
                    SavePlayer(userName, data, onSuccess, onFailure); // danger call
                }
            }
            catch (Exception ex) { onFailure?.Invoke(ex); }
        }

        public void LoadPlayer(string userName, Action<PlayerData> onSuccess, Action<Exception> onFailure)
        {
            try
            {
                if (IsFileExist(userName))
                {
                    using (var stream = new FileStream(m_DbPath + userName + m_Extension, FileMode.Open))
                    {
                        onSuccess?.Invoke(ReadUser(stream));
                    }
                }
                else
                {
                    //create one
                    var stream = CreateUser(userName);
                    stream.Close();
                    LoadPlayer(userName, onSuccess, onFailure); // danger call
                }
            }
            catch(Exception ex) { onFailure?.Invoke(ex); }
        }

        public bool IsFileExist(string userName)
        {
            return File.Exists(m_DbPath + userName + m_Extension);
        }

        private FileStream CreateUser(string userName)
        {
            return File.Create(m_DbPath + userName + m_Extension);
        }

        private void WriteUser(PlayerData data, FileStream stream)
        {
            try
            {
                var binary = new BinaryFormatter();
                binary.Serialize(stream, data);
                stream.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Unhandled exception : " + ex.Message);
            }
        }

        private PlayerData ReadUser(FileStream stream)
        {
            try
            {
                PlayerData obj = null;
                var binary = new BinaryFormatter();
                obj = binary.Deserialize(stream) as PlayerData;
                stream.Close();
                return obj;
            }
            catch(Exception ex) {
                throw new Exception("Unhandled exception : " + ex.Message);
            }
        }
    }
}
