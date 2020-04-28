﻿using System;
using System.Threading.Tasks;
using ArgonautCore.Maybe;

namespace SoraBot.Services.Cache
{
    public partial class CacheService
    {
        public Maybe<object> Get(string id)
        {
            _customCache.TryGetValue(id, out var item);
            if (item == null) return Maybe.Zero<object>();
            if (item.IsValid()) return Maybe.FromVal<object>(item);
            
            _customCache.TryRemove(id, out _);
            return Maybe.Zero<object>();
        }

        public Maybe<T> Get<T>(string id)
        {
            _customCache.TryGetValue(id, out var item);
            if (item == null) return Maybe.Zero<T>();
            if (item.IsValid()) return Maybe.FromVal<T>((T)item.Content);
            
            _customCache.TryRemove(id, out _);
            return Maybe.Zero<T>();
        }

        public Maybe<T> GetOrSetAndGet<T>(string id, Func<T> set, TimeSpan? ttl = null)
        {
            return Maybe.FromVal(this.GetOrSetAndGet(id, _customCache, set, ttl));
        }

        public async Task<Maybe<T>> GetOrSetAndGetAsync<T>(string id, Func<Task<T>> set, TimeSpan? ttl = null)
        {
            return Maybe.FromVal<T>(await GetOrSetAndGetAsync(id, _customCache, set, ttl).ConfigureAwait(false));
        }

        public async Task<Maybe<T>> TryGetOrSetAndGetAsync<T>(string id, Func<Task<T>> set, TimeSpan? ttl = null)
        {
            return await this.TryGetOrSetAndGetAsync(id, _customCache, set, ttl).ConfigureAwait(false);
        }

        public void Set(string id, object obj, TimeSpan? ttl = null)
        {
            var itemToStore = new CacheItem(obj, ttl.HasValue ? (DateTime?)DateTime.UtcNow.Add(ttl.Value) : null);
            _customCache.AddOrUpdate(id, itemToStore, ((key, cacheItem) => itemToStore));
        }

        public void AddOrUpdate(string id, CacheItem addItem, Func<string, CacheItem, CacheItem> updateFunc)
        {
            this._customCache.AddOrUpdate(id, addItem, updateFunc);
        }

        public Maybe<T> TryRemove<T>(string id)
        {
            _customCache.TryRemove(id, out var cacheItem);
            if (cacheItem == null) return Maybe.Zero<T>();
            if (!cacheItem.IsValid()) return Maybe.Zero<T>();
            return Maybe.FromVal((T) cacheItem.Content);}
    }
}