using System.Collections.ObjectModel;

namespace MA_L6;

public partial class NewLocationPage : ContentPage
{
    public record ProductInfoDto(ImageSource ThumbnailSource, ProductInfo Info);

    public ObservableCollection<ProductInfoDto> PointListItems { get; private set; } = [];
    public ProductInfo? SelectedItem { get; private set; }

    public NewLocationPage()
    {
        InitializeComponent();
        PointCollectionView.BindingContext = this;
        PointCollectionView.SetBinding(ItemsView.ItemsSourceProperty, "PointListItems");
    }

    private async void OnClick_Search(object? sender, EventArgs e)
    {
        PointListItems.Clear();

        SearchBtn.IsEnabled = false;
        var list = await FoodHttp.RequestProductSearch(NameEntry.Text);
        if (list is null)
        {
            // Toast с ошибкой
            return;
        }

        foreach (var point in list.Products)
        {
            ImageSource? source = null;
            if (point.ThumbnailURL is not null)
                source = ImageCache.CheckImageInCache(point.ThumbnailURL)
                       ? ImageCache.GetImageFromCache(point.ThumbnailURL)
                       : await FoodHttp.RequestProductImage(point.ThumbnailURL);

            PointListItems.Add(new(source!, point));
        }

        SearchBtn.IsEnabled = true;
    }

    private async void OnTapped(object? sender, EventArgs e)
    {
        var a = (Border)sender!;
        string idstr = ((Label)a.FindByName("IdHiddenLabel")).Text;
        long id = long.Parse(idstr);
        SelectedItem = PointListItems.Where(t => t.Info.Id == id).Select(t => t.Info).Single();

        //     SelectedItem = (LocationPoint) ((Frame)sender!).SelectedItem;
        // await Navigation.PopAsync();
        await Navigation.PushAsync(new NewPage1(SelectedItem));
    }
}