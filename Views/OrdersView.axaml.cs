using Avalonia.Controls;
using Avalonia.Interactivity;
using TaxiDispatcher.Models;
using TaxiDispatcher.Services;

namespace TaxiDispatcher.Views;

public partial class OrdersView : UserControl
{
    private Order? _selectedOrder;

    public OrdersView()
    {
        InitializeComponent();
        Loaded += async (s, e) => await LoadOrdersAsync();
    }

    private async System.Threading.Tasks.Task LoadOrdersAsync()
    {
        try
        {
            OrdersList.ItemsSource = await OrderService.GetOrdersAsync();
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine(ex.Message);
        }
    }

    private async void ReloadClick(object? sender, RoutedEventArgs e)
    {
        await LoadOrdersAsync();
    }

    private void NewOrderClick(object? sender, RoutedEventArgs e)
    {
        _selectedOrder = null;
        ModalTitle.Text = "Создание нового заказа";

        ClientNameBox.Text = "";
        ClientPhoneBox.Text = "";
        PickupBox.Text = "";
        DestinationBox.Text = "";
        DistanceBox.Text = "";

        ModalOverlay.IsVisible = true;
    }

    private void EditOrderClick(object? sender, RoutedEventArgs e)
    {
        var btn = sender as Button;
        _selectedOrder = btn?.DataContext as Order;
        if (_selectedOrder == null)
            return;

        ModalTitle.Text = $"Редактирование заказа №{_selectedOrder.OrderId}";

        ClientNameBox.Text = _selectedOrder.ClientName;
        ClientPhoneBox.Text = _selectedOrder.ClientPhone;
        PickupBox.Text = _selectedOrder.PickupAddress;
        DestinationBox.Text = _selectedOrder.DestinationAddress;
        DistanceBox.Text = _selectedOrder.DistanceKm?.ToString() ?? "";

        ModalOverlay.IsVisible = true;
    }

    private void CancelClick(object? sender, RoutedEventArgs e)
    {
        ModalOverlay.IsVisible = false;
    }

    private void SaveClick(object? sender, RoutedEventArgs e)
    {
        ModalOverlay.IsVisible = false;
        // Empty
    }
}
