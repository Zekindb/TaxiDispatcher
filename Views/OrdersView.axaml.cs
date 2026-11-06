using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using TaxiDispatcher.Models;
using TaxiDispatcher.Services;

namespace TaxiDispatcher.Views;

public partial class OrdersView : UserControl
{
    private Order? _selectedOrder;
    private bool _isLoaded = false;

    public OrdersView()
    {
        InitializeComponent();
        Loaded += async (s, e) => { _isLoaded = true; await LoadOrdersAsync(); };
    }

    private async Task LoadOrdersAsync()
    {
        if (!_isLoaded)
            return;

        try
        {
            string search = SearchBox?.Text ?? "";
            int statusId = (StatusFilter?.SelectedItem as ComboBoxItem)?.Tag is string tag ? int.Parse(tag) : 0;
            bool isDesc = (SortOrder?.SelectedItem as ComboBoxItem)?.Tag?.ToString() == "1";

            OrdersList.ItemsSource = await OrderService.GetOrdersAsync(search, statusId, isDesc);
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }
    }

    private async void SearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        await LoadOrdersAsync();
    }

    private async void FilterSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        await LoadOrdersAsync();
    }

    private void NewOrderClick(object? sender, RoutedEventArgs e)
    {
        _selectedOrder = null;
        ModalTitle.Text = "Создание нового заказа";
        ClientNameBox.Text = ""; ClientPhoneBox.Text = ""; PickupBox.Text = ""; DestinationBox.Text = ""; DistanceBox.Text = "";
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

    private void CancelClick(object? sender, RoutedEventArgs e) => ModalOverlay.IsVisible = false;

    private async void SaveClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ClientNameBox.Text))
            {
                Console.WriteLine("Введите ФИО");
                return;
            }

            var order = _selectedOrder ?? new Order();
            order.ClientName = ClientNameBox.Text;
            order.ClientPhone = ClientPhoneBox.Text ?? "";
            order.PickupAddress = PickupBox.Text ?? "";
            order.DestinationAddress = DestinationBox.Text ?? "";
            
            if (decimal.TryParse(DistanceBox.Text, out decimal dist))
                order.DistanceKm = dist;

            await OrderService.SaveOrderAsync(order);
            
            ModalOverlay.IsVisible = false;
            await LoadOrdersAsync();
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }
    }
}
