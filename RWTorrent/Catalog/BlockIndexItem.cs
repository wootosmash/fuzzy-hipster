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
namespace FuzzyHipster
{
  [Serializable()]
  public class BlockIndexItem
  {
    public byte[] Hash {
      get;
      set;
    }

    public long Length {
      get;
      set;
    }

    [NonSerialized()]
    bool downloaded;
    
    public bool Downloaded {
      get {
        return downloaded;
      }
      set {
        downloaded = value;
      }
    }
    
    [NonSerialized()]
    bool downloading;
    
    [XmlIgnore()]
    public bool Downloading {
      get {
        return downloading;
      }
      set {
        downloading = value;
      }
    }
  }
}



