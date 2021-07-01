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

            public static string GetDepartmentsCacheKey(int? hospitalId)
            {
                var cacheKey = "Departments";
                if (hospitalId.HasValue)
                {
                    cacheKey += $"?HospitalId={hospitalId}";
                }

                return cacheKey;
            }
            
            public static string GetDepartmentCacheKey(int departmentId)
            {
                return $"Departments/{departmentId}";
            }

        }   
    }
}