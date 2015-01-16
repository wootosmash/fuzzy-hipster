/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 12/01/2015
 * Time: 8:51 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace FuzzyHipster.Crypto
{
  /// <summary>
  /// Description of Key.
  /// </summary>
  public class Key : IEquatable<Key>
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    
    public Key()
    {
    }

    #region IEquatable implementation

    public bool Equals(Key other)
    {
      return Id == other.Id;
    }

    #endregion
    
    public override string ToString()
    {
      return string.Format("[Key Id={0}, Name={1}, Description={2}]", Id, Name, Description);
    }

  }
}
