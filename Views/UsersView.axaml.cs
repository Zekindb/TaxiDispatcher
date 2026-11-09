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

public partial class UsersView : UserControl
{
    private User? _selectedUser;
    private List<Role> _roles = new();
    private List<User> _allUsers = new();
    private bool _isLoaded = false;

    public UsersView()
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
            _roles = await UserService.GetRolesAsync();
            RoleBox.ItemsSource = _roles;

            var users = await UserService.GetUsersAsync();
            _allUsers = users.Where(u => u.UserId != Session.CurrentUser?.UserId).ToList();
            ApplyFilters();
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }
    }

    private void ApplyFilters()
    {
        if (!_isLoaded || UsersList == null)
            return;

        var query = _allUsers.AsQueryable();
        
        string search = SearchBox?.Text ?? "";
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(u => u.FullName.Contains(search, StringComparison.OrdinalIgnoreCase) || u.Login.Contains(search, StringComparison.OrdinalIgnoreCase));

        bool isAsc = (SortOrder?.SelectedItem as ComboBoxItem)?.Tag?.ToString() == "1";
        query = isAsc ? query.OrderBy(u => u.LastName) : query.OrderByDescending(u => u.LastName);
        UsersList.ItemsSource = query.ToList();
    }

    private void SearchTextChanged(object? sender, TextChangedEventArgs e) => ApplyFilters();
    private void FilterSelectionChanged(object? sender, SelectionChangedEventArgs e) => ApplyFilters();

    private void ValidateName() 
    {
        string text = FullNameBox.Text?.Trim() ?? "";
        string[] parts = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        FullNameBox.Classes.Set("error", parts.Length < 2 || parts.Length > 3);
    }

    private void ValidateLogin() => LoginBox.Classes.Set("error", string.IsNullOrWhiteSpace(LoginBox.Text) || LoginBox.Text.Length < 3);
    private void ValidatePhone() => PhoneBox.Classes.Set("error", !IsValidPhone(PhoneBox.Text));
    private void ValidatePassword() => PasswordBox.Classes.Set("error", _selectedUser == null && string.IsNullOrWhiteSpace(PasswordBox.Text));
    private void ValidateRole() => RoleBox.Classes.Set("error", RoleBox.SelectedItem == null);

    private void FullNameTextChanged(object? sender, TextChangedEventArgs e) => ValidateName();
    private void LoginTextChanged(object? sender, TextChangedEventArgs e) => ValidateLogin();
    private void PhoneTextChanged(object? sender, TextChangedEventArgs e) => ValidatePhone();
    private void PasswordTextChanged(object? sender, TextChangedEventArgs e) => ValidatePassword();
    private void RoleSelectionChanged(object? sender, SelectionChangedEventArgs e) => ValidateRole();

    private void AddClick(object? sender, RoutedEventArgs e)
    {
        if (!Session.IsAdmin)
            return;

        _selectedUser = null;
        ModalTitle.Text = "Новый пользователь";
        PasswordBox.PlaceholderText = "Задайте пароль";
        
        FullNameBox.Text = "";
        LoginBox.Text = ""; PhoneBox.Text = ""; PasswordBox.Text = "";
        RoleBox.SelectedItem = _roles.FirstOrDefault(r => r.RoleName == "Диспетчер") ?? _roles.FirstOrDefault();
        
        ValidateName(); ValidateLogin(); ValidatePhone(); ValidatePassword(); ValidateRole();
        ModalOverlay.IsVisible = true;
    }

    private void EditClick(object? sender, RoutedEventArgs e)
    {
        if (!Session.IsAdmin)
            return;

        _selectedUser = (sender as Button)?.DataContext as User;
        if (_selectedUser == null)
            return;

        ModalTitle.Text = "Изменить пользователя";
        PasswordBox.PlaceholderText = "Оставьте пустым, чтобы не менять";
        
        FullNameBox.Text = _selectedUser.FullName;
        LoginBox.Text = _selectedUser.Login;
        PhoneBox.Text = _selectedUser.Phone;
        PasswordBox.Text = "";
        RoleBox.SelectedItem = _roles.FirstOrDefault(r => r.RoleId == _selectedUser.RoleId);
        
        ValidateName(); ValidateLogin(); ValidatePhone(); ValidatePassword(); ValidateRole();
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
        if (!Session.IsAdmin)
            return;

        string fullName = FullNameBox.Text?.Trim() ?? "";
        string[] parts = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        var errors = new List<string>();
        if (parts.Length < 2) errors.Add("Введите хотя бы Фамилию и Имя");
        else if (parts.Length > 3) errors.Add("Слишком много слов в ФИО (максимум 3: Фамилия Имя Отчество)");
        
        if (LoginBox.Classes.Contains("error")) errors.Add("Логин от 3 символов");
        if (PhoneBox.Classes.Contains("error")) errors.Add("Неверный формат телефона");
        if (RoleBox.Classes.Contains("error")) errors.Add("Выберите роль");
        if (PasswordBox.Classes.Contains("error")) errors.Add("Задайте пароль");

        if (errors.Count > 0) { MainWindow.Instance.ShowToasts(errors); return; }

        var role = RoleBox.SelectedItem as Role;

        string lastName = parts[0];
        string firstName = parts[1];
        string? middleName = parts.Length == 3 ? parts[2] : null;

        if (_selectedUser != null)
        {
            if (_selectedUser.FullName == fullName &&
                _selectedUser.Login == LoginBox.Text &&
                _selectedUser.Phone == PhoneBox.Text &&
                _selectedUser.RoleId == role?.RoleId &&
                string.IsNullOrWhiteSpace(PasswordBox.Text))
            {
                ModalOverlay.IsVisible = false;
                return; 
            }
        }

        try
        {
            var user = _selectedUser ?? new User();
            user.LastName = lastName;
            user.FirstName = firstName;
            user.MiddleName = middleName;

            user.Login = LoginBox.Text!;
            user.Phone = PhoneBox.Text!;
            user.RoleId = role!.RoleId;

            await UserService.SaveUserAsync(user, PasswordBox.Text ?? "");
            ModalOverlay.IsVisible = false;
            await LoadDataAsync();
            MainWindow.Instance.ShowToast("Пользователь сохранен!", false);
        }
        catch (Exception ex) { MainWindow.Instance.ShowToast("Ошибка: " + ex.Message); }
    }
}

