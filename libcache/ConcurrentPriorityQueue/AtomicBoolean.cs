namespace Kontur.Cache.ConcurrentPriorityQueue
{
    using System.Threading;

    public class AtomicBoolean
    {
        private const int VALUE_TRUE = 1;
        private const int VALUE_FALSE = 0;

        private int _currentValue;

        public AtomicBoolean(bool initialValue)
        {
            _currentValue = BoolToInt(initialValue);
        }

        public bool Value
        {
            get { return Thread.VolatileRead(ref _currentValue) == VALUE_TRUE; }
        }

        public bool SetValue(bool newValue)
        {
            return Interlocked.Exchange(ref _currentValue, BoolToInt(newValue)) == VALUE_TRUE;
        }

        public bool CompareAndSet(bool expectedValue, bool newValue)
        {
            int expectedVal = BoolToInt(expectedValue);
            int newVal = BoolToInt(newValue);
            return Interlocked.CompareExchange(ref _currentValue, newVal, expectedVal) == expectedVal;
        }

        private static int BoolToInt(bool value)
        {
            return value ? VALUE_TRUE : VALUE_FALSE;
        }
    }
}