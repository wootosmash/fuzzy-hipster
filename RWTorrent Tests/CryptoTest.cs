/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 12/01/2015
 * Time: 8:51 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;

namespace FuzzyHipster.Tests
{
  [TestFixture]
  public class CryptoTest
  {
    
    
    [Test]
    public void SignAndVerifyTest()
    {
      string plaintext = "ROFL MY LOFL";
      string cyphertext = "";
      
      using (var rsa = new RSACryptoServiceProvider(1024))
      {
        try
        {
          var encoder = new UTF8Encoding();
          byte[] encbytes = rsa.Encrypt(encoder.GetBytes(plaintext), true);
          
          Console.WriteLine(encoder.GetString(encbytes));
          
          byte[] decbytes = rsa.Decrypt(encbytes, true);
          
          Console.WriteLine(encoder.GetString(decbytes));
          
          Console.WriteLine(rsa.ToXmlString(false));
          Console.WriteLine(rsa.ToXmlString(true));
        }
        finally
        {
          rsa.PersistKeyInCsp = false;
        }
      }
      
      //      CryptoTest crypto = new CryptoTest();
      //      RSAParameters privateKey = crypto.GenerateKeys("simlanghoff@gmail.com");
//
      //      const string PlainText = "This is really sent by me, really!";
//
      //      RSAParameters publicKey = crypto.GetPublicKey("simlanghoff@gmail.com");
//
      //      string encryptedText = Cryptograph.Encrypt(PlainText, publicKey);
//
      //      Console.WriteLine("This is the encrypted Text:" + "\n " + encryptedText);
//
      //      string decryptedText = Cryptograph.Decrypt(encryptedText, privateKey);
//
      //      Console.WriteLine("This is the decrypted text: " + decryptedText);
//
      //      string messageToSign = encryptedText;
//
      //      string signedMessage = Cryptograph.SignData(messageToSign, privateKey);
//
      //      //// Is this message really, really, REALLY sent by me?
      //      bool success = Cryptograph.VerifyData(messageToSign, signedMessage, publicKey);
//
      //      Console.WriteLine("Is this message really, really, REALLY sent by me? " + success);
      
    }
    
    [Test]
    public void VerifyTest()
    {
      
    }
    
    public static string SignData(string message, RSAParameters privateKey)
    {
      // The array to store the signed message in bytes
      byte[] signedBytes;
      using (var rsa = new RSACryptoServiceProvider())
      {
        // Write the message to a byte array using UTF8 as the encoding.
        var encoder = new UTF8Encoding();
        byte[] originalData = encoder.GetBytes(message);

        try
        {
          // Import the private key used for signing the message
          rsa.ImportParameters(privateKey);

          // Sign the data, using SHA512 as the hashing algorithm
          signedBytes = rsa.SignData(originalData, CryptoConfig.MapNameToOID("SHA512"));
        }
        catch (CryptographicException e)
        {
          Console.WriteLine(e.Message);
          return null;
        }
        finally
        {
          // Set the keycontainer to be cleared when rsa is garbage collected.
          rsa.PersistKeyInCsp = false;
        }
      }
      // Convert the a base64 string before returning
      return Convert.ToBase64String(signedBytes);
    }
    
    public static bool VerifyData(string originalMessage, string signedMessage, RSAParameters publicKey)
    {
      bool success = false;
      using (var rsa = new RSACryptoServiceProvider())
      {
        var encoder = new UTF8Encoding();
        byte[] bytesToVerify = encoder.GetBytes(originalMessage);
        byte[] signedBytes = Convert.FromBase64String(signedMessage);
        try
        {
          rsa.ImportParameters(publicKey);

          SHA512Managed Hash = new SHA512Managed();

          byte[] hashedData = Hash.ComputeHash(signedBytes);

          success = rsa.VerifyData(bytesToVerify, CryptoConfig.MapNameToOID("SHA512"), signedBytes);
        }
        catch (CryptographicException e)
        {
          Console.WriteLine(e.Message);
        }
        finally
        {
          rsa.PersistKeyInCsp = false;
        }
      }
      return success;
    }
  }
}
