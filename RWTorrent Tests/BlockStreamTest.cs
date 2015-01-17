﻿/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 17/01/2015
 * Time: 5:13 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using FuzzyHipster.Crypto;
using NUnit.Framework;
using FuzzyHipster.Catalog;

namespace FuzzyHipster.Tests
{
  [TestFixture]
  public class BlockStreamTest
  {
    [Test]
    public void ReadBlockTest()
    {
      var catalog = FuzzyHipster.Catalog.Catalog.Load(@"E:\Temp\BlockStreamTest\");
      var statche = new MoustacheLayer(catalog);

      var wad = new FileWad();
      wad.Name = "Test WAD-" + DateTime.Now;
      wad.Description = "Built from B";
      wad.BlockSize = 4096;
      wad.BuildFromPath( @"C:\temp\mendozaaaa");
      
      var chan = new Channel();
      chan.Name = "FOR";
      wad.ChannelId = chan.Id;
      
      catalog.AddChannel(chan);
      catalog.AddFileWad(wad);
      
      Console.WriteLine("TEST");
      
      for( int i=92;i<93;i++) // wad.BlockIndex.Count;i++)
      {
        using ( var stream = new BlockStream(wad))
        {
          long count = wad.BlockIndex[i].Length;
          byte[] buffer = new byte[count];
          stream.SeekBlock(i);
          stream.Read(buffer, 0, (int)count);
          
          Assert.IsTrue(Hash.Compare(Hash.GetHash(buffer, count), wad.BlockIndex[i].Hash), "Hash Fail " + i + "/" + wad.BlockIndex.Count);
        }
      }
    }
  }
}
