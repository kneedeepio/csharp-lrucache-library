using System;

namespace kneedeepio.lrucache
{
    [Serializable]
    public class InternalMismatchException : Exception
    {
        public InternalMismatchException() : base() { }
        public InternalMismatchException(string message) : base(message) { }
        public InternalMismatchException(string message, Exception inner) : base(message, inner) { }
    }
}
