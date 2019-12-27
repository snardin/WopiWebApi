using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WopiWebApi.Models;

namespace WopiWebApi.Utils
{
    public class FileManager
    {
        private readonly IHostingEnvironment _hostingEnv;

        private readonly object _filesLock = new object();

        private static FileStoreModel[] _files = null;

        public FileManager(IHostingEnvironment he)
        {
            _hostingEnv = he;
            string filePath = String.Empty;

            //Files to include in App_Data
            _files = new FileStoreModel[]
            {
                new FileStoreModel()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "doc1.docx"
                },
                new FileStoreModel()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "doc2.docx"
                }
            };

            filePath = Path.Combine(_hostingEnv.ContentRootPath, "App_Data", _files[0].Name);
            if (File.Exists(filePath))
                _files[0].FileSize = new System.IO.FileInfo(filePath).Length;

            filePath = Path.Combine(_hostingEnv.ContentRootPath, "App_Data", _files[1].Name);
            if (File.Exists(filePath))
                _files[1].FileSize = new System.IO.FileInfo(filePath).Length;            
        }


        #region File

        //Method to retrieve id and name of all docx files in App_Data
        public List<FileInfoModel> GetFilesInfo()
        {
            List<FileInfoModel> ret = new List<FileInfoModel>();
            string filePath = Path.Combine(_hostingEnv.ContentRootPath, "App_Data");

            for (int i = 0; i < _files.GetLength(0); i++)
            {
                if (File.Exists(Path.Combine(filePath, _files[i].Name)))
                    ret.Add(new FileInfoModel()
                    {
                        Id = _files[i].Id,
                        Name = _files[i].Name
                    });
            }

            return ret;
        }

        //Method to retrieve the information about a specific file
        public bool GetFileInfo(string fileId, out FileStoreModel fileInfo)
        {
            bool bRet = false;
            fileInfo = null;

            for (int i = 0; i < _files.GetLength(0); i++)
            {
                if (String.Compare(fileId, _files[i].Id, true) == 0)
                {
                    string filePath = Path.Combine(_hostingEnv.ContentRootPath, "App_Data", _files[i].Name);
                    if (File.Exists(filePath))
                    {
                        bRet = true;
                        fileInfo = _files[i];
                    }
                    break;
                }
            }
            return bRet;
        }

        //Method to retrieve a specific file
        public bool GetFile(string fileId, out byte[] fileContent)
        {
            bool bRet = false;
            fileContent = null;
            
            for (int i = 0; i < _files.GetLength(0); i++)
            {
                if (String.Compare(fileId, _files[i].Id, true) == 0)
                {
                    string filePath = Path.Combine(_hostingEnv.ContentRootPath, "App_Data", _files[i].Name);
                    if (File.Exists(filePath))
                    {
                        bRet = true;
                        fileContent = File.ReadAllBytes(filePath);                        
                    }
                    break;
                }
            }
            return bRet;
        }

        //Method to update a specific file
        public bool PutFile(string fileId, byte[] fileContent)
        {
            bool bRet = false;
            
            for (int i = 0; i < _files.GetLength(0); i++)
            {
                if (String.Compare(fileId, _files[i].Id, true) == 0)
                {
                    string filePath = Path.Combine(_hostingEnv.ContentRootPath, "App_Data", _files[i].Name);
                    if (File.Exists(filePath))
                    {
                        bRet = true;
                        File.WriteAllBytes(filePath, fileContent);
                        _files[i].FileSize = fileContent.Length;
                        _files[i].Version++;
                    }
                    break;
                }
            }
            return bRet;
        }

        #endregion

        #region Lock

        //Method to lock a specific file
        public bool LockFile(string fileId, string lockId, out string currentLockId)
        {
            bool bRet = false;
            currentLockId = String.Empty;

            lock (_filesLock)
            {
                for (int i = 0; i < _files.GetLength(0); i++)
                {
                    if (String.Compare(fileId, _files[i].Id, true) == 0)
                    {
                        bRet = true;
                        bool bExpired = false;
                        if (_files[i].LockDuration > 0)
                        {
                            long dateNow = DateTimeOffset.Now.ToUnixTimeSeconds();
                            if (dateNow > (_files[i].LockDuration * 30 * 60))
                                bExpired = true;
                        }

                        if (String.IsNullOrEmpty(_files[i].LockId) || String.Compare(_files[i].LockId, lockId, true) == 0 || bExpired)
                        {
                            _files[i].LockId = lockId;
                            _files[i].LockDuration = DateTimeOffset.Now.ToUnixTimeSeconds();                            
                        }
                        
                        currentLockId = _files[i].LockId;                        
                        break;
                    }
                }
            }

            return bRet;
        }

        //Method to re-lock a specific file
        public bool UnlockAndRelockFile(string fileId, string lockId, string oldLockId, out string currentLockId)
        {
            bool bRet = false;
            currentLockId = String.Empty;

            lock (_filesLock)
            {
                for (int i = 0; i < _files.GetLength(0); i++)
                {
                    if (String.Compare(fileId, _files[i].Id, true) == 0)
                    {
                        bRet = true;
                        bool bExpired = false;
                        if (_files[i].LockDuration > 0)
                        {
                            long dateNow = DateTimeOffset.Now.ToUnixTimeSeconds();
                            if (dateNow > (_files[i].LockDuration * 30 * 60))
                                bExpired = true;
                        }

                        if (String.Compare(_files[i].LockId, oldLockId, true) == 0 || bExpired)
                        {
                            _files[i].LockId = lockId;
                            _files[i].LockDuration = DateTimeOffset.Now.ToUnixTimeSeconds();
                        }
                        
                        currentLockId = _files[i].LockId;                        
                        break;
                    }
                }
            }

            return bRet;
        }

        //Method to unlock a specific file
        public bool UnlockFile(string fileId, string lockId, out string currentLockId)
        {
            bool bRet = false;
            currentLockId = String.Empty;

            lock (_filesLock)
            {
                for (int i = 0; i < _files.GetLength(0); i++)
                {
                    if (String.Compare(fileId, _files[i].Id, true) == 0)
                    {
                        bRet = true;
                        currentLockId = _files[i].LockId;
                        if (String.Compare(_files[i].LockId, lockId, true) == 0)
                            _files[i].LockId = "";
                        break;
                    }
                }
            }

            return bRet;
        }

        //Method to retrieve the lock information
        public bool GetLockFile(string fileId, out string currentLockId)
        {
            bool bRet = false;
            currentLockId = String.Empty;

            for (int i = 0; i < _files.GetLength(0); i++)
            {
                if (String.Compare(fileId, _files[i].Id, true) == 0)
                {
                    bRet = true;
                    currentLockId = _files[i].LockId;                    
                    break;
                }
            }

            return bRet;
        }

        //Method to refresh the lock
        public bool RefreshLockFile(string fileId, string lockId, out string currentLockId)
        {
            bool bRet = false;
            currentLockId = String.Empty;

            lock (_filesLock)
            {
                for (int i = 0; i < _files.GetLength(0); i++)
                {
                    if (String.Compare(fileId, _files[i].Id, true) == 0)
                    {
                        bRet = true;
                        currentLockId = _files[i].LockId;
                        if (String.Compare(_files[i].LockId, lockId, true) == 0)
                            _files[i].LockDuration = DateTimeOffset.Now.ToUnixTimeSeconds();
                        break;
                    }
                }
            }

            return bRet;
        }

        #endregion

        #region UserInfo

        //Method to add the user information
        public bool PutUserInfoFile(string fileId, string userId)
        {
            bool bRet = false;

            lock (_filesLock)
            {
                for (int i = 0; i < _files.GetLength(0); i++)
                {
                    if (String.Compare(fileId, _files[i].Id, true) == 0)
                    {
                        bRet = true;
                        _files[i].UserId = userId;
                        break;
                    }
                }
            }

            return bRet;
        }

        #endregion
    }

}
