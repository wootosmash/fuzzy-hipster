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
	/// <summary>
	/// A class for wrapping up the code and objects for the moustache intelligence
	/// </summary>
	public abstract class MoustacheStrategy
	{
		public Catalog.Catalog Catalog {
			get {
				return MoustacheLayer.Singleton.Catalog;
			}
		}

		public RWNetwork Network {
			get {
				return MoustacheLayer.Singleton.Network;
			}
		}

		public PeerCollection Peers {
			get {
				return MoustacheLayer.Singleton.Peers;
			}
		}
	  
	  public Settings Settings { 
	    get {
	      return MoustacheLayer.Singleton.Settings;
	    }
	  }
	  
	  public bool Enabled { get; set; }
	  
	  public void Enable()
	  {
	    Enabled = true;
	    Install();
	  }
	  
	  public void Disable()
	  {
	    Uninstall();
	    Enabled = false;
	  }

		public abstract void Install();

		public abstract void Uninstall();

		public abstract void Think();
	}
}



