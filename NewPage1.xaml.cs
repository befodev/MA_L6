namespace MA_L6;

public partial class NewPage1 : ContentPage // О продукте
{
	private readonly ProductInfo _product;
    private bool _inFavs;

    private void ChangeFavsBtn()
    {
        AddToFavBtn.Text = _inFavs ? "Убрать из избранного 🌟" : "Добавить в избранное ⭐";
        AddToFavBtn.BackgroundColor = _inFavs ? Color.FromRgb(0, 128, 0) : Color.FromRgb(128, 192, 200);
    }

	public NewPage1(ProductInfo product)
	{
		InitializeComponent();

		_product = product;
        ProductNameLabel.Text = _product.Name;
        BrandNameLabel.Text = _product.Brand;
        //Label_Anti.Text = _product.

        Label_100_Carbohydrates.Text = product.Nutriments.Carbohydrates + " г";
        Label_100_Fat.Text = product.Nutriments.Fat + " г";
        Label_100_Proteins.Text = product.Nutriments.Proteins + " г";
        Label_100_Energy.Text = product.Nutriments.Energy + " ккал";

        if (product.Ingredients != null)
            Label_Ingridients.Text = product.Ingredients;

        if (product.Quantity.HasValue)
        {
            float quan = product.Quantity.Value;
            QuantityLabel.Text = quan.ToString() + " " + _product.QuantityUnit;
        }

        _inFavs = FavManager.CheckInFavs(product.Id);
        ChangeFavsBtn();
    }

    protected async override void OnAppearing()
    {
        if (_product.ImageURL != null)
        {
            ImageSource? source = ImageCache.CheckImageInCache(_product.ImageURL)
                        ? ImageCache.GetImageFromCache(_product.ImageURL)
                        : await FoodHttp.RequestProductImage(_product.ImageURL);
            if(source is not null)
                ProdImage.Source = await FoodHttp.RequestProductImage(_product.ImageURL);
        }
            
    }

    private void OnClick_AddToFav(object? sender, EventArgs e)
	{
        if (_inFavs)
        {
            FavManager.RemoveToFavs(_product.Id);
        }
        else 
        {
            FavManager.AddToFavs(_product);
        }

        _inFavs = !_inFavs;
        ChangeFavsBtn();
    }
}