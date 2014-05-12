namespace Kontur.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using ServiceStack.Redis;

    public class RedisReadWrite : IQueue<string>
    {
        private readonly RedisClient client = new RedisClient();

        public void Enqueue(QueueNode<string> q)
        {
            client.Add(q.Key, q.GetItemValue(), q.GetItemTime());
        }
          
        public QueueNode<string> Dequeue()
        {
            return null;
        }

        public bool IsEmpty()
        {
            return false; //?
        }
    }
}
