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
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace FuzzyHipster.Crypto
{
  /// <summary>
  /// Description of Key.
  /// </summary>
  [XmlInclude(typeof(AsymmetricKey))]
  [XmlInclude(typeof(SymmetricKey))]
  [Serializable()]
  public abstract class Key : IEquatable<Key>
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    
    public Key()
    {
      Id = Guid.NewGuid();
    }

    public abstract byte[] Encrypt( byte[] plaintext, int length );
    public abstract byte[] Decrypt( byte[] cyphertext, int length );
    
    #region IEquatable implementation
    public bool Equals(Key other)
    {
      return other.Id == Id;
    }
    #endregion
  }
  
  [Serializable()]
  public class AsymmetricKey : Key
  {
    public string PrivateString { get; set; }
    public string PublicString { get; set; }
    
    public bool IsPrivate { get { return String.IsNullOrWhiteSpace(PrivateString); }}
    
    public AsymmetricKey()
    {
    }
    
    public override byte[] Encrypt( byte[] plaintext, int length )
    {
      using (var rsa = new RSACryptoServiceProvider())
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
    
    public override byte[] Decrypt( byte[] cyphertext, int length )
    {
      if ( String.IsNullOrWhiteSpace(PrivateString))
        throw new Exception("This is key does not contain the private parameters to be able to decrypt this message");
      
      using (var rsa = new RSACryptoServiceProvider())
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
    
    public byte[] SignData( object obj )
    {
      if ( String.IsNullOrWhiteSpace(PrivateString))
        throw new Exception("This is key does not contain the private parameters to be able to sign this message");
      
      using (var rsa = new RSACryptoServiceProvider(1024))
      {
        try
        {
          rsa.FromXmlString(PrivateString);
          
          var serializer = new BinaryFormatter();
          using ( var stream = new MemoryStream())
          {
            serializer.Serialize(stream, obj);
            return rsa.SignData(stream.GetBuffer(), new SHA512CryptoServiceProvider());
          }
        }
        finally
        {
          rsa.PersistKeyInCsp = false;
        }
      }
    }
    
    public bool VerifySignature( byte[] signature, object obj )
    {
      if ( String.IsNullOrWhiteSpace(PublicString))
        throw new Exception("This is key does not contain the public parameters to be able to verify this signature");
      
      using (var rsa = new RSACryptoServiceProvider(1024))
      {
        try
        {
          rsa.FromXmlString(PublicString);
          
          var serializer = new BinaryFormatter();
          using ( var stream = new MemoryStream())
          {
            serializer.Serialize(stream, obj);
            return rsa.VerifyData(stream.GetBuffer(), new SHA512CryptoServiceProvider(), signature);
          }
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

    public override byte[] Encrypt(byte[] plaintext, int length)
    {
      _cryptoService.Padding = PaddingMode.Zeros;
      _cryptoService.BlockSize = 128;
      _cryptoService.Mode = CipherMode.ECB;
      return Transform(plaintext, length, _cryptoService.CreateEncryptor(Key, Vector));
    }

    public override byte[] Decrypt(byte[] cyphertext, int length)
    {
      _cryptoService.Padding = PaddingMode.Zeros;
      _cryptoService.BlockSize = 128;
      _cryptoService.Mode = CipherMode.ECB;
      
      return Transform(cyphertext, length, _cryptoService.CreateDecryptor(Key, Vector));
    }

    private byte[] Transform(byte[] buffer, int length, ICryptoTransform cryptoTransform)
    {
      using(var stream = new MemoryStream())
      {
        using (var cryptoStream = new CryptoStream(stream, cryptoTransform, CryptoStreamMode.Write))
        {

          cryptoStream.Write(buffer, 0, length);
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
