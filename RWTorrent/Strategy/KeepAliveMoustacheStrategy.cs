/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 10/01/2015
 * Time: 11:53 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
namespace FuzzyHipster
{
  public class KeepAliveMoustacheStrategy : MoustacheStrategy
  {
    public DateTime NextThink {
      get;
      set;
    }

    public override void Install()
    {
    }

    public override void Uninstall()
    {
    }

    public override void Think()
    {
      DateTime now = DateTime.Now;
      foreach( var peer in MoustacheLayer.Singleton.Network.ActivePeers.ToArray())
        if ( peer.OkToSendAt > now )
          MoustacheLayer.Singleton.Network.SendMyStatus(peer);
      NextThink = DateTime.Now.AddMilliseconds(MoustacheLayer.Singleton.Settings.KeepAliveInterval);
    }
  }
}



