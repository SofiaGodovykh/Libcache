using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kontur.Cache.Distributed
{
    public class RedisEndPoint
    {
        private readonly string host;

        public string Host
        {
            get { return host; }
        }

        private readonly int port;

        public int Port
        {
            get { return port; }
        }

        private readonly long db;
        
        public long Db
        {
            get { return db; }
        }

        public RedisEndPoint(string host, int port, long db)
        {
            this.host = host;
            this.port = port;
            this.db = db;
        }

        public override bool Equals(object obj)
        {
            RedisEndPoint temp = obj as RedisEndPoint;
            if(temp != null)
            {
                return (this.port == temp.port && this.host == temp.host && this.db == temp.db);
            }
            else
            {
                return false;
            }
        }
    }
}