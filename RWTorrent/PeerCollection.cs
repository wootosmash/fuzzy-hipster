

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace RWTorrent
{
  public class PeerCollection : SortedList<Guid, Peer>
  {
    public void RefreshPeer(Peer peer)
    {
      if (ContainsKey(peer.Guid))
        this[peer.Guid] = peer;
      this.Add(peer.Guid, peer);
    }

    public void Add(Peer peer)
    {
      Add(peer.Guid, peer);
    }
    
    public void Save()
    {
      string basePath = RWTorrent.Singleton.Catalog.BasePath;
      string peersPath = Path.Combine(basePath, @"Catalog\Peers.xml");
      
      if ( !Directory.Exists(peersPath))
        Directory.CreateDirectory(peersPath);
      
      var serialiserStacks = new XmlSerializer(typeof(Peer));
      using (var writer = new StreamWriter(peersPath))
        serialiserStacks.Serialize(writer, this);
    }
    
  }
}


