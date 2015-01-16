/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 16/01/2015
 * Time: 11:35 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Security.Cryptography;

namespace FuzzyHipster.Crypto
{
  /// <summary>
  /// Description of Hash.
  /// </summary>
  public class Hash
  {
    public Hash()
    {
    }
    
    public static byte[] GetHash( string filename )
    {
      using ( var md5 = MD5.Create())
      {
        using (var stream = File.OpenRead(filename))
        {
          return md5.ComputeHash(stream);
        }
      }
    }
    
    public static byte[] GetHash( byte[] data )
    {
      using ( var md5 = MD5.Create())
      {
        using ( var stream = new MemoryStream(data))
        {
          return md5.ComputeHash(stream);
        }
      }
    }
    
    public static bool Compare( byte[] hash1, byte[] hash2 )
    {
      if ( hash1.Length != hash2.Length )
        return false;
      
      for ( int i=0;i<hash1.Length;i++)
        if ( hash2[i] != hash1[i] )
          return false;
      
      return true;
    }
  }
}
