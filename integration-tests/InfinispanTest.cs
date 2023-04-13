namespace integration_tests;
using EFCoreSecondLevelCacheInterceptor;
// RUN THIS WITH A RUNNING INFINISPAN SERVER on localhost:11222
// WITHOUT AUTHENTICATION
public class MarshallersTest
{
    [Fact]
    public void TestPutGet()
    {
        var service = new EFInfinispanCacheServiceProvider();
        var key1 = new EFCacheKey(new HashSet<string> { "entity1", "entity2" })
                   {
                       KeyHash = "EF_key1",
                   };
        var data = new EFCachedData() { Scalar = "value1", NonQuery=42, IsNull=true };
        service.InsertValue(key1,data,null);
        var cacheData = service.GetValue(key1,null);
        Assert.Equivalent(data, cacheData);
    }
    [Fact]
    public void TestClear()
    {
        var service = new EFInfinispanCacheServiceProvider();
        var key1 = new EFCacheKey(new HashSet<string> { "entity1", "entity2" })
                   {
                       KeyHash = "EF_key1",
                   };
        var data = new EFCachedData() { Scalar = "value1", NonQuery=42, IsNull=true };
        service.InsertValue(key1,data,null);
        service.ClearAllCachedEntries();
        var cacheData = service.GetValue(key1,null);
        Assert.Null(cacheData);
    }
}