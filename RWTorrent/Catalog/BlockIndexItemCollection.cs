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
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Xml.Serialization;
namespace FuzzyHipster.Catalog
{
  [Serializable()]
  public class BlockIndexItemCollection : List<BlockIndexItem>
  {
    public BlockIndexItem GetRandom()
    {
      if ( Count == 0 )
        return null;
      
      int index = MoustacheLayer.Singleton.Random.Next(0, Count);
      return this[index];
    }
    
    public decimal PercentDownloaded
    {
      get
      {
        decimal c = Count;
        if ( c == 0 )
          return 0;
        else
          return this.Count(x => x.Downloaded) / c;
      }
    }
  }
}



