using System.Text;

namespace MsaasBackend
{
    public class JwtOptions
    {
        public string SigningKey { get; set; }

        public double ExpiresIn { get; set; } = 7;

        public byte[] SigningKeyData => Encoding.UTF8.GetBytes(SigningKey);
    }
}