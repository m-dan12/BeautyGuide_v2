using Avalonia.Media.Imaging;

namespace BeautyGuide_v2.Interfaces;

public interface IImageLoaderService
{
    Bitmap LoadImage(string imagePath);
}