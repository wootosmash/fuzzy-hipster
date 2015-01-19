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
using System.Timers;
using FuzzyHipster.Catalog;
using FuzzyHipster.Network;
namespace FuzzyHipster
{
  public class CatalogManagementMoustacheStrategy : MoustacheStrategy
  {
    public CatalogManagementMoustacheStrategy()
    {
    }
    
    public override void Install()
    {
      Network.NewChannel += NetworkNewChannel;
      Network.NewWad += NetworkNewWad;
    }

    public override void Uninstall()
    {
      Network.NewChannel -= NetworkNewChannel;
      Network.NewWad -= NetworkNewWad;
    }

    public override void Think()
    {
    }
    
    void NetworkNewChannel(object sender, GenericEventArgs<Channel> e)
    {
      // get whatever comes out as we'll get it with our settings in it
      var chan = Catalog.AddChannel(e.Value);
      
      if ( chan.Subscribed )
      {
        Peer[] peers = Network.ActivePeers.ToArray();
        foreach (Peer peer in peers)
          Network.RequestWads(peer, 0, 30, e.Value.Id);
      }
    }
    void NetworkNewWad(object sender, GenericEventArgs<FileWad> e)
    {
      var wad = Catalog.AddFileWad(e.Value);
    }
  }
}



