using BingoNumbers.ViewModels;

namespace BingoNumbers;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
    }
	  
}
