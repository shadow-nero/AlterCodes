using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace AlterAFS
{

	public class EbootPath
	{
		const int RAMDIFF = 0xFFF80;

		public static string[] DecompileEboot(string EbootPath, string AfsName)
		{
			// Divide a string de entrada usando o delimitador especificado
			using (FileStream fs = new FileStream(EbootPath, FileMode.Open))
			{
				using (BinaryReader br = new BinaryReader(fs))
				{
					fs.Seek(0x2d5b50, SeekOrigin.Begin);

					int[] namePtr = new int[3];
					int[] pathArrLen = new int[3];
					int[] pathArrPtr = new int[3];
					int TypeAFS = 0;

					for (int i = 0; i < 3; i++)
					{
						namePtr[i] = br.ReadInt32() - RAMDIFF;
						pathArrLen[i] = br.ReadInt32();
						pathArrPtr[i] = br.ReadInt32() - RAMDIFF;
					}

					for (int i = 0; i < 3; i++)
					{
						fs.Seek(namePtr[i], SeekOrigin.Begin);
						string result = ReadUntilNullTerminator(fs);
						string AFSLower = Path.GetFileName(AfsName.ToLower());
						if (result == AFSLower) TypeAFS = i;

					}

					fs.Seek(pathArrPtr[TypeAFS], SeekOrigin.Begin);

					string[] NamesArray = new string[pathArrLen[TypeAFS]];

					for (int i = 0; i < pathArrLen[TypeAFS]; i++)
					{
						int NAME_PTR = br.ReadInt32() - RAMDIFF;
						long tmp = fs.Position;

						if (NAME_PTR < 0 || NAME_PTR > fs.Length)
						{
							throw new ArgumentOutOfRangeException($"Offset {NAME_PTR} is out of file bounds.");
						}

						fs.Seek(NAME_PTR, SeekOrigin.Begin);

						//byte[] bytesName = new byte[32];
						//fs.Read(bytesName, 0, bytesName.Length);
						/////////
						byte[] buffer = new byte[1024];
						int bytesRead = 0;
						while (true)
						{
							byte b = br.ReadByte();
							if (b == 0x00)
							{
								break;
							}
							buffer[bytesRead] = b;
							bytesRead++;
						}
						string txt = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead).TrimEnd('\0');


						//string StringName = Encoding.UTF8.GetString(bytesName).TrimEnd('\0');

						NamesArray[i] = txt;

						fs.Seek((int)tmp, SeekOrigin.Begin);

					}

					return NamesArray;

				}

			}

		}
		static string ReadUntilNullTerminator(FileStream fs)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				int byteRead;
				while ((byteRead = fs.ReadByte()) != -1)
				{
					if (byteRead == 0x00)
					{
						return Encoding.UTF8.GetString(ms.ToArray());
					}
					ms.WriteByte((byte)byteRead);
				}
			}

			return null;
		}
	}
}
