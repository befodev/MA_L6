using System.Text.Json;

namespace MA_L6
{
    public static class FavManager
    {
        private static List<ProductInfo> s_Products = [];
        public static void LoadFavs()
        {
            string? json = Preferences.Default.Get("favs", "");
            if (!string.IsNullOrEmpty(json))
                s_Products = JsonSerializer.Deserialize<List<ProductInfo>>(json) ?? [];

        }

        private static void SaveFavs()
        {
            var options = new JsonSerializerOptions
            {
#if DEBUG
                WriteIndented = true,
#endif // DEBUG
                IgnoreReadOnlyProperties = true
            };

            string output = JsonSerializer.Serialize(s_Products);
            Preferences.Default.Set("favs", output);
        }

        public static void AddToFavs(ProductInfo product)
        {
            s_Products.Add(product);
            SaveFavs();
        }

        public static void RemoveToFavs(long id)
        {
            s_Products.RemoveAt(s_Products.FindIndex(0, t => t.Id == id));
            SaveFavs();
        }

        public static bool CheckInFavs(long id)
        {
            return s_Products.Any(t => id == t.Id);
        }

        public static IEnumerable<ProductInfo> Favs => s_Products;
    }
}
