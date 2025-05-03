using System;

namespace kneedeepio.lrucache
{
    [Serializable]
    public class ItemRemovalFailedException : Exception
    {
        public ItemRemovalFailedException() : base() { }
        public ItemRemovalFailedException(string message) : base(message) { }
        public ItemRemovalFailedException(string message, Exception inner) : base(message, inner) { }
    }
}
