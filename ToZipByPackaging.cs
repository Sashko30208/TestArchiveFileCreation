using System;
using System.IO;
using System.IO.Packaging;
using System.Collections.Generic;

namespace TestArchiveFileCreation
{
    class ToZipByPackaging
    {

        //
        // AddFileToZip("test.zip", @"C:\Users\Александр\Downloads\Git-2.31.1-64-bit.exe");

        //var l = new List<string>() {
        //    @"C:\Users\Александр\Downloads\Git-2.31.1-64-bit.exe",
        //    @"C:\Users\Александр\Downloads\Alt+СУД.xlsx"};
        //AddManyFilesToZip("test2.zip", l);
        //
        private const long BUF_SIZE = 4096;

        private static void AddFileToZip(string zipFileName, string fileToAdd)
        {
            using (Package zip = Package.Open(zipFileName, FileMode.OpenOrCreate))
            {
                string destFileName = ".\\" + Path.GetFileName(fileToAdd);
                Uri uri = PackUriHelper.CreatePartUri(new Uri(destFileName, UriKind.Relative));
                if (zip.PartExists(uri))
                {
                    zip.DeletePart(uri);
                }

                PackagePart part = zip.CreatePart(uri, "", CompressionOption.Normal);

                using (FileStream fileStream = new FileStream(fileToAdd, FileMode.Open, FileAccess.Read))
                {
                    using (Stream dest = part.GetStream())
                    {
                        CopyStream(fileStream, dest);
                    }
                }
            }
        }

        private static void AddManyFilesToZip(string zipFileName, List<string> filesToAdd)
        {
            using (Package zip = Package.Open(zipFileName, FileMode.OpenOrCreate))
            {
                foreach (var fileToAdd in filesToAdd)
                {
                    string destFileName = ".\\" + Path.GetFileName(fileToAdd);
                    Uri uri = PackUriHelper.CreatePartUri(new Uri(destFileName, UriKind.Relative));

                    if (zip.PartExists(uri))
                    {
                        zip.DeletePart(uri);
                    }

                    PackagePart part = zip.CreatePart(uri, "", CompressionOption.Normal);
                    using (FileStream fileStream = new FileStream(fileToAdd, FileMode.Open, FileAccess.Read))
                    {
                        using (Stream dest = part.GetStream())
                        {
                            CopyStream(fileStream, dest);
                        }
                    }
                }
            }
        }

        private static void CopyStream(FileStream inputStream, Stream outputStream)
        {
            long bufferSize = inputStream.Length < BUF_SIZE ? inputStream.Length : BUF_SIZE;
            byte[] buffer = new byte[bufferSize];
            int bytesRead = 0;
            long bytesWritten = 0;
            while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                outputStream.Write(buffer, 0, bytesRead);
                bytesWritten += bufferSize;
            }
        }
    }
}
