using BingoNumbers.Services;
using BingoNumbers.ViewModels;

namespace BingoNumbers.Views;
 

public partial class MainPage : ContentPage
{

	public MainPage()
	{

        InitializeComponent();

		BindingContext = new MainViewModel(new PreferencesService());
    }
}
