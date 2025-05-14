using System.Collections.Generic;
using Avalonia.Media.Imaging;

namespace BeautyGuide_v2.Models;

public class ServiceCard
{
    public Service Service { get; set; }
    public List<Bitmap> Photos { get; set; } // Список URL фотографий услуги
    public Master Master { get; set; }
    public Salon Salon { get; set; }
}