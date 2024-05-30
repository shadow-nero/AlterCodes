using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace AlterAFS
{

	public class AFSUnpacker
	{
		public static void AFSExtract(string AFSFile, string Dest, string[] ListPath)
		{

			using (FileStream AFSStream = new FileStream(AFSFile, FileMode.Open, FileAccess.ReadWrite))
			{
				using (BinaryReader AFSBinary = new BinaryReader(AFSStream))
				{
					byte[] Format = new byte[4];
					AFSBinary.Read(Format, 0, 4);
					string AFSHeader = Encoding.ASCII.GetString(Format);

					if (!AFSHeader.Contains("AFS"))
					{
						Console.WriteLine("Warning: Esse não e um arquivo AFS.");
						return;
					}

					uint SFiles = AFSBinary.ReadUInt32() + 1;
					//uint EntryBlock = AFSBinary.ReadUInt32();
					uint[] FilesLength = new uint[SFiles];
					uint[] OffsetData = new uint[SFiles];

					for (int i = 0; i < SFiles; i++)
					{
						OffsetData[i] = AFSBinary.ReadUInt32();
						FilesLength[i] = AFSBinary.ReadUInt32();
					}

					//AFSStream.Seek(OffsetData[0], SeekOrigin.Begin);

					for (int i = 0; i < SFiles - 1; i++)
					{

						string directoryPath = Path.GetDirectoryName(Path.Combine(Dest, ListPath[i]));
						if (Directory.Exists(directoryPath) == false)
						{
							Directory.CreateDirectory(directoryPath);
						}

						AFSStream.Seek(OffsetData[i], SeekOrigin.Begin);
						//Console.WriteLine("Offset: " + OffsetData[i] + " Tamanho: " + FilesLength[i]);
						//Console.WriteLine(i + ": " + ListPath[i]);
						using (FileStream TempFile = new FileStream(Path.Combine(Dest, ListPath[i]), FileMode.Create, FileAccess.Write))
						{

							byte[] BodyFile = new byte[FilesLength[i]];
							AFSBinary.Read(BodyFile, 0, BodyFile.Length);
							TempFile.Write(BodyFile, 0, BodyFile.Length);
							//Console.WriteLine("Extraindo o arquivo \"" + ListPath[i] + "\"");
						}
						//Console.Clear();
						//AFSStream.Seek(OffsetData[i], SeekOrigin.Begin);
						//Console.WriteLine(OffsetData[i]);
					}

					string PathTime = "Ext\\AFSTime";
					if (Directory.Exists(PathTime) == false)
					{
						Directory.CreateDirectory(PathTime);
					}

					AFSStream.Seek(OffsetData[SFiles - 1], SeekOrigin.Begin);
					
					using (FileStream TempFile = new FileStream(Path.Combine(PathTime, Path.GetFileNameWithoutExtension(Dest)), FileMode.Create, FileAccess.Write))
					{

						byte[] BodyFile = new byte[FilesLength[SFiles - 1]];
						AFSBinary.Read(BodyFile, 0, BodyFile.Length);
						TempFile.Write(BodyFile, 0, BodyFile.Length);
						//Console.WriteLine("Extraindo o arquivo \"" + ListPath[i] + "\"");
					}


				}
			}
		}
	}
}