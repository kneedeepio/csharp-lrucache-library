// Doc Header Stuff here.

using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace kneedeepio.lrucache
{
    public class LruCache<T>
    {
        private readonly ILogger? _logger;

        private bool _outOfSync = false;

        private readonly uint _capacity;

        public uint Capacity
        {
            get => _capacity;
        }

        //private readonly List<LruCacheItem<T>> _cacheItems;
        private readonly ConcurrentQueue<LruCacheItem<T>> _cacheQueue;
        private readonly ConcurrentDictionary<string, LruCacheItem<T>> _cacheDictionary;

        // FIXME: Figure out how to store callback signatures (delegates?)
        // FIXME: Should there be a max size and max item count?
        // FIXME: There's probably a more efficient way of storing the data than a list.

        public LruCache(uint capacity = 0, ILogger? logger = null)
        {
            _logger = logger;
            _capacity = capacity;
            //_cacheItems = new List<LruCacheItem<T>>();
            _cacheQueue = new ConcurrentQueue<LruCacheItem<T>>();
            _cacheDictionary = new ConcurrentDictionary<string, LruCacheItem<T>>();
        }

        private uint GetTotalSize()
        {
            // FIXME: There's probably a more efficient way of adding all the sizes of all the items.
            uint itemSizeTotal = 0;
            foreach (LruCacheItem<T> cacheItem in _cacheQueue.ToList())
            {
                itemSizeTotal += cacheItem.LruSize;
            }
            _logger?.LogDebug("Total size of items in LRUCache: size {itemSizeTotal}", itemSizeTotal);
            return itemSizeTotal;
        }

        private void MakeSpace(uint newSize)
        {
            // Check if the item size will fit in the capacity of the LRUCache
            if (newSize > _capacity)
            {
                _logger?.LogWarning("Item too large to fit in LRUCache: size {size}, capacity {capacity}", newSize, _capacity);
                throw new NotEnoughCapacityException("Item too large to fit in LRUCache");
            }

            // Calc the current size of the LRUCache (sum of item sizes)
            uint itemSizeTotal = GetTotalSize();

            // Check if entries need to be cleared to make room for the new item
            while (newSize > (_capacity - itemSizeTotal))
            {
                // Remove from queue
                if (_cacheQueue.TryDequeue(out LruCacheItem<T>? removedItem))
                {
                    itemSizeTotal -= removedItem.LruSize;
                    _logger?.LogDebug("Removing item to make room: (size {size}, new total {total}) {item}", removedItem.LruSize, itemSizeTotal, removedItem);
                    if (_cacheDictionary.TryRemove(removedItem.LruKey, out LruCacheItem<T>? rItem))
                    {
                        // Just checking for more error conditions, 'cause why not?
                        if (removedItem != rItem)
                        {
                            throw new InternalMismatchException("LRUCache internal mis-match.");
                        }
                    }
                }
                else
                {
                    _logger?.LogError("Failed to remove item from LRUCache.");
                    throw new ItemRemovalFailedException("Failed to remove item from LRUCache.");
                }
            }
        }

        public void AddItem(string key, T item, uint size = 1)
        {
            if (_outOfSync)
            {
                // FIXME: Should this lockout happen?  Or should this be handled outside of the LRUCache?
                _logger?.LogError("LRUCache internals out of sync.  Do not trust LRUCache!");
                return;
            }

            _logger?.LogDebug("Adding item to cache: (size {size}) {item}", size, item);

            // Make space for the new item
            try
            {
                MakeSpace(size);

                // Add the item
                LruCacheItem<T> newCacheItem = new LruCacheItem<T>(key, item, size);
                if (_cacheDictionary.TryAdd(key, newCacheItem))
                {
                    _cacheQueue.Enqueue(newCacheItem);
                }
                else
                {
                    // FIXME: Throw exception here?
                    _logger?.LogWarning("Key collision, failed to add item: (size {size}), {key}: {item}", size, key, item);
                }

                _logger?.LogDebug("Added the item.");
            }
            catch (NotEnoughCapacityException ex)
            {

            }
            catch (InternalMismatchException ex)
            {
                _outOfSync = true;
                _logger?.LogError("LRUCache internal mismatch.  Do not trust cache!");
            }
            catch (ItemRemovalFailedException ex)
            {

            }


        }
    }
}
