using System.Collections.ObjectModel;

namespace MA_L6
{
    public partial class MainPage : ContentPage
    {
        public record ProductInfoDto(ImageSource ThumbnailSource, ProductInfo Info);

        public ObservableCollection<ProductInfoDto> PointListItems { get; private set; } = [];
        public ProductInfo? SelectedItem { get; private set; }

        public MainPage()
        {
            InitializeComponent();
            FavManager.LoadFavs();
            ImageCache.InitialiseStorage();
            PointCollectionView.BindingContext = this;
            PointCollectionView.SetBinding(ItemsView.ItemsSourceProperty, "PointListItems");
        }

        protected override async void OnAppearing()
        {
            UpdateFavList();
        }

        private async void OnCounterClicked(object? sender, EventArgs e)
        {
            NewLocationPage page = new();
            await Navigation.PushAsync(page);
        }

        private async void UpdateFavList()
        {
            PointListItems.Clear();
            foreach (var point in FavManager.Favs)
            {
                ImageSource? source = null;
                if (point.ThumbnailURL is not null)
                    source = ImageCache.CheckImageInCache(point.ThumbnailURL)
                           ? ImageCache.GetImageFromCache(point.ThumbnailURL)
                           : await FoodHttp.RequestProductImage(point.ThumbnailURL);

                PointListItems.Add(new(source!, point));
            }
        }

        private async void OnTapped(object? sender, EventArgs e)
        {
            var a = (Border)sender!;
            string idstr = ((Label)a.FindByName("IdHiddenLabel")).Text;
            long id = long.Parse(idstr);
            SelectedItem = PointListItems.Where(t => t.Info.Id == id).Select(t => t.Info).Single();

            //     SelectedItem = (LocationPoint) ((Frame)sender!).SelectedItem;
            // await Navigation.PopAsync();

            Page page = new NewPage1(SelectedItem);
            page.Disappearing += (sender, e) =>
            {
                UpdateFavList();
            };
            await Navigation.PushAsync(page);
        }
    }
}
