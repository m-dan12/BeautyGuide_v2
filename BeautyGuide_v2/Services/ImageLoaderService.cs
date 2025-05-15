using System;
using System.IO;
using Avalonia.Media.Imaging;
using BeautyGuide_v2.Interfaces;

namespace BeautyGuide_v2.Services;

public class ImageLoaderService : IImageLoaderService
{
    private readonly string _assemblyName = "BeautyGuide_v2"; // Замените на имя вашей сборки

    public Bitmap LoadImage(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath)) return null;

        try
        {
            // Удаляем начальный слеш, если он есть
            imagePath = imagePath.TrimStart('/');

            // Проверка файла в выходной директории
            var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imagePath);
            if (File.Exists(fullPath))
            {
                Console.WriteLine($"Загружаю изображение из файла: {fullPath}");
                return new Bitmap(fullPath);
            }

            // Загрузка как встроенного ресурса
            string resourceUri = $"avares://{_assemblyName}/{imagePath}";
            Console.WriteLine($"Загружаю изображение как ресурс: {resourceUri}");
            return new Bitmap(resourceUri);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки изображения {imagePath}: {ex.Message}");
            return null; // Или можно вернуть заглушку (placeholder)
        }
    }
}