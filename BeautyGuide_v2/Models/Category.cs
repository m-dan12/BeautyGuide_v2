using System.Collections.Generic;

namespace BeautyGuide_v2.Models;

public class Category
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<Category> SubCategories { get; set; } = new List<Category>();
}