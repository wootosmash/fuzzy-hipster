/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 10/01/2015
 * Time: 11:53 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Xml.Serialization;
namespace FuzzyHipster.Catalog
{
  [Serializable()]
  public class Stack : IEquatable<Stack>
  {
    public Guid Id {
      get;
      set;
    }

    public string Name {
      get;
      set;
    }

    public string Description {
      get;
      set;
    }

    public string PublicKey {
      get;
      set;
    }

    [XmlIgnore()]
    [NonSerialized()]
    List<FileWad> _wads;

    [XmlIgnore()]
    public List<FileWad> Wads {
      get {
        return _wads;
      }
      set {
        _wads = value;
      }
    }
    
    public Stack()
    {
      Id = Guid.NewGuid();
      Wads = new List<FileWad>();
    }
    
    /// <summary>
    /// Gets the wads after a certain recency number oldest to newest
    /// </summary>
    /// <param name="recency"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public FileWad[] GetWadsByRecency( long recency, int count )
    {
      int i = 0;
      var wads = new List<FileWad>();
      
      while( i < count && i < Wads.Count )
      {
        if ( wads[i].LastUpdate > recency )
          wads.Add( Wads[i] );
        i++;
      }
      
      return wads.ToArray();
    }
    
    public void RefreshWad( FileWad wad )
    {
      if ( Wads.Find(x => x.Id == wad.Id) == null )
        Wads.Add(wad);
      wad.Save();
    }
    
    public void Save()
    {
      string basePath = RWTorrent.Singleton.Catalog.BasePath;
      string stackPath = Path.Combine(basePath, string.Format(@"Catalog\Stacks\{0}\", Id));
      
      if ( !Directory.Exists(stackPath))
        Directory.CreateDirectory(stackPath);
      
      var serialiserStacks = new XmlSerializer(typeof(Stack));
      using (var writer = new StreamWriter(string.Format("{0}Index.xml", stackPath)))
        serialiserStacks.Serialize(writer, this);
      
      if ( Wads != null )
        foreach( var wad in Wads)
          wad.Save();
    }

    public override string ToString()
    {
      return string.Format("[Stack Wads={0}, Id={1}, Name={2}, Description={3}, PublicKey={4}]", _wads, Id, Name, Description, PublicKey);
    }

    #region IEquatable implementation
    public bool Equals(Stack other)
    {
      return other.Id == Id;
    }
    #endregion

    
  }
}



