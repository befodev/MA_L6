using System.Globalization;

namespace MA_L6
{
    internal class FoodHttp
    {
        private static readonly HttpClient s_Client = new();

        private async static Task<string?> RequestToServer(string url)
        {
            try
            {
                HttpResponseMessage response = await s_Client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string body = await response.Content.ReadAsStringAsync();
                return body;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        private static string GetInfoURL(int Id)
            => $"https://world.openfoodfacts.net/api/v2/product/{Id}.json?fields=nutriments,id,image_url,image_front_thumb_url,serving_quantity,ingredients_text,generic_name,brands";

        public static string GetSearchURL(string text)
    => $"https://world.openfoodfacts.org/cgi/search.pl?search_terms={text}&search_simple=1&action=process&json=1&page_size=5&"
            + "fields=nutriments,id,image_url,image_front_thumb_url,serving_quantity,ingredients_text,generic_name,brands";

        public async static Task<ProductInfo?> RequestProductInfo(int id)
        {
            string url = GetInfoURL(id);
            string? resp = await RequestToServer(url);
            return resp is null ? null : ProductInfo.FromRawJson(resp);
        }

        public async static Task<SearchProductResponse?> RequestProductSearch(string name)
        {
            string url = GetSearchURL(name);
            string? resp = await RequestToServer(url);
            return resp is null ? null : SearchProductResponse.FromRawJson(resp);
        }
    }
}
