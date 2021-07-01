namespace MsaasBackend.Helpers
{
    public static class Constants
    {
        public static class CacheKey
        {
            public const string GetHospitalsCacheKey = "Hospitals";

            public static string GetHospitalCacheKey(int hospitalId)
            {
                return $"Hospitals/{hospitalId}";
            }

        }   
    }
}