namespace xunit_tests;
using EFCoreSecondLevelCacheInterceptor;
using System.Diagnostics.CodeAnalysis;
using System.Text;
public class MarshallersTest
{
    [Fact]
    public void Test1()
    {
        var marshaller = new EFCacheKeyMarshaller();
        var key1 = new EFCacheKey(new HashSet<string> { "entity1", "entity2" })
        {
            KeyHash = "EF_key1",
        };
        var bytes = marshaller.marshall(key1);
        var unmarshalledKey1 = marshaller.unmarshall(bytes);
        Assert.Equivalent(unmarshalledKey1, key1);

    }
    [Fact]
    public void TestEFCachedDataMarshaller()
    {
        var marshaller = new EFCachedDataMarshaller();
        var efCachePolicy = new EFCachePolicy().Timeout(TimeSpan.FromMinutes(10))
                                               .ExpirationMode(CacheExpirationMode.Absolute);
        var data = new EFCachedData() { Scalar = "value1", NonQuery = 42, IsNull = true };
        var bytes = marshaller.marshall(data);
        var unmarshalledData = marshaller.unmarshall(bytes);
        Assert.Equivalent(unmarshalledData, data);
    }
    [Fact]
    public void TestEFCachedDataMarshallerWithRows()
    {
        // I fear this will be complicate
        throw new MissingMethodException();
    }
}