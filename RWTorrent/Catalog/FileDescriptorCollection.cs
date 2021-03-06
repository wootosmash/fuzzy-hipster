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
  public class FileDescriptorCollection : List<FileDescriptor>
  {
    public FileDescriptor[] GetDescriptorsByBlock(Block block)
    {
      var descs = new List<FileDescriptor>();
      foreach (var desc in this)
        if (desc.EndBlock >= block.Sequence && desc.StartBlock <= block.Sequence)
          descs.Add(desc);
      return descs.ToArray();
    }
    
    public FileDescriptor GetRandom()
    {
      if ( Count == 0 )
        return null;
      
      int index = MoustacheLayer.Singleton.Random.Next(0, Count);
      return this[index];
    }    
    
  }
}



