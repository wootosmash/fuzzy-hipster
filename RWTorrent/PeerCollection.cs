

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Xml.Serialization;
using System.Linq;

namespace FuzzyHipster
{
  public class PeerCollection : List<Peer>
  {
    public Peer FindBySocket( Socket socket )
    {
      return this.FirstOrDefault(x => x.Socket == socket);
    }
    
    public Peer GetRandom()
    {
      if ( Count == 0 )
        return null;
      
      int index = RWTorrent.Singleton.Random.Next(0, Count);
      return this[index];
    }
    
    public void RefreshPeer(Peer peer)
    {
      Peer myPeer = null;
      if ( peer.Socket != null )
      {
        myPeer = FindBySocket(peer.Socket);
        if ( myPeer != null )
          myPeer.UpdateFromCopy(peer);
        else
          Add(peer);
      }
      else if (Contains(peer))
      {
        myPeer = this.Find( x => x.Id == peer.Id);
        myPeer.UpdateFromCopy(peer);
      }
      else
        Add(peer);
      
      Save();
    }

    public void Add(Peer peer)
    {
      Add(peer);
    }
    
    public static PeerCollection Load( string basePath )
    {
      var col = new PeerCollection();
      string peersPath = Path.Combine(basePath, @"Catalog\");

      if ( !Directory.Exists(peersPath))
        Directory.CreateDirectory(peersPath);

      if ( File.Exists(peersPath + "Peers.xml"))
      {
        var serialiser = new XmlSerializer(typeof(List<Peer>));
        using (var reader = new StreamReader(peersPath + "Peers.xml"))
        {
          var list = serialiser.Deserialize(reader) as List<Peer>;
          foreach( var peer in list )
            if ( !col.Contains(peer))
              col.Add(peer);
        }
      }

      return col;
    }
    
    
    public void Save()
    {
      lock( this)
      {
        string basePath = RWTorrent.Singleton.Catalog.BasePath;
        string peersPath = Path.Combine(basePath, @"Catalog\");
        
        if ( !Directory.Exists(peersPath))
          Directory.CreateDirectory(peersPath);
        
        var list = new List<Peer>();
        
        foreach( var peer in this )
          list.Add(peer);
        
        var serialiser = new XmlSerializer(typeof(List<Peer>));
        using (var writer = new StreamWriter(peersPath + "Peers.xml"))
          serialiser.Serialize(writer, list);
      }
    }
    
    public Peer[] ToArray()
    {
      List<Peer> peers = new List<Peer>();
      
      while ( peers.Count < Count )
        peers.Add(this[peers.Count]);
      
      return peers.ToArray();
    }
    
    public void ResetConnectionAttempts()
    {
      foreach( var peer in this )
        peer.ResetConnectionAttempts();
    }
    
  }
}


