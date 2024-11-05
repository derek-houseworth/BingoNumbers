namespace BingoNumbers;

using BingoNumbers.Views;
using BingoNumbers.ViewModels;

public partial class App : Application
{
	public App(MainPage page)
	{

		InitializeComponent();
		//MainPage = new AppShell();

	}
	protected override Window CreateWindow(IActivationState? activationState)
	{

		return new BingoNumbersWindow(new MainPage(new MainViewModel()));


		/*
		Window window = base.CreateWindow(activationState);

		window.Created += (s, e) => 
		{
			Debug.WriteLine("Insulter.App: 1. Created event");
		};
		window.Activated += (s, e) => 
		{
			Debug.WriteLine("Insulter.App: 2. Activated event");
		};
		window.Deactivated += (s, e) => 
		{
			Debug.WriteLine("Insulter.App: 3. Deactivated event");
		};
		window.Stopped += (s, e) => 
		{
			Debug.WriteLine("Insulter.App: 4. Stopped event");
		};
		window.Resumed += (s, e) => 
		{
			Debug.WriteLine("Insulter.App: 5. Resumed event");
		};
		window.Destroying += (s, e) => 
		{
			Debug.WriteLine("Insulter.App: 6. Destroying event");
		};

		return window;
		*/
	}

}
