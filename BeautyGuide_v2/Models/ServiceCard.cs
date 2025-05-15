using System.Collections.Generic;
using Avalonia.Media.Imaging;

namespace BeautyGuide_v2.Models;

public class ServiceCard
{
    public Service Service { get; set; }
    public List<string> Photos { get; set; }
    public Master Master { get; set; }
    public Salon Salon { get; set; }
    public Bitmap MainPhoto { get; set; }
    public Bitmap MasterPhoto { get; set; }
}