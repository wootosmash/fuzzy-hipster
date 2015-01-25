/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 24/01/2015
 * Time: 3:17 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using FuzzyHipster.Network;

namespace FuzzyHipster.Strategy
{
  /// <summary>
  /// Description of BlockAvailabilityStrategy.
  /// </summary>
  public class BlockAvailabilityStrategy : MoustacheStrategy
  {
    public BlockAvailabilityStrategy()
    {
    }

    public override void Install()
    {
      Network.BlocksAvailableReceived += NetworkBlocksAvailableReceived;
    }
    
    public override void Uninstall()
    {
      Network.BlocksAvailableReceived -= NetworkBlocksAvailableReceived;
    }
    
    public override void Think()
    {
    }

    void NetworkBlocksAvailableReceived(object sender, MessageComposite<BlocksAvailableNetMessage> e)
    {
      var wad = MoustacheLayer.Singleton.Catalog.GetFileWad(e.Value.FileWadId);
      
      BlockAvailability.Update( e.Peer, wad, e.Value.BlocksAvailable);
      
      
    }
  }
}
