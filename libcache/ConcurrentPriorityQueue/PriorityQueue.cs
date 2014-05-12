﻿namespace Kontur.Cache.ConcurrentPriorityQueue
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class ConcurrentPriorityQueue<T>
    {
        public SkipList<T> skipList;

        private int length;

        public int Length
        {
            get { return length; }
        }

        public ConcurrentPriorityQueue()
        {
            length = 0;
            skipList = new SkipList<T>();   
        }

        public bool Enqueue(T value, DateTime score)
        {
            var res = skipList.Add(value, score);

            if (res)
            {
                Interlocked.Increment(ref length);
                return true;
            }

            return false;
        }

        public T Peek()
        {
            var res = skipList.Peek().NodeValue.Value;
            if (res != null)
                return res;
            else
                return default(T);
        }

        public T Dequeue()
        {
            var node = skipList.FindAndMarkMin();
            if (node != null)
            {
                skipList.Remove(node);
                Interlocked.Decrement(ref length);
                return node.NodeValue.Value;     
            }
            return default(T);
        }
    }
}
