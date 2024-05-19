using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;
using System.Text;


namespace BanCoppel.RSADencript
{
    public static class RSADecript
    {
   
    public static byte[] DecryptKey(byte[] privateKeyBytes, byte[] aesKeyBytes)
    {
        string stringPrivateKey = Encoding.ASCII.GetString(privateKeyBytes);
        var keyBytes = Convert.FromBase64String(stringPrivateKey);

        AsymmetricKeyParameter asymmetricKeyParameter = PrivateKeyFactory.CreateKey(keyBytes);
        RSAParameters rsaParameters = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)asymmetricKeyParameter);

        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        rsa.ImportParameters(rsaParameters);

        return rsa.Decrypt(aesKeyBytes, false);
    }

    public static RSACryptoServiceProvider ImportPrivateKey(string pem)
    {
        PemReader pr = new PemReader(new StringReader(pem));
        AsymmetricCipherKeyPair KeyPair = (AsymmetricCipherKeyPair)pr.ReadObject();
        RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)KeyPair.Private);

        RSACryptoServiceProvider csp = new RSACryptoServiceProvider();// cspParams);
        csp.ImportParameters(rsaParams);
        return csp;
    }
    }
}
