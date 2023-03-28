using System.Runtime.CompilerServices;
using System;
using EFCoreSecondLevelCacheInterceptor;
using Infinispan.Hotrod.Core;

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
        throw new NotImplementedException();
    }

    public EFCachedData GetValue(EFCacheKey cacheKey, EFCachePolicy cachePolicy)
    {
        throw new NotImplementedException();
    }

    public void InsertValue(EFCacheKey cacheKey, EFCachedData value, EFCachePolicy cachePolicy)
    {
        throw new NotImplementedException();
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
    public override byte[] marshall(EFCachedData t)
    {
        throw new NotImplementedException();
    }

    public override EFCachedData unmarshall(byte[] buff)
    {
        throw new NotImplementedException();
    }
}