using Xunit;

namespace MsaasBackend.Tests.Utils
{
    public static class AssertExtensions
    {
        public static void ContainsDeeply(object expected, object actual)
        {
            foreach (var p in actual.GetType().GetProperties())
            {
                var expectedProp = expected.GetType().GetProperty(p.Name);
                if (expectedProp == null) continue;
                Assert.Equal(expectedProp.GetValue(expected), p.GetValue(actual));
            }
        }
    }
}