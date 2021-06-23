using System;

namespace TestArchiveFileCreation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            try
            {
                _ = ToZipByIOCompression.CreateZipArchive(@"C:\Users\Александр\Downloads\video_20210525_133819.mp4", @"C:\Users\Александр\Downloads", @"video_20210525_133819.zip");
                Console.WriteLine("Архив создан. Тыкни для выхода");
            }
            catch (Exception e)
            { Console.WriteLine("При выполнении произошла ошибка: {0}", e.Message); }
            Console.ReadKey();
        }

    }
}
