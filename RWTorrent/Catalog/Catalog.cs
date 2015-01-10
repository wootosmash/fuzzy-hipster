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
  
  
	public class Catalog
	{
		public string Namespace {
			get;
			set;
		}

		public string Description {
			get;
			set;
		}

		public string BasePath { get; set; }
	  
	  public long LastUpdated { get; set; }

	  [XmlIgnore()]
		public StackCollection Stacks {
			get;
			set;
		}

		public Catalog()
		{
			Stacks = new StackCollection();
		}

		public static Catalog Load(string basePath)
		{
		  Catalog catalog = null;

		  var serialiser = new XmlSerializer(typeof(Catalog));
			using (var reader = new StreamReader(Path.Combine(basePath, @"Catalog\Catalog.xml")))
				catalog = (Catalog)serialiser.Deserialize(reader);

			var serialiserStacks = new XmlSerializer(typeof(Stack));
			foreach (string file in Directory.GetFiles(Path.Combine(basePath, @"Catalog\Stacks")))
				using (var reader = new StreamReader(file))
					catalog.Stacks.Add((Stack)serialiserStacks.Deserialize(reader));

			return catalog;
		}

		public void Save()
		{
		  if ( !Directory.Exists(Path.Combine(BasePath, @"Catalog")))
		    Directory.CreateDirectory(Path.Combine(BasePath, @"Catalog"));
		  
		  if ( !Directory.Exists(Path.Combine(BasePath, @"Catalog\Stacks\")))
		    Directory.CreateDirectory(Path.Combine(BasePath, @"Catalog\Stacks\"));
		  
			var serialiser = new XmlSerializer(typeof(Catalog));
			using (var writer = new StreamWriter(Path.Combine(BasePath, @"Catalog\Catalog.xml")))
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
	}
}



