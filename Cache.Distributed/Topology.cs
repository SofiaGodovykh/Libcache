namespace Kontur.Cache.Distributed
{
    using System;

    public class Topology
    {
        private readonly int minToken;
        private readonly int maxToken;
        private readonly RedisEndPoint endPoint;

        public Topology(int minToken, int maxToken, RedisEndPoint endPoint)
        {
            if (minToken > maxToken)
            {
                throw new ArgumentException("MinToken must be greater then maxToken", "minToken");
            }

            if (endPoint == null)
            {
                throw new ArgumentNullException("endPoint");
            }

            this.minToken = minToken;
            this.maxToken = maxToken;
            this.endPoint = endPoint;
        }

        public RedisEndPoint EndPoint
        {
            get { return endPoint; }
        }

        public int MinToken
        {
            get { return minToken; }
        }

        public int MaxToken
        {
            get { return maxToken; }
        }

        public bool Contains(int token)
        {
            return token >= minToken && token < maxToken;
        }
    }
}