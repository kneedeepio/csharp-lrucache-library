// Doc Header Stuff here.

using System;

namespace kneedeepio.lrucache
{
    public class LruCacheItem<T>
    {
        // Size can be overridden if capacity is based on file size or similar.
        private readonly uint _lrusize;

        public uint LruSize
        {
            get => _lrusize;
        }

        private readonly string _lrukey;

        public string LruKey
        {
            get => _lrukey;
        }

        private readonly T _lruitem;

        public T LruItem
        {
            get => _lruitem;
        }

        public LruCacheItem(string key, T item, uint size = 1)
        {
            _lrukey = key;
            _lruitem = item;
            _lrusize = size;
        }
    }
}
