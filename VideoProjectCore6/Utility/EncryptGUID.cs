using System.Security.Cryptography;
using System.Text;

namespace VideoProjectCore6.Utility
{
    public class EncryptGUID
    {

        private Aes aes;

        public EncryptGUID(byte[] key)
        {
            aes = Aes.Create();
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;
            aes.Key = key;
        }

        public byte[] encryptUID(byte[] guid)
        {
            ICryptoTransform aesDecryptor = aes.CreateDecryptor();
            byte[] result = aesDecryptor.TransformFinalBlock(guid, 0, guid.Length);
            return result;
        }

        public static string ToHex(byte[] data)
        {
            StringBuilder hex = new StringBuilder(data.Length * 2);
            foreach (byte b in data)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}
