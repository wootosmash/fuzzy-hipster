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
  [Serializable()]
	public abstract class CatalogItem
	{
	  public Guid Id { get; set; }
	  public long LastUpdated { get; set; }
	  
	  public abstract void Validate();
	  
    public void UpdateLastUpdated( long lastUpdated )
    {
      if ( lastUpdated > LastUpdated )
        LastUpdated = lastUpdated;
    }	  
	}
}





