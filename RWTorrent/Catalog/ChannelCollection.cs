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
namespace FuzzyHipster.Catalog
{
  public enum SearchFilter 
  {
    All,
    WriteableOnly,
    ReadableOnly
  }
  
	public class ChannelCollection : List<Channel>
	{
	  
		public Channel this[Guid channelGuid] {
			get {
				return Find(x => x.Id == channelGuid);
			}
		}

	  public Channel[] Find( string searchText, SearchFilter filter )
	  {
	    return ToArray();
	  }
		
    public Channel GetRandom()
    {
      if ( Count == 0 )
        return null;
      
      int index = MoustacheLayer.Singleton.Random.Next(0, Count);
      return this[index];
    }
		
	}
}





