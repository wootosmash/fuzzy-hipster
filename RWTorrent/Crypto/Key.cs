/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 12/01/2015
 * Time: 8:51 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace FuzzyHipster.Crypto
{
  /// <summary>
  /// Description of Key.
  /// </summary>
  [Serializable()]
  public abstract class Key 
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    
    public Key()
    {
    }

    public abstract byte[] Encrypt( byte[] plaintext );
    public abstract byte[] Decrypt( byte[] cyphertext );
    
  }
  
  [Serializable()]
  public class AsymmetricKey : Key
  {
    public string PrivateString { get; set; }
    public string PublicString { get; set; }
    
    public bool IsPrivate { get { return String.IsNullOrWhiteSpace(PrivateString); }}
    
    public override byte[] Encrypt( byte[] plaintext )
    {
      using (var rsa = new RSACryptoServiceProvider(1024))
      {
        try
        {
          rsa.FromXmlString(PublicString);
          return rsa.Encrypt(plaintext, true);
        }
        finally
        {
          rsa.PersistKeyInCsp = false;
        }
      }
    }
    
    public override byte[] Decrypt( byte[] cyphertext )
    {
      if ( String.IsNullOrWhiteSpace(PrivateString))
        throw new Exception("This is key does not contain the private parameters to be able to decrypt this message");
      
      using (var rsa = new RSACryptoServiceProvider(1024))
      {
        try
        {
          rsa.FromXmlString(PrivateString);
          return rsa.Decrypt(cyphertext, true);
        }
        finally
        {
          rsa.PersistKeyInCsp = false;
        }
      }
    }
    
    public static AsymmetricKey Create()
    {
      using (var rsa = new RSACryptoServiceProvider(1024))
      {
        try
        {
          var k = new AsymmetricKey();
          k.PrivateString = rsa.ToXmlString(true);
          k.PublicString = rsa.ToXmlString(false);
          return k;
        }
        finally
        {
          rsa.PersistKeyInCsp = false;
        }
      }
    }
    
    public AsymmetricKey GetPublicKey()
    {
      var k = new AsymmetricKey();
      k.PublicString = PublicString;
      return k;
    }
  }
  
  [Serializable()]
  public class SymmetricKey : Key
  {
    public byte[] Key { get; set; }
    public byte[] Vector { get; set; }

    private static SymmetricAlgorithm _cryptoService = new AesCryptoServiceProvider();

    public override byte[] Encrypt(byte[] plaintext)
    {
      return Transform(plaintext, _cryptoService.CreateEncryptor(Key, Vector));
    }

    public override byte[] Decrypt(byte[] cyphertext)
    {
      return Transform(cyphertext, _cryptoService.CreateDecryptor(Key, Vector));
    }

    private byte[] Transform(byte[] buffer, ICryptoTransform cryptoTransform)
    {
      using(var stream = new MemoryStream())
      {
        using (var cryptoStream = new CryptoStream(stream, cryptoTransform, CryptoStreamMode.Write))
        {

          cryptoStream.Write(buffer, 0, buffer.Length);
          cryptoStream.FlushFinalBlock();

          return stream.ToArray();
        }
      }
    }
    
    public static SymmetricKey Create()
    {
      _cryptoService.GenerateKey();
      
      var k = new SymmetricKey();
      k.Key = _cryptoService.Key;
      k.Vector = _cryptoService.IV;
      
      return k;
    }
    
    public override string ToString()
    {
      return string.Format("[SymmetricKey Key={0}, Vector={1}]", Key, Vector);
    }

  }
}
