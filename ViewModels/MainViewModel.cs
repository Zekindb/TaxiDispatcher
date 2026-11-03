using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using TaxiDispatcher.Views;

namespace TaxiDispatcher.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private UserControl _currentPage = null!;
    public UserControl CurrentPage
    {
        get => _currentPage;
        set { _currentPage = value; OnPropertyChanged(); }
    }

    public MainViewModel() => CurrentPage = new LoginView();

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
