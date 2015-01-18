/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 18/01/2015
 * Time: 7:59 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text;

namespace FuzzyHipster
{
  /// <summary>
  /// Description of Utility.
  /// </summary>
  public class Utility
  {
    public Utility()
    {
    }
    
    public static string ByteArrayToHexString(byte[] ba)
    {
      var hex = new StringBuilder(ba.Length * 2);
      foreach (byte b in ba)
        hex.AppendFormat("{0:x2}", b);
      return hex.ToString();
    }
    
  }
}
