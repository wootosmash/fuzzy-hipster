

using System;
using System.IO;
namespace FuzzyHipster.Network
{
	public abstract class TransferManager
	{
		public Guid TransferId {
			get;
			set;
		}

		public Guid FileWadId {
			get;
			set;
		}

		public int Block {
			get;
			set;
		}

		public int ExpectedPackets {
			get;
			set;
		}

		public int TotalLength {
			get;
			set;
		}

		public int NextPacket {
			get;
			set;
		}

		public bool IsCompleted {
			get {
				return ExpectedPackets <= NextPacket;
			}
		}

		public string TempFile {
			get;
			set;
		}

		public int CurrentPosition {
			get;
			set;
		}

		public TransferManager()
		{
			NextPacket = 0;
			TransferId = Guid.NewGuid();
			CurrentPosition = 0;
		}

		public void SavePacket(BlockPacketNetMessage msg)
		{
			if (String.IsNullOrWhiteSpace(TempFile)) 
			{
				string blocksPath = Path.Combine(MoustacheLayer.Singleton.Catalog.BasePath, 
			                                   string.Format(@"Catalog\Blocks\{0}\", FileWadId));
				TempFile = blocksPath + Block + "-" + TransferId + ".blk";
				if (!Directory.Exists(blocksPath))
					Directory.CreateDirectory(blocksPath);
			}
			
			using (var stream = new FileStream(TempFile, FileMode.OpenOrCreate)) 
			{
				stream.Seek(CurrentPosition, SeekOrigin.Begin);
				stream.Write(msg.Data, 0, msg.DataLength);
				CurrentPosition += msg.DataLength;
			}
			
			NextPacket++;
			
			if (IsCompleted)
				Execute();
		}

		public abstract void Execute();
	}
}


