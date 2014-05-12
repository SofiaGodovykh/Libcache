namespace Kontur.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PriorityQueue<T> : IEnumerable<T>
    {
        private readonly IComparer<T> comparer;
        private readonly List<T> items;

        public PriorityQueue()
            : this(Comparer<T>.Default)
        {
        }

        public PriorityQueue(IComparer<T> comparer)
        {
            this.items = new List<T>();
            this.comparer = comparer;
        }

        public int Count
        {
            get { return items.Count; }
        }

        public List<T> GetItems
        {
            get { return items; }
        }

        public void Enqueue(T item)
        {
            this.items.Add(item);
            int i = items.Count - 1;
            T temp;
            while (i > 0 && comparer.Compare(this.items[i], this.items[(i - 1) / 2]) < 0)
            {
                temp = this.items[i];
                this.items[i] = this.items[(i - 1)/ 2];
                this.items[(i - 1) / 2] = temp;
                i = (i - 1) / 2;
            }
        }

        public T Peek()
        {
            if (items.Count == 0)
            {
                throw new Exception("the queue is empty");
            }

            var result = items[0];
            return result;
        }

        public T RemoveAt(int position)
        {
            if (this.items.Count == 0)
            {
                throw new Exception("the queue is empty");
            }

            if (position >= this.items.Count || position < 0)
            {
                throw new Exception("the position is out of bounds");
            }

            var x = this.items[position];
            this.items[position] = this.items[this.items.Count - 1];
            this.items.RemoveAt(this.items.Count - 1);
            int i = position;
            int j;

            T temp;
            while (2 * (i + 1) <= (items.Count - 1) && i >= position)
            {
                if ((2 * i) < this.items.Count - 1
                    && this.comparer.Compare(this.items[2 * (i + 1)], this.items[2 * (i + 1) -1]) < 0)
                {
                    j = 2 * (i + 1);
                }
                else
                {
                    j = 2 * (i + 1) - 1;
                }


                if (comparer.Compare(this.items[i], this.items[j]) > 0)
                {
                    temp = this.items[i];
                    this.items[i] = this.items[j];
                    this.items[j] = temp;
                    i = j;
                }
                else
                {
                    i = items.Count;
                }
            }

            if (this.items.Count == 2)
            {
                if (this.comparer.Compare(this.items[0], this.items[1]) > 0)
                {
                    temp = this.items[0];
                    this.items[0] = this.items[1];
                    this.items[1] = temp;
                }
            }

            return x;
        }

        public T Dequeue()
        {
            return this.RemoveAt(0);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}