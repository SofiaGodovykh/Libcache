namespace CacheCleaner
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class FibonacciQueue<T, V>  where T : System.IComparable<T>
    {
        private int count;

        private Node<T, V> minNode;

        public FibonacciQueue()
        {
            count = 0;
            minNode = null;
        }

        private FibonacciQueue(Node<T, V> root, int count)
        {
            minNode = root;
            this.count = count;
        }

        public void Insert(Node<T, V> node)
        {
            node.left = node;
            node.right = node;
            node.mark = false;

            if (minNode == null)
            {
                minNode = node;
            }
            else
            {
                if (node.key.CompareTo(minNode.key) < 0)
                {
                    node.left = minNode;
                    node.right = minNode.right;
                    minNode = node;
                }
                else
                {
                    node.right = minNode;
                    node.left = minNode.left;
                }
            }

            count++;
        }

        public Node<T, V> MinNode()
        {
            return minNode;
        }

        public FibonacciQueue<T, V> Union(FibonacciQueue<T, V> first, FibonacciQueue<T, V> second)
        {
            var result = new FibonacciQueue<T, V>();
            result.minNode = first.minNode;
            Node<T, V> temp1, temp2;

            if (first.minNode == null)
            {
                return second;
            }

            if (second.minNode == null)
            {
                return first;
            }

            temp1 = first.minNode.right;
            temp2 = second.minNode.right;
            first.minNode.right = temp2;
            second.minNode.right = temp1;
            temp2.left = first.minNode;
            temp1.left = second.minNode;

            if (first.minNode.key.CompareTo(second.minNode.key) < 0)
            {
                return new FibonacciQueue<T, V>(first.minNode, first.count + second.count);
            }

            return new FibonacciQueue<T, V>(second.minNode, first.count + second.count);
        }


        public void Consolidate()
        {
        //    for(int i = 0; i < ; i++)
        }

        public void ExtractMin()
        {
            if (minNode == null)
            {
                return;
            }

            var x = minNode.child;

            if (x != null)
            {
                do
                {
                    x.parent = null;
                    this.Insert(x);
                    x = x.left;
                }
                while (x != minNode.child);
                minNode.left.right = minNode.right;
                minNode.right.left = minNode.left;
                for (var v = minNode;; v = v.right)
                {
                    
                }
            }
        }

        public class Node<T, V>
        {
            public T key;

            public Node<T, V> parent;

            public Node<T, V> child;

            public Node<T, V> right;

            public Node<T, V> left;

            public int degree;

            public bool mark;

            public V data;

            public Node(T key, V value)
            {
                this.key = key;
                this.data = value;
            }
        }

        public int CompareTo(T other)
        {
            throw new NotImplementedException();
        }
    }
}
