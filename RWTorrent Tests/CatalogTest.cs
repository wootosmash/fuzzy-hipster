/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 10/01/2015
 * Time: 4:49 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using NUnit.Framework;
using FuzzyHipster.Catalog;

namespace FuzzyHipster.Tests
{

  [TestFixture]
  public class CatalogTest
  {
    [Test]
    public void SaveTest()
    {      
      var catalog = new Catalog.Catalog();
      catalog.BasePath = Environment.CurrentDirectory + @"\Localhost-7892\";
      catalog.Namespace = "Base Programs";
      catalog.Description = "Test Catalog for Base Programs";
      
      Channel faxes = new Channel() { 
        Id = Guid.NewGuid(),
        Name = "Rofl", 
        Description = "ROFL MAYO", 
        PublicKey = null 
      };
      
      long blockSize = FileWad.EstimateBlockSize(100, FileWad.CalculatePathSize(@"C:\temp\chuck"));
      FileWad myprog = new FileWad() { ChannelId = faxes.Id, BlockSize = blockSize, Name = "Chuck", Description = "Wacky" };
      myprog.BuildFromPath( @"C:\temp\chuck");
      
      faxes.Wads.Add(myprog);      

      catalog.Channels.Add( faxes );
      
      var torrent = new MoustacheLayer(catalog);
      catalog.Save();
      
      
    }
  }
}
