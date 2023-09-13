namespace BingoNumbers;

using BingoNumbers.Views;
using BingoNumbers.ViewModels;

public partial class App : Application
{
	public App(MainPage page)
	{

		InitializeComponent();
		
    }
	protected override Window CreateWindow(IActivationState activationState)
	{

		return new BingoNumbersWindow(new MainPage(new MainViewModel()));

	}

}
