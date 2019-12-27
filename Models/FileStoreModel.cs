using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WopiWebApi.Models
{
    public class FileStoreModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int Version { get; set; }

        public string LockId { get; set; }

        public long LockDuration { get; set; }

        public string OwnerId { get; set; }

        public string UserId { get; set; }

        public long FileSize { get; set; }

        public FileStoreModel()
        {
            Id = String.Empty;
            Name = String.Empty;
            Version = 1;
            LockId = String.Empty;
            LockDuration = 0;
            OwnerId = "TEST";
            UserId = String.Empty;
            FileSize = 0;
        }
    }
}
