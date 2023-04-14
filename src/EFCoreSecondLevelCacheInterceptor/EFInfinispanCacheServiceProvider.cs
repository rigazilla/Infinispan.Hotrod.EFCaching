using System;
using EFCoreSecondLevelCacheInterceptor;
using Infinispan.Hotrod.Core;
using System.Runtime.Serialization.Json;
using System.IO;

/// <summary>
///     Using Infinispan as a cache service.
/// </summary>
public class EFInfinispanCacheServiceProvider : IEFCacheServiceProvider
{
    InfinispanDG dg = new InfinispanDG();
    Cache<EFCacheKey, EFCachedData> cache;
    public EFInfinispanCacheServiceProvider()
    {
        // Use a non-authenticated non-encrypted cluster;
        dg.AddHost("127.0.0.1", 11222);
        /// [Create a cluster object]
        /// [Create a cache]
        cache = dg.NewCache<EFCacheKey, EFCachedData>(new EFCacheKeyMarshaller(), new EFCachedDataMarshaller(), "default");
    }
    public void ClearAllCachedEntries()
    {
        cache.Clear().Wait();
    }

    public EFCachedData GetValue(EFCacheKey cacheKey, EFCachePolicy cachePolicy)
    {
        return cache.Get(cacheKey).Result;
    }

    public void InsertValue(EFCacheKey cacheKey, EFCachedData value, EFCachePolicy cachePolicy)
    {
        cache.Put(cacheKey, value).Wait();
    }

    public void InvalidateCacheDependencies(EFCacheKey cacheKey)
    {
        throw new NotImplementedException();
    }
}

public class EFCacheKeyMarshaller : Marshaller<EFCacheKey>
{
    public override byte[] marshall(EFCacheKey t)
    {
        throw new NotImplementedException();
    }

    public override EFCacheKey unmarshall(byte[] buff)
    {
        throw new NotImplementedException();
    }
}

public class EFCachedDataMarshaller : Marshaller<EFCachedData>
{
    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(EFCachedData));
    public override byte[] marshall(EFCachedData t)
    {
        var ms = new MemoryStream();
        serializer.WriteObject(ms, t);
        return ms.ToArray();
    }
    public override EFCachedData unmarshall(byte[] buff)
    {
        var ms = new MemoryStream(buff);
        var obj = (EFCachedData)serializer.ReadObject(ms);
        return obj;
    }
}