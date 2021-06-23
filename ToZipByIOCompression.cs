using System;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestArchiveFileCreation
{
    internal class ToZipByIOCompression
    {
        private const int maxArchiveSize = 5 * 1024/*Kb*/ * 1024/*Mb*/; // можно создавать архивы не более 'N' Мб.
        private const long BUF_SIZE = 4096;                             // размер буфера для архивирования

        internal static async Task<List<string>> CreateZipArchive(string fileToAdd, string archiveDirectoryToSave, string archiveName = "")
        {
            List<string> archives = new List<string>();
            if (!File.Exists(fileToAdd))
            {
                throw new FileNotFoundException($"Файл {fileToAdd} не найден. Архивация невозможна");
            }
            using (MemoryStream zipStream = new MemoryStream())
            {
                try
                {
                    using (ZipArchive zip = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
                    {
                        string name = fileToAdd.Split(@"\")?.LastOrDefault();
                        zip.CreateEntryFromFile(fileToAdd, name, CompressionLevel.Optimal);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка при попытке чтения файла для архивирования. ({fileToAdd}) ", ex);
                }

                try
                {
                    // Определение количества томов архива.
                    byte partsCount = (byte)(zipStream.Length / maxArchiveSize);
                    partsCount = (byte)((zipStream.Length / (maxArchiveSize * 1.0) != 0) ? partsCount + 1 : partsCount);

                    long maxSegmentSize = maxArchiveSize % BUF_SIZE == 0 ? maxArchiveSize : (maxArchiveSize / BUF_SIZE) * BUF_SIZE;         // Максимум байт в сегменте (томе)

                    int bufferSize = (checked((int)(zipStream.Length < BUF_SIZE ? zipStream.Length : BUF_SIZE)));                           //Stream.Read не может читать long. Потому int
                    byte[] buffer = new byte[bufferSize];

                    zipStream.Position = 0;

                    for (byte b = 1; b <= partsCount; b++)
                    {
                        string fullArchiveName = archiveName + (partsCount > 1 ? @$".00{b}" : "");                                          // Adds Suffix

                        fullArchiveName = string.IsNullOrEmpty(archiveDirectoryToSave) ? fullArchiveName :
                                                                                         archiveDirectoryToSave + "\\" + fullArchiveName;   // Adds directory

                        if (File.Exists(fullArchiveName))
                        {
                            File.Delete(fullArchiveName);
                        }

                        long bytesWritten = 0;
                        int bytesRead = 0;

                        using (FileStream fs = new FileStream(fullArchiveName, FileMode.Create))
                        {
                            while ((bytesRead = await zipStream.ReadAsync(buffer, 0, bufferSize)) != 0)
                            {
                                await fs.WriteAsync(buffer, 0, bytesRead);
                                bytesWritten += bytesRead;

                                if (bytesWritten >= maxSegmentSize)
                                { break; }
                            }
                        }
                        archives.Add(fullArchiveName);
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Ошибка при архивации. ", e);
                }
            }
            return archives;
        }
    }
}
