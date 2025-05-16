using System.Threading.Tasks;

namespace BeautyGuide_v2.Interfaces;

public interface IInitializableViewModel
{
    Task LoadAsync(string parameter = null);
}