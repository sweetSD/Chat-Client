using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

/// <summary>
/// Created by sweetSD. (with Singletone)
/// 
/// AES256을 사용하는 보안 스크립트입니다.
/// _securityKey와 _securityIv를 직접 설정하여 사용해주세요.
/// 
/// </summary>
public class SDSecurityManager : SDSingleton<SDSecurityManager>
{
    private static readonly string _securityKey = "gbirtgbutrhgb8rhg8ehg97g7u3h8ygh9843ghyu8erfhgvy8dfhgy8";
    private static readonly byte[] _securityIv = new byte[] { 45, 12, 89, 47, 32, 59, 12, 48, 65, 45, 12, 50, 94};

    private RijndaelManaged _aes = new RijndaelManaged();

    private void Awake()
    {
        // AES 256 세팅
        _aes.KeySize = 256;
        _aes.BlockSize = 128;
        _aes.Mode = CipherMode.CBC;
        _aes.Padding = PaddingMode.PKCS7;
        _aes.Key = Encoding.UTF8.GetBytes(_securityKey);
        _aes.IV = _securityIv;

        SetInstance(this);
    }

    /// <summary>
    /// 주어진 데이터를 AES256으로 암호화(Encrypt) 합니다.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public string Encrypt(string data)
    {
        var encrypt = _aes.CreateEncryptor(_aes.Key, _aes.IV);

        byte[] buffer = null;
        using(var ms = new MemoryStream())
        {
            using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
            {
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                cs.Write(dataBytes, 0, dataBytes.Length);
            }

            buffer = ms.ToArray();
        }
        return Encoding.UTF8.GetString(buffer);
    }

    /// <summary>
    /// 주어진 데이터를 AES256으로 복호화(Decrypt) 합니다.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public string Decrypt(string data)
    {
        var decrypt = _aes.CreateDecryptor(_aes.Key, _aes.IV);

        byte[] buffer = null;
        using (var ms = new MemoryStream())
        {
            using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
            {
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                cs.Write(dataBytes, 0, dataBytes.Length);
            }

            buffer = ms.ToArray();
        }
        return Encoding.UTF8.GetString(buffer);
    }
}
