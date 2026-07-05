namespace BingoNumbers;

using BingoNumbers.Views;

public partial class App : Application
{
	public App()
	{

		InitializeComponent();

    } //App

    protected override Window CreateWindow(IActivationState? activationState)
	{

		return new BingoNumbersWindow(new MainPage());

    } //Window

} //App
