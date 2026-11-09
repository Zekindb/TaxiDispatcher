using Avalonia.Controls;
using Avalonia.Interactivity;
using TaxiDispatcher.Models;
using TaxiDispatcher.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaxiDispatcher.Views;

public partial class TariffsView : UserControl
{
    private Tariff? _selectedTariff;
    private List<CarClass> _carClasses = new();
    private List<Tariff> _allTariffs = new();
    private bool _isLoaded = false;

    public TariffsView()
    {
        InitializeComponent();
        Loaded += async (s, e) => { _isLoaded = true; await LoadDataAsync(); };
    }

    private async Task LoadDataAsync()
    {
        if (!Session.IsAdmin)
            return;

        try
        {
            using var db = Data.Db.Create();
            _carClasses = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(db.CarClasses);
            ClassBox.ItemsSource = _carClasses;
            
            _allTariffs = await TariffService.GetTariffsAsync();
            ApplyFilters();
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }
    }

    private void ApplyFilters()
    {
        if (!_isLoaded || TariffsList == null)
            return;

        var query = _allTariffs.AsQueryable();
        
        string search = SearchBox?.Text ?? "";
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(t => t.TariffName.Contains(search, StringComparison.OrdinalIgnoreCase));

        bool isAsc = (SortOrder?.SelectedItem as ComboBoxItem)?.Tag?.ToString() == "1";
        query = isAsc ? query.OrderBy(t => t.TariffName) : query.OrderByDescending(t => t.TariffName);
        TariffsList.ItemsSource = query.ToList();
    }

    private void SearchTextChanged(object? sender, TextChangedEventArgs e) => ApplyFilters();
    private void FilterSelectionChanged(object? sender, SelectionChangedEventArgs e) => ApplyFilters();

    private void ValidateName() => NameBox.Classes.Set("error", string.IsNullOrWhiteSpace(NameBox.Text));
    private void ValidateBaseFare() => BaseFareBox.Classes.Set("error", !decimal.TryParse(BaseFareBox.Text, out decimal val) || val < 0);
    private void ValidatePriceKm() => PriceKmBox.Classes.Set("error", !decimal.TryParse(PriceKmBox.Text, out decimal val) || val < 0);
    private void ValidateMinFare() => MinFareBox.Classes.Set("error", !decimal.TryParse(MinFareBox.Text, out decimal val) || val < 0);
    private void ValidateClass() => ClassBox.Classes.Set("error", ClassBox.SelectedItem == null);

    private void NameTextChanged(object? sender, TextChangedEventArgs e) => ValidateName();
    private void BaseFareTextChanged(object? sender, TextChangedEventArgs e) => ValidateBaseFare();
    private void PriceKmTextChanged(object? sender, TextChangedEventArgs e) => ValidatePriceKm();
    private void MinFareTextChanged(object? sender, TextChangedEventArgs e) => ValidateMinFare();
    private void ClassSelectionChanged(object? sender, SelectionChangedEventArgs e) => ValidateClass();

    private void AddClick(object? sender, RoutedEventArgs e)
    {
        if (!Session.IsAdmin)
            return;

        _selectedTariff = null;
        ModalTitle.Text = "Новый тариф";
        NameBox.Text = ""; BaseFareBox.Text = ""; PriceKmBox.Text = ""; MinFareBox.Text = "";
        ClassBox.SelectedIndex = 0;
        
        ValidateName(); ValidateBaseFare(); ValidatePriceKm(); ValidateMinFare(); ValidateClass();
        ModalOverlay.IsVisible = true;
    }

    private void EditClick(object? sender, RoutedEventArgs e)
    {
        if (!Session.IsAdmin)
            return;

        _selectedTariff = (sender as Button)?.DataContext as Tariff;
        if (_selectedTariff == null)
            return;

        ModalTitle.Text = "Изменить тариф";
        NameBox.Text = _selectedTariff.TariffName;
        BaseFareBox.Text = _selectedTariff.BaseFare.ToString();
        PriceKmBox.Text = _selectedTariff.PricePerKm.ToString();
        MinFareBox.Text = _selectedTariff.MinFare.ToString();
        ClassBox.SelectedItem = _carClasses.FirstOrDefault(c => c.ClassId == _selectedTariff.CarClassId);
        
        ValidateName(); ValidateBaseFare(); ValidatePriceKm(); ValidateMinFare(); ValidateClass();
        ModalOverlay.IsVisible = true;
    }

    private void CancelClick(object? sender, RoutedEventArgs e) => ModalOverlay.IsVisible = false;

    private async void SaveClick(object? sender, RoutedEventArgs e)
    {
        if (!Session.IsAdmin)
            return;
        
        var errors = new List<string>();
        if (NameBox.Classes.Contains("error")) errors.Add("Введите название");
        if (BaseFareBox.Classes.Contains("error")) errors.Add("Некорректная подача");
        if (PriceKmBox.Classes.Contains("error")) errors.Add("Некорректная цена за км");
        if (MinFareBox.Classes.Contains("error")) errors.Add("Некорректный минимум");
        if (ClassBox.Classes.Contains("error")) errors.Add("Выберите класс");

        if (errors.Count > 0) { MainWindow.Instance.ShowToasts(errors); return; }

        var carClass = ClassBox.SelectedItem as CarClass;
        decimal.TryParse(BaseFareBox.Text, out decimal baseFare);
        decimal.TryParse(PriceKmBox.Text, out decimal priceKm);
        decimal.TryParse(MinFareBox.Text, out decimal minFare);

        if (_selectedTariff != null)
        {
            if (_selectedTariff.TariffName == NameBox.Text &&
                _selectedTariff.BaseFare == baseFare &&
                _selectedTariff.PricePerKm == priceKm &&
                _selectedTariff.MinFare == minFare &&
                _selectedTariff.CarClassId == carClass?.ClassId)
            {
                ModalOverlay.IsVisible = false;
                return; 
            }
        }

        try
        {
            var t = _selectedTariff ?? new Tariff();
            t.TariffName = NameBox.Text!;
            t.BaseFare = baseFare;
            t.PricePerKm = priceKm;
            t.MinFare = minFare;
            t.CarClassId = carClass!.ClassId;

            await TariffService.SaveTariffAsync(t);
            ModalOverlay.IsVisible = false;
            await LoadDataAsync();
            MainWindow.Instance.ShowToast("Тариф успешно сохранен!", false);
        }
        catch (Exception ex) { MainWindow.Instance.ShowToast("Ошибка: " + ex.Message); }
    }
}
