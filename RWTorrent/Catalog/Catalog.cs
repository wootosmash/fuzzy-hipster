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
using FuzzyHipster.Crypto;
namespace FuzzyHipster.Catalog
{

  
  public class Catalog : CatalogItem
  {    
    public string Namespace { get; set; }

    public string Description { get; set; }

    public string BasePath { get; set; }
    
    public long LastUpdated { get; set; }

    [XmlIgnore()]
    public ChannelCollection Channels {
      get;
      set;
    }
    
    [XmlIgnore()]
    public KeyCollection Keys {
      get; set; 
    }

    public event EventHandler<GenericEventArgs<FileWad>> NotifyFileWad;
    protected virtual void OnNotifyFileWad(GenericEventArgs<FileWad> e)
    {
      var handler = NotifyFileWad;
      if (handler != null)
        handler(this, e);
    }

    public event EventHandler<GenericEventArgs<Channel>> NotifyChannel;
    protected virtual void OnNotifyChannel(GenericEventArgs<Channel> e)
    {
      var handler = NotifyChannel;
      if (handler != null)
        handler(this, e);
    }

    public event EventHandler<GenericEventArgs<Key>> NotifyKey;
    protected virtual void OnNotifyKey(GenericEventArgs<Key> e)
    {
      var handler = NotifyKey;
      if (handler != null)
        handler(this, e);
    }
    
    public Catalog()
    {
      Id = Guid.NewGuid();
      Channels = new ChannelCollection();
      Keys = new KeyCollection();
    }


    public static Catalog Load(string basePath)
    {
      string catalogEndPointPath = Path.Combine(basePath, @"Catalog\Catalog.xml");
      Catalog catalog = null;
      
      if ( !Directory.Exists(Path.Combine(basePath, @"Catalog")) || !File.Exists(catalogEndPointPath) )
      {
        catalog = new Catalog();
        catalog.BasePath = basePath;
        catalog.Save(); // cleans up the bogus directory
      }
      else
      {
        var serialiser = new XmlSerializer(typeof(Catalog));
        using (var reader = new StreamReader( catalogEndPointPath ))
          catalog = (Catalog)serialiser.Deserialize(reader);

        catalog.BasePath = basePath;        
        catalog.Keys = KeyCollection.Load(catalog.BasePath);
        
        string channelsPath = Path.Combine(basePath, @"Catalog\Channels");
        
        if ( !Directory.Exists(channelsPath ))
          Directory.CreateDirectory(channelsPath);

        var serialiserChannels = new XmlSerializer(typeof(Channel));
        foreach (string dir in Directory.GetDirectories(channelsPath))
        {
          Channel channel = null;
          using (var reader = new StreamReader(string.Format(@"{0}\Index.xml", dir)))
          {
            channel = (Channel)serialiserChannels.Deserialize(reader);
            channel.Validate();
            catalog.Channels.Add(channel);
          }
          
          foreach( string file in Directory.GetFiles(Path.Combine(basePath, string.Format(@"Catalog\Channels\{0}\",channel.Id))))
          {
            if ( !file.EndsWith("Index.xml"))
            {
              var wad = FileWad.Load( file);
              wad.Validate();
              channel.Wads.Add(wad);
            }
          }
        }
      }
      
      return catalog;
    }

    public void Save()
    {
      string catalogEndPointPath = Path.Combine(BasePath, @"Catalog\Catalog.xml");

      if ( !Directory.Exists(Path.Combine(BasePath, @"Catalog")))
        Directory.CreateDirectory(Path.Combine(BasePath, @"Catalog"));
      
      if ( !Directory.Exists(Path.Combine(BasePath, @"Catalog\Channels\")))
        Directory.CreateDirectory(Path.Combine(BasePath, @"Catalog\Channels\"));
      
      var serialiser = new XmlSerializer(typeof(Catalog));
      using (var writer = new StreamWriter(catalogEndPointPath))
        serialiser.Serialize(writer, this);
      
      foreach (var channel in Channels)
        channel.Save();
    }
    
    public FileWad[] GetFileWadsByFileHash( byte[] hash )
    {
      var wads = new List<FileWad>();
      foreach( var s in Channels )
        foreach( var wad in s.Wads )
          foreach( var file in wad.Files )
            if ( Hash.Compare( file.Hash, hash ))
              wads.Add( wad );
      return wads.ToArray();
    }
    
    public FileWad[] GetFileWads( string searchText )
    {
      return null;
    }

    public FileWad GetFileWad(Guid id)
    {
      foreach (var channel in Channels)
        foreach (var wad in channel.Wads)
          if (wad.Id == id)
            return wad;
      
      return null;
    }
    
    public FileWad AddFileWad( FileWad wad )
    {
      var channel = Channels.Find( x => x.Id == wad.ChannelId );
      
      if ( channel == null )
        throw new Exception("Wad has no associated channel");
      
      if ( channel.Wads == null )
        channel.Wads = new List<FileWad>();
      
      // wad doesn't exist
      if ( channel.Wads.Find(x => x.Id == wad.Id) == null )
      {
        channel.Wads.Add(wad);
        wad.Save();
      } 

      UpdateLastUpdated( wad.LastUpdated );
      OnNotifyFileWad( new GenericEventArgs<FileWad>(wad));
      return wad;
    }
    
    public Channel AddChannel( Channel channel )
    {
      if ( Channels[channel.Id] == null )
  			Channels.Add(channel);
			channel.Save();
      
			UpdateLastUpdated(channel.LastUpdated);
      OnNotifyChannel( new GenericEventArgs<Channel>(channel));
      return channel;
    }
    
    public Key AddKey( Key key )
    {
      Keys.Add(key);
      Keys.Save();
      OnNotifyKey(new GenericEventArgs<Key>(key));
      return key;
    }
    
    public override void Validate()
    {
    }
    
    public override string ToString()
    {
      return string.Format("[Catalog Namespace={0}, Description={1}, BasePath={2}, LastUpdated={3}, Channels.Count={4}]", 
                           Namespace, Description, BasePath, LastUpdated, Channels.Count);
    }

  }

}



