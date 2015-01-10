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
using RWTorrent.Catalog;

namespace RWTorrent.Tests
{

  [TestFixture]
  public class CatalogTest
  {
    [Test]
    public void SaveTest()
    {      

      var catalog = new Catalog.Catalog();
      catalog.BasePath = Environment.CurrentDirectory;
      catalog.Namespace = "Base Programs";
      catalog.Description = "Test Catalog for Base Programs";
      
      Stack faxes = new Stack() { 
        Id = Guid.NewGuid(),
        Name = "Faxes Shit", 
        Description = "All fax wangs shit", 
        PublicKey = "..." 
      };
      FileWad blazingSaddles = new FileWad() { BlockSize = 1024, Name = "My Program", Description = "A hilarious Program" };
      blazingSaddles.BuildFromPath( @".");
      
      faxes.Wads.Add(blazingSaddles);      

      catalog.Stacks.Add( faxes );
      catalog.Save();
      
      
    }
  }
}
