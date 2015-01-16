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
namespace FuzzyHipster.Catalog
{
  [Serializable()]
  public class FileWad : IEquatable<FileWad>
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

    public void CatalogBlock( int block, string tempFile )
    {
      string path = string.Format(@"{0}\Catalog\{1}\{2}\", MoustacheLayer.Singleton.Catalog.BasePath, this.StackId, Id);
      if ( Directory.Exists( path ))
        Directory.CreateDirectory(path);
    }
    
    public bool VerifyBlock(string tempFile )
    {
//			if (block.Sequence >= BlockIndex.Count)
//				return false;
//			// sequence wrong
//			if (block.Sequence <= -1)
//				return false;
//			// sequence wrong
//			BlockIndexItem item = BlockIndex[block.Sequence];
//			if (block.Length != item.Length)
//				return false;
//			// block length doesn't match expected length
//			if (block.Hash != item.Hash)
//				return false;
//			// hashes dont match
//			if (block.Data.Length != BlockSize)
//				return false;
//			// length of data buffer doesn't match block size
      return true;
    }

    /// <summary>
    /// Build a FileWad from either a File Path or Directory Path
    /// </summary>
    /// <param name="path"></param>
    public void BuildFromPath(string path)
    {
      bool isFile = File.Exists(path);
      long totalLength = 0;
      
      if ( isFile )
        totalLength = AddFile(Path.GetDirectoryName(path), path);
      else
      {
        if (!Directory.Exists(path))
          throw new Exception(string.Format("Path {0} doesn't exist", path));
        
        totalLength = RecursiveBuildFiles(path, path);
      }
      
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

    object locker = new object();
    public void Save()
    {
      lock (locker) {
        string basePath = MoustacheLayer.Singleton.Catalog.BasePath;
        string stackPath = Path.Combine(basePath, string.Format(@"Catalog\Stacks\{0}\", StackId));
        
        if (!Directory.Exists(stackPath))
          Directory.CreateDirectory(stackPath);
        
        var serialiserStacks = new XmlSerializer(typeof(FileWad));
        using (var writer = new StreamWriter(string.Format("{0}{1}.xml", stackPath, Id)))
          serialiserStacks.Serialize(writer, this);
      }
    }
    
    public static FileWad Load( string filePath )
    {
      if ( !File.Exists(filePath))
        return null;
      
      var serialiser = new XmlSerializer(typeof(FileWad));
      using (var reader = new StreamReader(filePath))
        return serialiser.Deserialize(reader) as FileWad;
    }
    
    private long AddFile(string basePath, string file)
    {
      var info = new FileInfo(file);
      var descriptor = new FileDescriptor()
      {
        CatalogFilepath = file.Substring(basePath.Length),
        LocalFilepath = file,
        IsAllocated = true,
        Length = info.Length
      };
      
      Console.WriteLine(descriptor.ToString());
      Files.Add(descriptor);
      return info.Length;
    }
    
    private long RecursiveBuildFiles(string basePath, string path)
    {
      string[] files = Directory.GetFiles(path);
      long totalLength = 0;
      
      foreach (string file in files)
      {
        totalLength += AddFile(basePath, file);
      }
      
      foreach (string dir in Directory.GetDirectories(path))
      {
        totalLength += RecursiveBuildFiles(basePath, dir);
      }
      return totalLength;
    }

    #region IEquatable implementation
    public bool Equals(FileWad other)
    {
      return other.Id == Id;
    }
    #endregion
    
    public override string ToString()
    {
      return string.Format("[FileWad Locker={0}, Id={1}, StackId={2}, Name={3}, Description={4}, BlockSize={5}, TotalBlocks={6}, TotalSize={7}, LastUpdate={8}, Files={9}, BlockIndex={10}]", locker, Id, StackId, Name, Description, BlockSize, TotalBlocks, TotalSize, LastUpdate, Files, BlockIndex);
    }
    
    public static long CalculatePathSize( string path )
    {
      long size = 0;
      
      if ( !Directory.Exists(path))
        return 0;
      
      foreach( string dir in Directory.GetDirectories(path))
      {
        size += CalculatePathSize(dir);
        foreach( string file in Directory.GetFiles(dir))
        {
          var info = new FileInfo(file);
          size += info.Length;
        }
      }
      
      return size;
    }
    
    public static long EstimateBlockSize( int quantityDesired, long totalBytes )
    {
      long blockSize = totalBytes / quantityDesired;
      int p = (int)Math.Pow(2, Math.Ceiling(Math.Log(blockSize)/Math.Log(2)));
      
      if ( p < 1024 )
        p = 1024;
      
      return p;
    }
  }
}



