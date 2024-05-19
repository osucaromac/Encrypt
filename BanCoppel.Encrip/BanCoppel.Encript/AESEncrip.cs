using BanCoppel.Util;
using System.Security.Cryptography;
using System.Text;


namespace BanCoppel.AesEncript
{
    public static class AESEncrip
    {
        public static void Main()
        {
        }
            public static byte[] EncryptFile(byte[] aesKey, byte[] ivParams, byte[] content)
        {
            byte[] encrypted;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = aesKey;
                aesAlg.IV = ivParams;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(Encoding.ASCII.GetString(content));
                        }
                        encrypted = ms.ToArray();
                    }
                }
            }
            return FileUtil.combineBytes(ivParams, encrypted);
        }
    }
}

