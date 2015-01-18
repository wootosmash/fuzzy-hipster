/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 12/01/2015
 * Time: 8:51 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Security;

namespace FuzzyHipster.Crypto
{
  /// <summary>
  /// Description of Key.
  /// </summary>
  public abstract class Key 
  {
    public Key()
    {
    }

  }
  
  public class AsymetricKey : Key 
  {
    public SecureString RsaXmlString { get; set; }
  }
  
  public class SymetricKey : Key 
  {
    public SecureString XmlString { get; set; }
  }
}
