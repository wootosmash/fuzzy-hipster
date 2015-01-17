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
  public class Channel : CatalogItem, IEquatable<Channel>
  {
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
    
    public Channel()
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
        if ( wads[i].LastUpdated > recency )
          wads.Add( Wads[i] );
        i++;
      }
      
      return wads.ToArray();
    }
    
    public override void Validate()
    {
      
    }
    
    public void Save()
    {
      string basePath = MoustacheLayer.Singleton.Catalog.BasePath;
      string channelPath = Path.Combine(basePath, string.Format(@"Catalog\Channels\{0}\", Id));
      
      if ( !Directory.Exists(channelPath))
        Directory.CreateDirectory(channelPath);
      
      var serialiserChannels = new XmlSerializer(typeof(Channel));
      using (var writer = new StreamWriter(string.Format("{0}Index.xml", channelPath)))
        serialiserChannels.Serialize(writer, this);
      
      if ( Wads != null )
        foreach( var wad in Wads)
          wad.Save();
    }

    public override string ToString()
    {
      return string.Format("[Channel Wads={0}, Id={1}, Name={2}, Description={3}, PublicKey={4}]", _wads, Id, Name, Description, PublicKey);
    }

    #region IEquatable implementation
    public bool Equals(Channel other)
    {
      return other.Id == Id;
    }
    #endregion

    
  }
}



