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
using FuzzyHipster.Crypto;
namespace FuzzyHipster.Catalog
{
  [Serializable()]
  public class FileWad : CatalogItem, IEquatable<FileWad>
  {
    public Guid Id { get; set; }
    
    public Guid ChannelId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public long BlockSize { get; set; }

    public int TotalBlocks { get; set; }
    
    public long TotalSize { get; set; }
    
    public FileDescriptorCollection Files { get; set; }

    public BlockIndexItemCollection BlockIndex { get; set; }
    
    [NonSerialized()]
    bool isFullyDownloaded;
    public bool IsFullyDownloaded {
      get {
        return isFullyDownloaded;
      }
      set {
        isFullyDownloaded = value;
      }
    }

    public FileWad()
    {
      Id = Guid.NewGuid();
      Files = new FileDescriptorCollection();
      BlockIndex = new BlockIndexItemCollection();
      LastUpdated = DateTime.Now.ToFileTimeUtc();
      IsFullyDownloaded = true;
    }

    public void CatalogBlock( int block, string tempFile )
    {
      string path = string.Format(@"{0}\Catalog\{1}\{2}\", MoustacheLayer.Singleton.Catalog.BasePath, this.ChannelId, Id);
      if ( Directory.Exists( path ))
        Directory.CreateDirectory(path);

      CheckIfFullyDownloaded();
    }
    
    public void CheckIfFullyDownloaded()
    {
      bool downloaded = true;
      for( int i=0;i<BlockIndex.Count;i++)
      {
        if ( !BlockIndex[i].Downloaded )
        {
          Console.WriteLine("Block {0} is not downloaded", i);
          downloaded = false;
          break;
        }
      }
      IsFullyDownloaded = downloaded;
    }
    
    public long GetBlockSize( FileDescriptor file, int block )
    {
      if ( file.StartBlock > block )
        return 0;
      if ( file.EndBlock < block )
        return 0;
      
      if ( file.StartBlock == file.EndBlock && file.StartBlock == block )
        return file.EndOffset - file.StartOffset;
      if ( file.StartBlock == block )
        return BlockSize - file.StartOffset;
      if ( file.EndBlock == block )
        return BlockSize - file.EndOffset;
      if ( file.StartBlock < block && file.EndBlock > block )
        return BlockSize;
      return 0;
    }
    
    public long GetBlockStartOffset( FileDescriptor file, int block )
    {
      if ( file.EndBlock < block )
        return -1;
      if ( file.StartBlock > block )
        return -1;
      
      if ( file.StartBlock == block )
        return file.StartOffset;
      if ( file.EndBlock == block )
        return 0;
      if ( file.StartBlock < block && file.EndBlock > block )
        return 0;
      return -1;
    }
    
    public long GetBlockEndOffset( FileDescriptor file, int block )
    {
      if ( file.EndBlock < block )
        return -1;
      if ( file.StartBlock > block )
        return -1;
      
      if ( file.StartBlock == block )
        return BlockSize - file.StartOffset;
      if ( file.EndBlock == block )
        return file.EndOffset;
      if ( file.StartBlock < block && file.EndBlock > block )
        return BlockSize;
      return -1;
    }
    

    public string GetBlocksPath()
    {
      string path = Path.Combine(MoustacheLayer.Singleton.Catalog.BasePath, @"Catalog\Blocks\" + Id + @"\");
      return path;
    }
    
    public Stream GetBlockStream( int block )
    {
      string blocksPath = GetBlocksPath();
      string [] blocks = Directory.GetFiles(blocksPath);
      foreach( string file in blocks )
      {
        if ( file.StartsWith(block + "-"))
          return new FileStream(file, FileMode.Open);
      }
      
      return null;
    }
    
    public void SaveFromBlocks( string basePath )
    {
      Console.WriteLine("Saving from blocks");
      
      if ( Files == null )
        throw new Exception("FileWad.Files is not set");
      if ( Files.Count == 0 )
        throw new Exception("FileDescriptors list is empty");
      
      foreach( var file in Files )
        SaveFromBlocks(file, basePath);
    }

    /// <summary>
    /// Save a file from blocks
    /// </summary>
    /// <param name="file"></param>
    /// <param name="basePath"></param>
    public void SaveFromBlocks( FileDescriptor file, string basePath )
    {
      const int bufferLength = 1024;
      byte[] buffer = new byte[bufferLength];
      string blocksPath = GetBlocksPath();
      
      if ( !Directory.Exists(basePath))
        Directory.CreateDirectory(basePath);
      
      string filePath = Path.Combine(basePath, file.CatalogFilepath);
      using (var writer = new FileStream(filePath, FileMode.CreateNew))
      {
        for ( int block = file.StartBlock; block <= file.EndBlock; block++ )
        {
          using (var reader = GetBlockStream(block))
          {
            long start = GetBlockStartOffset(file, block);
            long end = GetBlockEndOffset(file, block);
            long totalLength = end - start;
            
            while ( totalLength > 0 )
            {
              long count = bufferLength;
              if ( count > totalLength )
                count = totalLength;
              count = reader.Read(buffer, (int)GetBlockStartOffset(file, block), (int)count);
              writer.Write(buffer, 0,  (int)count);
              totalLength -= count;
            }
          }
        }
      }
      
      byte[] hash = Hash.GetHash(filePath);
      if ( Hash.Compare(file.Hash, hash))
      {
        File.Delete(filePath);
        throw new Exception(string.Format("Convert from blocks failed, hash is bad. {0} != {1}", file.Hash, hash));
      }
    }
    
    public bool VerifyBlock( int block, string blockFilePath )
    {
      var info = new FileInfo(blockFilePath);

      BlockIndexItem item = BlockIndex[block];
      
      if ( item.Hash == null )
        throw new Exception("BlockIndex item " + block + " has no hash");
      
      // block length doesn't match expected length
      if (info.Length != item.Length)
        return false;
      // hashes dont match
      if ( Hash.Compare(Hash.GetHash(blockFilePath), item.Hash))
        return false;
      // length of data buffer doesn't match block size
      if (info.Length != BlockSize)
        return false;
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
      
      if ( BlockSize <= 0 )
        BlockSize = EstimateBlockSize(MoustacheLayer.Singleton.Settings.DefaultBlockQuantity, CalculatePathSize(path));
      
      TotalBlocks = (int)Math.Ceiling((double)totalLength / (double)BlockSize);
      
      int lastBlock = 0;
      long lastOffset = 0;
      
      // build file descs
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
        file.Hash = Hash.GetHash(file.LocalFilepath );
      }
      
      // build block index
      for (int i = 0; i < TotalBlocks; i++)
      {
        var block = new BlockIndexItem();
        block.Length = BlockSize;
        block.Downloaded = true;
        BlockIndex.Add(block);
      }
      
      if (totalLength % BlockSize > 0)
        BlockIndex[BlockIndex.Count - 1].Length = totalLength % BlockSize;
      
      // calculate the hashes
      using ( var stream = new BlockStream( this ))
      {
        foreach( var block in BlockIndex )
        {
          block.Hash = Hash.GetHash(stream, block.Length);
        }
      }
      
      
    }
    
    public override void Validate()
    {
      CheckIfFullyDownloaded();
    }

    public void Save()
    {
      lock (this) {
        string basePath = MoustacheLayer.Singleton.Catalog.BasePath;
        string channelPath = Path.Combine(basePath, string.Format(@"Catalog\Channels\{0}\", ChannelId));
        
        if (!Directory.Exists(channelPath))
          Directory.CreateDirectory(channelPath);
        
        var serialiserChannels = new XmlSerializer(typeof(FileWad));
        using (var writer = new StreamWriter(string.Format("{0}{1}.xml", channelPath, Id)))
          serialiserChannels.Serialize(writer, this);
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
        Hash = Hash.GetHash(file),
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
      return string.Format("[FileWad Id={0}, ChannelId={1}, Name={2}, Description={3}, BlockSize={4}, TotalBlocks={5}, TotalSize={6}, LastUpdate={7}, Files={8}, BlockIndex={9}]", Id, ChannelId, Name, Description, BlockSize, TotalBlocks, TotalSize, LastUpdated, Files, BlockIndex);
    }

    
    public static long CalculatePathSize( string path )
    {
      long size = 0;
      
      if ( !Directory.Exists(path))
        return 0;
      
      foreach( string file in Directory.GetFiles(path))
      {
        var info = new FileInfo(file);
        size += info.Length;
      }
      
      foreach( string dir in Directory.GetDirectories(path))
      {
        size += CalculatePathSize(dir);
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



