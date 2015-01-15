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
  
  
  public class Catalog
  {
    public Guid Guid { get; set; }
    
    public string Namespace { get; set; }

    public string Description { get; set; }

    public string BasePath { get; set; }
    
    public long LastUpdated { get; set; }

    [XmlIgnore()]
    public StackCollection Stacks {
      get;
      set;
    }

    public Catalog()
    {
      Guid = Guid.NewGuid();
      Stacks = new StackCollection();
    }


    public static Catalog Load(string basePath)
    {
      string catalogEndPointPath = Path.Combine(basePath, @"Catalog\Catalog.xml");
      Catalog catalog = null;
      
      if ( !Directory.Exists(Path.Combine(basePath, @"Catalog")) || !File.Exists(catalogEndPointPath) )
      {
        catalog = new Catalog();
        catalog.BasePath = basePath;
        catalog.Save(); // cleans up the bogus directory
      }
      else
      {
        var serialiser = new XmlSerializer(typeof(Catalog));
        using (var reader = new StreamReader( catalogEndPointPath ))
          catalog = (Catalog)serialiser.Deserialize(reader);

        catalog.BasePath = basePath;
        
        string stacksPath = Path.Combine(basePath, @"Catalog\Stacks");
        
        if ( !Directory.Exists(stacksPath ))
          Directory.CreateDirectory(stacksPath);

        var serialiserStacks = new XmlSerializer(typeof(Stack));
        foreach (string dir in Directory.GetDirectories(stacksPath))
        {
          Console.WriteLine(dir);
          Stack stack = null;
          using (var reader = new StreamReader(string.Format(@"{0}\Index.xml", dir)))
          {
            stack = (Stack)serialiserStacks.Deserialize(reader);
            catalog.Stacks.Add(stack);
          }
          
          foreach( string file in Directory.GetFiles(Path.Combine(basePath, string.Format(@"Catalog\Stacks\{0}\",stack.Id))))
            if ( !file.EndsWith("Index.xml"))
              stack.Wads.Add(FileWad.Load(file));
        }
      }
      
      return catalog;
    }

    public void Save()
    {
      string catalogEndPointPath = Path.Combine(BasePath, @"Catalog\Catalog.xml");

      if ( !Directory.Exists(Path.Combine(BasePath, @"Catalog")))
        Directory.CreateDirectory(Path.Combine(BasePath, @"Catalog"));
      
      if ( !Directory.Exists(Path.Combine(BasePath, @"Catalog\Stacks\")))
        Directory.CreateDirectory(Path.Combine(BasePath, @"Catalog\Stacks\"));
      
      var serialiser = new XmlSerializer(typeof(Catalog));
      using (var writer = new StreamWriter(catalogEndPointPath))
        serialiser.Serialize(writer, this);
      
      foreach (var stack in Stacks)
        stack.Save();
    }

    public FileWad GetFileWad(Guid id)
    {
      foreach (var stack in Stacks)
        foreach (var wad in stack.Wads)
          if (wad.Id == id)
            return wad;
      
      return null;
    }
    
    public override string ToString()
    {
      return string.Format("[Catalog Namespace={0}, Description={1}, BasePath={2}, LastUpdated={3}, Stacks.Count={4}]", 
                           Namespace, Description, BasePath, LastUpdated, Stacks.Count);
    }

  }
}



