using Avalonia.Controls;
using Avalonia.Interactivity;
using TaxiDispatcher.Models;
using TaxiDispatcher.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TaxiDispatcher.Views;

public partial class OrdersView : UserControl
{
    private Order? _selectedOrder;
    private List<Tariff> _tariffs = new();
    private bool _isLoaded = false;

    public OrdersView()
    {
        InitializeComponent();
        Loaded += async (s, e) => { _isLoaded = true; await LoadDataAsync(); };
    }

    private async Task LoadDataAsync()
    {
        try
        {
            _tariffs = await TariffService.GetTariffsAsync();
            if (TariffBox != null) TariffBox.ItemsSource = _tariffs;
            await LoadOrdersAsync();
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }
    }

    private async Task LoadOrdersAsync()
    {
        if (!_isLoaded || OrdersList == null)
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

    private async void SearchTextChanged(object? sender, TextChangedEventArgs e) => await LoadOrdersAsync();
    private async void FilterSelectionChanged(object? sender, SelectionChangedEventArgs e) => await LoadOrdersAsync();

    private void ValidateName() => ClientNameBox.Classes.Set("error", string.IsNullOrWhiteSpace(ClientNameBox.Text));
    private void ValidatePhone() => ClientPhoneBox.Classes.Set("error", !IsValidPhone(ClientPhoneBox.Text));
    private void ValidatePickup() => PickupBox.Classes.Set("error", string.IsNullOrWhiteSpace(PickupBox.Text) || PickupBox.Text.Trim().Length < 6);
    private void ValidateDestination() => DestinationBox.Classes.Set("error", string.IsNullOrWhiteSpace(DestinationBox.Text) || DestinationBox.Text.Trim().Length < 6);
    
    private void ValidateDistance() 
    {
        bool isValid = decimal.TryParse(DistanceBox.Text, out decimal dist) && dist > 0;
        DistanceBox.Classes.Set("error", !isValid);
        RecalculatePrice();
    }

    private void ValidateTariff() 
    {
        TariffBox.Classes.Set("error", TariffBox.SelectedItem == null);
        RecalculatePrice();
    }

    private void ClientNameTextChanged(object? sender, TextChangedEventArgs e) => ValidateName();
    private void ClientPhoneTextChanged(object? sender, TextChangedEventArgs e) => ValidatePhone();
    private void PickupTextChanged(object? sender, TextChangedEventArgs e) => ValidatePickup();
    private void DestinationTextChanged(object? sender, TextChangedEventArgs e) => ValidateDestination();
    private void DistanceTextChanged(object? sender, TextChangedEventArgs e) => ValidateDistance();
    private void TariffSelectionChanged(object? sender, SelectionChangedEventArgs e) => ValidateTariff();

    private void RecalculatePrice()
    {
        if (TariffBox == null || DistanceBox == null || PriceLabel == null)
            return;

        var tariff = TariffBox.SelectedItem as Tariff;
        if (decimal.TryParse(DistanceBox.Text, out decimal dist))
        {
            decimal price = TariffService.CalculatePrice(tariff, dist);
            PriceLabel.Text = $"{price:N2} руб.";
        }
        else PriceLabel.Text = "0,00 руб.";
    }

    private void NewOrderClick(object? sender, RoutedEventArgs e)
    {
        _selectedOrder = null;
        ModalTitle.Text = "Создание заказа";
        ClientNameBox.Text = ""; ClientPhoneBox.Text = ""; PickupBox.Text = ""; DestinationBox.Text = ""; DistanceBox.Text = "";
        TariffBox.SelectedIndex = -1;
        PriceLabel.Text = "0,00 руб.";
        
        ValidateName(); ValidatePhone(); ValidatePickup(); ValidateDestination(); ValidateDistance(); ValidateTariff();
        
        ModalOverlay.IsVisible = true;
    }

    private void EditOrderClick(object? sender, RoutedEventArgs e)
    {
        _selectedOrder = (sender as Button)?.DataContext as Order;
        if (_selectedOrder == null)
            return;

        ModalTitle.Text = $"Заказ №{_selectedOrder.OrderId}";
        ClientNameBox.Text = _selectedOrder.ClientName;
        ClientPhoneBox.Text = _selectedOrder.ClientPhone;
        PickupBox.Text = _selectedOrder.PickupAddress;
        DestinationBox.Text = _selectedOrder.DestinationAddress;
        DistanceBox.Text = _selectedOrder.DistanceKm?.ToString() ?? "";
        
        if (TariffBox != null) TariffBox.SelectedItem = _tariffs.FirstOrDefault(t => t.TariffId == _selectedOrder.TariffId);
        
        ValidateName(); ValidatePhone(); ValidatePickup(); ValidateDestination(); ValidateDistance(); ValidateTariff();
        ModalOverlay.IsVisible = true;
    }

    private void CancelClick(object? sender, RoutedEventArgs e) => ModalOverlay.IsVisible = false;

    private bool IsValidPhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        string digits = Regex.Replace(phone, @"[^\d]", "");
        return (digits.Length == 11 && (digits[0] == '7' || digits[0] == '8')) || (digits.Length == 10 && digits[0] == '9');
    }

    private async void SaveClick(object? sender, RoutedEventArgs e)
    {
        var errors = new List<string>();
        if (ClientNameBox.Classes.Contains("error")) errors.Add("Введите ФИО клиента");
        if (ClientPhoneBox.Classes.Contains("error")) errors.Add("Неверный формат телефона");
        if (PickupBox.Classes.Contains("error")) errors.Add("Адрес подачи слишком короткий (мин. 6 симв.)");
        if (DestinationBox.Classes.Contains("error")) errors.Add("Адрес назначения слишком короткий (мин. 6 симв.)");
        if (DistanceBox.Classes.Contains("error")) errors.Add("Введите корректную дистанцию");
        if (TariffBox.Classes.Contains("error")) errors.Add("Выберите тариф");

        if (errors.Count > 0) { MainWindow.Instance.ShowToasts(errors); return; }

        var tariff = TariffBox.SelectedItem as Tariff;
        decimal.TryParse(DistanceBox.Text, out decimal dist);

        if (_selectedOrder != null)
        {
            if (_selectedOrder.ClientName == ClientNameBox.Text &&
                _selectedOrder.ClientPhone == ClientPhoneBox.Text &&
                _selectedOrder.PickupAddress == PickupBox.Text &&
                _selectedOrder.DestinationAddress == DestinationBox.Text &&
                _selectedOrder.DistanceKm == dist &&
                _selectedOrder.TariffId == tariff?.TariffId)
            {
                ModalOverlay.IsVisible = false;
                return; 
            }
        }

        try
        {
            var order = _selectedOrder ?? new Order();
            order.ClientName = ClientNameBox.Text!;
            order.ClientPhone = ClientPhoneBox.Text!;
            order.PickupAddress = PickupBox.Text!;
            order.DestinationAddress = DestinationBox.Text!;
            order.TariffId = tariff?.TariffId;
            order.DistanceKm = dist;

            await OrderService.SaveOrderAsync(order);
            ModalOverlay.IsVisible = false;
            await LoadOrdersAsync();
            MainWindow.Instance.ShowToast("Заказ успешно сохранен!", false);
        }
        catch (Exception ex) { MainWindow.Instance.ShowToast("Ошибка: " + ex.Message); }
    }
}

