using System.Net;
using System.Timers;

class Program
{
    private static long bytesReceived;
    private static DateTime startTime;

    static void Main(string[] args)
    {
        string url = "https://hil-speed.hetzner.com/100MB.bin";
        string destinationPath = "Temp/100mb.bin";

        Console.WriteLine("Создание временной папки...");

        if (Directory.Exists("Temp"))
        {
            Directory.Delete("Temp", true);
        }
        Directory.CreateDirectory("Temp");

        Console.WriteLine("Готово!");

        using (WebClient webClient = new WebClient())
        {
            webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
            webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;

            startTime = DateTime.Now;
            bytesReceived = 0;

            // Начинаем загрузку
            webClient.DownloadFileAsync(new Uri(url), destinationPath);

            // Ожидаем завершения загрузки
            Console.WriteLine("Загрузка файла...");
            Console.ReadLine(); // Ожидание ввода, чтобы программа не завершилась
        }
    }

    private static void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
        bytesReceived = e.BytesReceived;
        TimeSpan elapsedTime = DateTime.Now - startTime;
        double speed = bytesReceived / 1024.0 / elapsedTime.TotalSeconds;

        Console.Write($"\rПрогресс: {e.ProgressPercentage}% | Скорость: {speed:F2} КБ/с");
    }

    private static void WebClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
    {
        Console.WriteLine("Загрузка завершена!");
    }
}