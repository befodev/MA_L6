using System.Globalization;
using System.IO;

namespace MA_L6
{
    internal class FoodHttp
    {
        private static readonly HttpClient s_Client = new();

        private async static Task<HttpContent?> RequestToServer(string url)
        {
            try
            {
                HttpResponseMessage response = await s_Client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return response.Content;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        private static string GetInfoURL(int Id)
            => $"https://world.openfoodfacts.net/api/v2/product/{Id}.json?fields=nutriments,id,image_url,image_front_thumb_url,serving_quantity,serving_quantity_unit,ingredients_text,generic_name,brands,product_name";

        public static string GetSearchURL(string text)
    => $"https://world.openfoodfacts.org/cgi/search.pl?search_terms={text}&search_simple=1&action=process&json=1&page_size=5&"
            + "fields=nutriments,id,image_url,image_front_thumb_url,serving_quantity,serving_quantity_unit,ingredients_text,generic_name,brands,product_name";

        public async static Task<ProductInfo?> RequestProductInfo(int id)
        {
            string url = GetInfoURL(id);
            string? resp = null;
            if (!ImageCache.CheckImageInCache(url))
            {
                HttpContent? cnt = await RequestToServer(url);
                if (cnt == null) return null;
                resp = await cnt.ReadAsStringAsync();
                if (resp is not null)
                    ImageCache.SaveJsonToCache(url, resp);
            }
            else resp = ImageCache.GetJsonFromCache(url);
            return resp is null ? null : ProductInfo.FromRawJson(resp);
        }

        public async static Task<SearchProductResponse?> RequestProductSearch(string name)
        {
            string url = GetSearchURL(name);
            string? resp = null;
            if (!ImageCache.CheckImageInCache(url))
            {
                HttpContent? cnt = await RequestToServer(url);
                if (cnt == null) return null;
                resp = await cnt.ReadAsStringAsync();
                if (resp is not null)
                    ImageCache.SaveJsonToCache(url, resp);
            }
            else resp = ImageCache.GetJsonFromCache(url);
            return resp is null ? null : SearchProductResponse.FromRawJson(resp);
        }

        public async static Task<ImageSource?> RequestProductImage(string? url)
        {
            if (url == null) return null;
            HttpContent? cnt = await RequestToServer(url);
            if (cnt == null) return null;
            Stream stream = await cnt.ReadAsStreamAsync();
            ImageCache.SaveImageToCache(url, stream);
            return ImageSource.FromStream(() => stream);
        }
    }
}
