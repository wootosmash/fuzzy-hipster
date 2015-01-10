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
namespace RWTorrent.Catalog
{
  public class FileDescriptor
  {
    public string CatalogFilepath {
      get;
      set;
    }

    public string LocalFilepath {
      get;
      set;
    }

    public int StartBlock {
      get;
      set;
    }

    public int EndBlock {
      get;
      set;
    }

    public long StartOffset {
      get;
      set;
    }

    public long EndOffset {
      get;
      set;
    }

    public string Hash {
      get;
      set;
    }

    public long Length {
      get;
      set;
    }

    public bool IsAllocated {
      get;
      set;
    }

    public void AllocateFile()
    {
      using (FileStream file = File.Create(LocalFilepath)) {
        file.SetLength(Length);
      }
      IsAllocated = true;
    }
    
    public override string ToString()
    {
      return string.Format("[FileDescriptor CatalogFilepath={0}, LocalFilepath={1}, StartBlock={2}, EndBlock={3}, StartOffset={4}, EndOffset={5}, Hash={6}, Length={7}, IsAllocated={8}]", CatalogFilepath, LocalFilepath, StartBlock, EndBlock, StartOffset, EndOffset, Hash, Length, IsAllocated);
    }

  }
}



