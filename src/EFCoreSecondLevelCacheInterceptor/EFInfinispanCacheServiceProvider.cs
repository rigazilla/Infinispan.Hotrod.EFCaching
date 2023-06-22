using System;
using EFCoreSecondLevelCacheInterceptor;
using Infinispan.Hotrod.Core;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using MessagePack;
using MessagePack.Resolvers;
using System.Collections.Generic;

/// <summary>
///     Using Infinispan as a cache service.
/// </summary>
public class EFInfinispanCacheServiceProvider : IEFCacheServiceProvider
{
    InfinispanDG dg = new InfinispanDG();
    Cache<EFCacheKey, EFCachedData> cache;
    Cache<string, HashSet<EFCacheKey>> depCache;
    public EFInfinispanCacheServiceProvider()
    {
        // Use a non-authenticated non-encrypted cluster;
        dg.AddHost("127.0.0.1", 11222);
        /// [Create a cluster object]
        /// [Create a cache]
        cache = dg.NewCache<EFCacheKey, EFCachedData>(new EFCacheKeyMarshaller(), new EFCachedDataMarshaller(), "default");
        depCache = dg.NewCache<string, HashSet<EFCacheKey>>(new StringMarshaller(), new HashSetEFCacheKeyMarshaller(), "dependencies");
    }
    public void ClearAllCachedEntries()
    {
        cache.Clear().Wait();
    }

    public EFCachedData GetValue(EFCacheKey cacheKey, EFCachePolicy cachePolicy)
    {
        var res = cache.Get(cacheKey).Result;
        Console.Out.WriteLine("=========== "+ (res != null ? "HIT" : "MISS")+ " ==> Getting from cache key:" + cacheKey.ToString());
        return res;
    }

    public void InsertValue(EFCacheKey cacheKey, EFCachedData value, EFCachePolicy cachePolicy)
    {
        foreach (var rootCacheKey in cacheKey.CacheDependencies)
        {
            bool replaced = false;
            while (!replaced)
            {
                var old = depCache.GetWithVersion(rootCacheKey).Result;

                if (old == null || old.Value == null)
                {
                    replaced = depCache.PutIfAbsent(rootCacheKey, new HashSet<EFCacheKey>() { cacheKey }).Result == null;
                    continue;
                }
                old.Value.Add(cacheKey);
                replaced = depCache.ReplaceWithVersion(rootCacheKey, old.Value, old.Version).Result;
            }
        }

        var lifeSpan = cachePolicy.CacheTimeout;
        var expTime = new ExpirationTime();
        expTime.Unit = TimeUnit.SECONDS;
        expTime.Value = cachePolicy.CacheTimeout.Seconds;
        Console.Out.WriteLine("=========> Putting cache key:" + cacheKey.ToString());
        cache.Put(cacheKey, value, expTime).Wait();
    }

    public void InvalidateCacheDependencies(EFCacheKey cacheKey)
    {
        if (cacheKey is null)
        {
            throw new ArgumentNullException(nameof(cacheKey));
        }

        Console.Out.WriteLine("==========> Invalidating for cacheKey: " + cacheKey.ToString());
        Console.Out.Write("==========> Invalidating root cache keys: ");
        foreach (var rootCacheKey in cacheKey.CacheDependencies)
        {
            Console.Out.Write(rootCacheKey.ToString()+", ");
            if (string.IsNullOrWhiteSpace(rootCacheKey))
            {
                continue;
            }

            var cachedValue = cache.Get(cacheKey).Result;
            var dependencyKeys = depCache.Get(rootCacheKey).Result;
            if (AreRootCacheKeysExpired(cachedValue, dependencyKeys))
            {
                // _logger.LogDebug(CacheableEventId.QueryResultInvalidated,
                //                  $"Invalidated all of the cache entries due to early expiration of a root cache key[{rootCacheKey}].");
                ClearAllCachedEntries();
                Console.Out.WriteLine("");
                return;
            }

            ClearDependencyValues(dependencyKeys);
            depCache.Remove(rootCacheKey);
        }
        Console.Out.WriteLine("");
    }

    private void ClearDependencyValues(HashSet<EFCacheKey> dependencyKeys)
    {
        if (dependencyKeys is null)
        {
            return;
        }

        foreach (var dependencyKey in dependencyKeys)
        {
            cache.Remove(dependencyKey);
        }
    }

    private static bool AreRootCacheKeysExpired(EFCachedData cachedValue, HashSet<EFCacheKey> dependencyKeys)
        => cachedValue is not null && dependencyKeys is null;
}

public class EFCacheKeyMarshaller : Marshaller<EFCacheKey>
{
    public static MessagePackSerializerOptions ops = MessagePackSerializerOptions.Standard.WithResolver(
            MessagePack.Resolvers.CompositeResolver.Create(
                                              new MessagePack.Formatters.IMessagePackFormatter[]
                                              {
                                                  DBNullFormatter.Instance, // This is necessary for the null values
                                 },
                                              new IFormatterResolver[]
                                              {
                                              NativeDateTimeResolver.Instance,
                                              ContractlessStandardResolver.Instance,
                                              StandardResolverAllowPrivate.Instance,
                                 }));
    public override byte[] marshall(EFCacheKey t)
    {
        return MessagePackSerializer.Serialize(t, ops);
    }

    public override EFCacheKey unmarshall(byte[] buff)
    {
        return MessagePackSerializer.Deserialize<EFCacheKey>(buff, ops);
    }
}

public class EFCachedDataMarshaller : Marshaller<EFCachedData>
{
    public static MessagePackSerializerOptions ops = MessagePackSerializerOptions.Standard.WithResolver(
            MessagePack.Resolvers.CompositeResolver.Create(
                                              new MessagePack.Formatters.IMessagePackFormatter[]
                                              {
                                                  DBNullFormatter.Instance, // This is necessary for the null values
                                 },
                                              new IFormatterResolver[]
                                              {
                                              NativeDateTimeResolver.Instance,
                                              ContractlessStandardResolver.Instance,
                                              StandardResolverAllowPrivate.Instance,
                                 }));
    public override byte[] marshall(EFCachedData t)
    {
        return MessagePackSerializer.Serialize(t, ops);
    }
    public override EFCachedData unmarshall(byte[] buff)
    {
        return MessagePackSerializer.Deserialize<EFCachedData>(buff, ops);
    }
}

public class HashSetEFCacheKeyMarshaller : Marshaller<HashSet<EFCacheKey>>
{
    public static MessagePackSerializerOptions ops = MessagePackSerializerOptions.Standard.WithResolver(
            MessagePack.Resolvers.CompositeResolver.Create(
                                              new MessagePack.Formatters.IMessagePackFormatter[]
                                              {
                                                  DBNullFormatter.Instance, // This is necessary for the null values
                                 },
                                              new IFormatterResolver[]
                                              {
                                              NativeDateTimeResolver.Instance,
                                              ContractlessStandardResolver.Instance,
                                              StandardResolverAllowPrivate.Instance,
                                 }));
    public override byte[] marshall(HashSet<EFCacheKey> t)
    {
        return MessagePackSerializer.Serialize(t, ops);
    }
    public override HashSet<EFCacheKey> unmarshall(byte[] buff)
    {
        return MessagePackSerializer.Deserialize<HashSet<EFCacheKey>>(buff, ops);
    }
}