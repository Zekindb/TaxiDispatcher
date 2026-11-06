using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using TaxiDispatcher.Services;

namespace TaxiDispatcher.Views;

public partial class LoginView : UserControl
{
    public LoginView() => InitializeComponent();

    private void LoginView_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            LoginClick(sender, new RoutedEventArgs());
        }
    }

    private async void LoginClick(object? sender, RoutedEventArgs e)
    {
        var login = LoginBox.Text?.Trim();
        var password = PasswordBox.Text;

        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
        {
            ErrorText.Text = "Введите логин и пароль."; return;
        }

        var user = await AuthService.LoginAsync(login, password);
        if (user == null)
        {
            ErrorText.Text = "Неверный логин или пароль."; return;
        }

        Session.SetCurrentUser(user);

        var main = (MainWindow)TopLevel.GetTopLevel(this)!;
        main.ShowMenu();
        (main.DataContext as ViewModels.MainViewModel)?.CurrentPage = new OrdersView();
    }
}
