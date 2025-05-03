using System;

namespace kneedeepio.lrucache
{
    [Serializable]
    public class NotEnoughCapacityException : Exception
    {
        public NotEnoughCapacityException() : base() { }
        public NotEnoughCapacityException(string message) : base(message) { }
        public NotEnoughCapacityException(string message, Exception inner) : base(message, inner) { }
    }
}
