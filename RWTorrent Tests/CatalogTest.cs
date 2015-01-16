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
      catalog.BasePath = Environment.CurrentDirectory + @"\Localhost1\";
      catalog.Namespace = "Base Programs";
      catalog.Description = "Test Catalog for Base Programs";
      
      Channel faxes = new Channel() { 
        Id = Guid.NewGuid(),
        Name = "Faxes Shit", 
        Description = "All fax wangs shit", 
        PublicKey = "..." 
      };
      FileWad myprog = new FileWad() { ChannelId = faxes.Id, BlockSize = 1024, Name = "My Program", Description = "A hilarious Program" };
      myprog.BuildFromPath( @".");
      
      faxes.Wads.Add(myprog);      

      catalog.Channels.Add( faxes );
      
      var torrent = new MoustacheLayer(catalog);
      catalog.Save();
      
      
    }
  }
}
