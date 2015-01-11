/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 10/01/2015
 * Time: 11:53 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Xml.Serialization;
namespace RWTorrent.Catalog
{
	public class FileWad
	{
	  public Guid Id { get; set; }
	  
	  public Guid StackId { get; set; }

	  public string Name { get; set; }

	  public string Description { get; set; }

	  public long BlockSize { get; set; }

	  public int TotalBlocks { get; set; }
	  
	  public long TotalSize { get; set; }
	  
	  public long LastUpdate { get; set; }	    

		public FileDescriptorCollection Files { get; set; }	    

		public BlockIndexItemCollection BlockIndex { get; set; }

		public FileWad()
		{
      Id = Guid.NewGuid();		  
			Files = new FileDescriptorCollection();
			BlockIndex = new BlockIndexItemCollection();
			LastUpdate = DateTime.Now.ToFileTimeUtc();
		}

		public bool VerifyBlock(Block block)
		{
			if (block.Sequence >= BlockIndex.Count)
				return false;
			// sequence wrong
			if (block.Sequence <= -1)
				return false;
			// sequence wrong
			BlockIndexItem item = BlockIndex[block.Sequence];
			if (block.Length != item.Length)
				return false;
			// block length doesn't match expected length
			if (block.Hash != item.Hash)
				return false;
			// hashes dont match
			if (block.Data.Length != BlockSize)
				return false;
			// length of data buffer doesn't match block size
			return true;
		}

		public void BuildFromPath(string path)
		{
			if (!Directory.Exists(path))
			  throw new Exception(string.Format("Path {0} doesn't exist", path));
			
			long totalLength = RecursiveBuildFiles(path, path);
			
			TotalBlocks = (int)Math.Ceiling((double)totalLength / (double)BlockSize);
			
			for (int i = 0; i < TotalBlocks; i++) 
			{
				var block = new BlockIndexItem();
				block.Length = BlockSize;
				block.Downloaded = true;
//				block.Hash = GetHash();
				BlockIndex.Add(block);
			}
			
			if (totalLength % BlockSize > 0)
				BlockIndex[BlockIndex.Count - 1].Length = totalLength % BlockSize;
			
			int lastBlock = 0;
			long lastOffset = 0;
			
			foreach (FileDescriptor file in Files) 
			{
				file.StartBlock = lastBlock;
				lastBlock += (int)Math.Floor((double)file.Length / (double)BlockSize);
				
				if (lastOffset == 0)
					lastBlock++;
				
				file.EndBlock = lastBlock;
				file.StartOffset = lastOffset;
				
				if (file.StartBlock == file.EndBlock)
					lastOffset += (long)file.Length % BlockSize;
				else
					lastOffset = (long)file.Length % BlockSize;
				
				file.EndOffset = lastOffset;
			}
		}

    public void Save()
    {
      string basePath = RWTorrent.Singleton.Catalog.BasePath;
      string stackPath = Path.Combine(basePath, string.Format(@"Catalog\Stacks\{0}\", StackId));
      
      if ( !Directory.Exists(stackPath))
        Directory.CreateDirectory(stackPath);
      
      var serialiserStacks = new XmlSerializer(typeof(FileWad));
      using (var writer = new StreamWriter(string.Format("{0}{1}.xml", stackPath, Id)))
        serialiserStacks.Serialize(writer, this);
    }
		
		
		private long RecursiveBuildFiles(string basePath, string path)
		{
			string[] files = Directory.GetFiles(path);
			long totalLength = 0;
			
			foreach (string file in files) 
			{
				var info = new FileInfo(file);
				var descriptor = new FileDescriptor() 
				{
					CatalogFilepath = file.Substring(basePath.Length),
					LocalFilepath = file,
					IsAllocated = true,
					Length = info.Length
				};
				
				Files.Add(descriptor);
				totalLength += info.Length;
				
				Console.WriteLine(descriptor.ToString());
			}
			
			foreach (string dir in Directory.GetDirectories(path)) 
			{
				totalLength += RecursiveBuildFiles(basePath, dir);
			}
			return totalLength;
		}
	}
}



