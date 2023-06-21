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
        // throw new NotImplementedException();
    }
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
                                                  //DBNullFormatter.Instance, // This is necessary for the null values
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