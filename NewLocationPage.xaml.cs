using MA_L6;
using System.Collections.ObjectModel;

namespace MA_L5;

public partial class NewLocationPage : ContentPage
{
    public ObservableCollection<ProductInfo> PointListItems { get; private set; } = [];
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

        Console.WriteLine("A");

        var list = await FoodHttp.RequestProductSearch(NameEntry.Text);
        if (list is null)
        {
            Console.WriteLine("A bad");
            // Toast с ошибкой
            return;
        }

        Console.WriteLine("B");

        foreach (var point in list.Products)
            PointListItems.Add(point);
        Console.WriteLine("C");
    }

    private async void OnTapped(object? sender, EventArgs e)
    {
        var a = (Border)sender!;
        string idstr = ((Label)a.FindByName("IdHiddenLabel")).Text;
        int id = int.Parse(idstr);
        SelectedItem = PointListItems.Where(t => t.Id == id).Single();

        //     SelectedItem = (LocationPoint) ((Frame)sender!).SelectedItem;
        await Navigation.PopAsync();
    }
}