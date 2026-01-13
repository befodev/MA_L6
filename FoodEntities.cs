using System.Globalization;
using System.Text.Json;

namespace MA_L6
{
    public static class JsonElementExtensions
    {
        public static JsonElement? GetPropertyOrNull(this JsonElement element, string key)
        {
            bool success = element.TryGetProperty(key, out JsonElement ret);
            return success ? ret : null;
        }

        public static string? GetString(this JsonElement? element)
        {
            if (element == null || element.HasValue == false) return null;
            return element.Value.GetString();
        }

        public static float? GetSingle(this JsonElement? element)
        {
            if (element == null || element.HasValue == false) return null;
            return element.Value.ValueKind switch
            {
                JsonValueKind.Number => element.Value.GetSingle(),
                JsonValueKind.String => float.Parse(element.GetString() ?? "0", CultureInfo.InvariantCulture),
                _ => throw new InvalidOperationException(),
            };
        }

        public static int? GetInt32(this JsonElement? element)
        {
            if (element == null || element.HasValue == false) return null;
            return element.Value.ValueKind switch
            {
                JsonValueKind.Number => element.Value.GetInt32(),
                JsonValueKind.String => int.Parse(element.GetString() ?? "0"),
                _ => throw new InvalidOperationException(),
            };
        }
    }

    public record ProductNutriments(float Carbohydrates, float Energy, float Fat, float Proteins)
    {
        // carbohydrates_100g (углеводы), energy-kcal_100g, fat_100g, proteins_100g

        private static float GetNutriment(JsonElement? nutriment)
        {
            return nutriment.GetSingle() ?? 0;
        }


        public static ProductNutriments FromJsonElement(JsonElement product)
        {
            return new ProductNutriments(GetNutriment(product.GetPropertyOrNull("carbohydrates_100g")),
                                         GetNutriment(product.GetPropertyOrNull("energy-kcal_100g")),
                                         GetNutriment(product.GetPropertyOrNull("fat_100g")),
                                         GetNutriment(product.GetPropertyOrNull("proteins_100g")));
        }
    }

    public record ProductInfo(long Id, string Name, string Brand, string? ThumbnailURL, string? ImageURL,
                              float? Quantity, string? QuantityUnit, ProductNutriments Nutriments, string? Ingredients)
    {
        // id => Id
        // brand => Brand
        // generic_name => Name
        // image_front_thumb_url => ThumbnailURL
        // image_url => ImageURL
        // serving_quantity => Quantity
        // nutriments => Nutriments
        // ingredients[i].text => Ingredients

        // Калорийность, питательная ценность, ингредиенты, противопоказания
        public static ProductInfo FromRawJson(string json) // Если инфы про продукты напрямую
        {
            JsonDocument doc = JsonDocument.Parse(json);
            JsonElement product = doc.RootElement.GetProperty("product");
            return FromJsonElement(product);
        }



        public static ProductInfo FromJsonElement(JsonElement product)
        {

            long a = long.Parse(product.GetPropertyOrNull("id").GetString() ?? "0");
            string b = product.GetPropertyOrNull("product_name").GetString() ?? product.GetPropertyOrNull("generic_name").GetString() ?? "Без названия";
            string c = product.GetPropertyOrNull("brands").GetString() ?? "Без названия";
            string? d = product.GetPropertyOrNull("image_front_thumb_url").GetString();
            string? e = product.GetPropertyOrNull("image_url").GetString();
            float? f = product.GetPropertyOrNull("serving_quantity").GetSingle();
            string? f2 = product.GetPropertyOrNull("serving_quantity_unit").GetString();
            ProductNutriments g = ProductNutriments.FromJsonElement(product.GetProperty("nutriments"));
            string? h = product.GetPropertyOrNull("ingredients_text").GetString() ?? "Неизвестно";

            ProductInfo info = new(a, b, c, d, e, f, f2, g, h);
            return info;
        }
    }

    public record SearchProductResponse(int Count, int Page, int PageCount, int PageSize, IReadOnlyList<ProductInfo> Products)
    {
        public static SearchProductResponse FromRawJson(string json)
        {
            JsonDocument doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;

            IReadOnlyList<ProductInfo> products = [.. root.GetProperty("products").EnumerateArray()
                                                          .Select(ProductInfo.FromJsonElement)];

            int a = root.GetPropertyOrNull("count").GetInt32() ?? 0;
            int b = root.GetPropertyOrNull("page").GetInt32() ?? 0;
            int c = root.GetPropertyOrNull("page_count").GetInt32() ?? 0;
            int d = root.GetPropertyOrNull("page_size").GetInt32() ?? 0;
            SearchProductResponse resp = new(a, b, c, d, products);
           

            return resp;
        }
    }
}
