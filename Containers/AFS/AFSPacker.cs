using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace AlterAFS
{

    public class AFSPacker
    {
        public static void AFSRepack(string FolderAFS, string Dest, string[] ListPath)
        {
            try
            {
                // Abre o arquivo para escrita, criando-o se não existir
                using (FileStream fs = new FileStream(Dest, FileMode.Create))
                {
                    // Cria um BinaryWriter para escrever no arquivo
                    using (BinaryWriter writer = new BinaryWriter(fs))
                    {
                        // Escreve "AFS" no cabeçalho do arquivo
                        int SizeList = ListPath.Length + 1;
                        writer.Write(Encoding.UTF8.GetBytes("AFS"));

                        // Escreve o byte 0x00 imediatamente após "AFS"
                        writer.Write((byte)0x00);
                        writer.Write(SizeList);

                        int HeaderEntry = CalculateEntry(SizeList * 4 * 2 + 8);
                        byte[] HeaderEntryByte = new byte[SizeList * 4 * 2 + HeaderEntry];
                        writer.Write(HeaderEntryByte);
                        //List<uint> new ListPosition 
                        uint[] FilesOffset = new uint[SizeList];
                        uint[] FilesSizes = new uint[SizeList];

                        for (int i = 0; i < SizeList - 1; i++)
                        {
                            using (FileStream TempFile = new FileStream(Path.Combine(FolderAFS, ListPath[i]), FileMode.Open, FileAccess.Read))
                            {
                                byte[] BodyFile = new byte[TempFile.Length];
                                TempFile.Read(BodyFile, 0, BodyFile.Length);

                                FilesOffset[i] = (uint)fs.Position;
                                FilesSizes[i] = (uint)BodyFile.Length;

                                writer.Write(BodyFile, 0, BodyFile.Length);
                                int FileEntry = CalculateEntry(BodyFile.Length);
                                byte[] FileEntryByte = new byte[FileEntry];
                                writer.Write(FileEntryByte);

                            }
                        }

                        string PathTime = "Ext\\AFSTime";
                        if (Directory.Exists(PathTime) == false)
                        {
                            Directory.CreateDirectory(PathTime);
                        }

                        using (FileStream TempFile = new FileStream(Path.Combine(PathTime, Path.GetFileNameWithoutExtension(FolderAFS)), FileMode.Open, FileAccess.Read))
                        {
                            byte[] BodyFile = new byte[TempFile.Length];
                            TempFile.Read(BodyFile, 0, BodyFile.Length);

                            FilesOffset[SizeList - 1] = (uint)fs.Position;
                            FilesSizes[SizeList - 1] = (uint)BodyFile.Length;

                            writer.Write(BodyFile, 0, BodyFile.Length);
                            /*
                            int FileEntry = CalculateEntry(BodyFile.Length);
                            byte[] FileEntryByte = new byte[FileEntry];
                            writer.Write(FileEntryByte);
                            */
                        }

                        fs.Seek(0x8, SeekOrigin.Begin);
                        for (int i = 0; i < SizeList; i++)
                        {
                            writer.Write(FilesOffset[i]);
                            writer.Write(FilesSizes[i]);
                        }

                    }
                }

                Console.WriteLine("Cabeçalho 'AFS\\0' escrito no arquivo com sucesso.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocorreu um erro ao escrever no arquivo: " + ex.Message);
            }
        }
        static int CalculateEntry(int currentSize)
        {
            int blockSize = 2048;
            int remainder = currentSize % blockSize;
            if (remainder == 0)
            {
                return 0;
            }
            return blockSize - remainder;
        }

    }
}


