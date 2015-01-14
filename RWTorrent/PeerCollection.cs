

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Xml.Serialization;
using System.Linq;

namespace FuzzyHipster
{
  public class PeerCollection : SortedList<Guid, Peer>
  {
    public Peer FindBySocket( Socket socket )
    {
      return Values.FirstOrDefault(x => x.Socket == socket);
    }
    
    public void RefreshPeer(Peer peer)
    {
      Peer myPeer = null;
      if ( peer.Socket != null )
      {
        myPeer = FindBySocket(peer.Socket);
        myPeer.UpdateFromCopy(peer);
      }
      else if (ContainsKey(peer.Guid))
      {
        myPeer = this[peer.Guid];
        myPeer.UpdateFromCopy(peer);
      }
      else
        Add(peer.Guid, peer);
      
      Save();
    }

    public void Add(Peer peer)
    {
      Add(peer.Guid, peer);
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
            col.Add(peer);
        }
      }

      return col;
    }
    
    public void Save()
    {
      string basePath = RWTorrent.Singleton.Catalog.BasePath;
      string peersPath = Path.Combine(basePath, @"Catalog\");
      
      if ( !Directory.Exists(peersPath))
        Directory.CreateDirectory(peersPath);
      
      var list = new List<Peer>();
      
      foreach( var peer in Values )
        list.Add(peer);
      
      var serialiser = new XmlSerializer(typeof(List<Peer>));
      using (var writer = new StreamWriter(peersPath + "Peers.xml"))
        serialiser.Serialize(writer, list);
    }
    
    public void ResetConnectionAttempts()
    {
      foreach( var peer in Values )
        peer.ResetConnectionAttempts();
    }
    
  }
}


