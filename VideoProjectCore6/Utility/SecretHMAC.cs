using System.Security.Cryptography;
using System.Text;

namespace VideoProjectCore6.Utility;

public class SecretHMAC
{
    private static readonly string _Key = "LN+hgbQugb5OjFZOkBz1ew==";
    public static string Sign(string text, string key)
    {
        HMACSHA256 hmacObj = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var signature = hmacObj.ComputeHash(Encoding.UTF8.GetBytes(text));
        var encodedSignature = Convert.ToBase64String(signature);
        return encodedSignature;

    }

    public static string Base64Encode(string plainText)
    {

        byte[] iv = new byte[16];
        byte[] array;

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(_Key);
            aes.IV = iv;            

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                    {
                        streamWriter.Write(plainText);
                    }

                    array = memoryStream.ToArray();
                }
            }
        }

        return Convert.ToBase64String(array);


    }

    public static string Base64Decode(string base64EncodedData)
    {

        byte[] iv = new byte[16];
        byte[] buffer = Convert.FromBase64String(base64EncodedData);

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(_Key);
            aes.IV = iv;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream(buffer))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader streamReader = new StreamReader(cryptoStream))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
        }
    }
}


