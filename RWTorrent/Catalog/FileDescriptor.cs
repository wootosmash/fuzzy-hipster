﻿/*
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
  [Serializable()]
  public class FileDescriptor
  {
    public string CatalogFilepath {
      get;
      set;
    }

    [NonSerialized()]
    string localFilepath;
    public string LocalFilepath {
      get {
        return localFilepath;
      }
      set {
        localFilepath = value;
      }
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

    public long EndFragmentSize {
      get;
      set;
    }

    public byte[] Hash {
      get;
      set;
    }

    public long Length {
      get;
      set;
    }

    [NonSerialized()]
    bool isAllocated;
    public bool IsAllocated {
      get {
        return isAllocated;
      }
      set {
        isAllocated = value;
      }
    }
    
    public void AllocateFile()
    {
      using (FileStream file = File.Create(LocalFilepath)) {
        file.SetLength(Length);
      }
      IsAllocated = true;
    }
    
    /// <summary>
    /// Determines if the block sequence contains part of this file
    /// </summary>
    /// <param name="block"></param>
    /// <returns></returns>
    public bool IsFileBlock( int block )
    {
      return StartBlock <= block && EndBlock >= block;
    }


    public override string ToString()
    {
      return string.Format("[FileDescriptor CatalogFilepath={0}, LocalFilepath={1}, StartBlock={2}, EndBlock={3}, StartOffset={4}, EndOffset={5}, Hash={6}, Length={7}, IsAllocated={8}]", CatalogFilepath, LocalFilepath, StartBlock, EndBlock, StartOffset, EndFragmentSize, Hash, Length, IsAllocated);
    }

  }
}



