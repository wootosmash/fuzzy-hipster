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
namespace FuzzyHipster.Strategy
{
  public class CatalogManagementMoustacheStrategy : MoustacheStrategy
  {
    public DateTime NextThink { get; set; }
    
    public CatalogManagementMoustacheStrategy()
    {
    }
    
    public override void Install()
    {
      Network.NewChannel += NetworkNewChannel;
      Network.NewWad += NetworkNewWad;
      Catalog.NotifyFileWad += CatalogNotifyFileWad;
      Catalog.NotifyChannel += CatalogNotifyChannel;
    }

    public override void Uninstall()
    {
      Network.NewChannel -= NetworkNewChannel;
      Network.NewWad -= NetworkNewWad;
      Catalog.NotifyFileWad -= CatalogNotifyFileWad;
      Catalog.NotifyChannel -= CatalogNotifyChannel;
    }

    public override void Think()
    {
      if ( NextThink < DateTime.Now )
      {
        Network.RequestChannels(Network.ActivePeers.GetRandom(), Catalog.LastUpdated, Settings.CatalogThinkRequestSize);
        Network.RequestWads(Network.ActivePeers.GetRandom(), Catalog.LastUpdated, Settings.CatalogThinkRequestSize, Guid.Empty);
        NextThink = DateTime.Now.AddMilliseconds(Settings.CatalogThinkInterval);
      }
    }
    
    void CatalogNotifyFileWad( object sender, GenericEventArgs<FileWad> e )
    {
      if ( ShouldAdvertiseCatalogItem( e.Value ) )
        Network.SendWads(new[] {e.Value}, Network.ActivePeers.ToArray());
    }
    
    void CatalogNotifyChannel( object sender, GenericEventArgs<Channel> e )
    {
      if ( ShouldAdvertiseCatalogItem( e.Value ) )
        Network.SendChannels(new [] {e.Value}, Network.ActivePeers.ToArray());
    }
    
    void NetworkNewChannel(object sender, GenericEventArgs<Channel> e)
    {
      if ( e.Value == null )
        return;
      
      e.Value.Validate();

      // get whatever comes out as we'll get it with our settings in it
      e.Value.AdvertisementMoratorium = DateTime.Now.AddSeconds(Settings.DefaultAdvertisementMoratorium);
      var chan = Catalog.AddChannel(e.Value);
    }
    void NetworkNewWad(object sender, GenericEventArgs<FileWad> e)
    {
      if ( e.Value == null )
        return;
      
      e.Value.Validate();
      
      e.Value.AdvertisementMoratorium = DateTime.Now.AddSeconds(Settings.DefaultAdvertisementMoratorium);
      var wad = Catalog.AddFileWad(e.Value);
    }
    
    bool ShouldAdvertiseCatalogItem( CatalogItem item )
    {
      return ( item.AdvertisementMoratorium < DateTime.Now  );      
    }
  }
}



