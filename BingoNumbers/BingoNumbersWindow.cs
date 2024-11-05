using BingoNumbers.ViewModels;
using BingoNumbers.Views;

namespace BingoNumbers;

public class BingoNumbersWindow : Window
{
	private readonly MainPage? _mainPage;

	/// <summary>
	/// inherited window class to enable responses to app lifecycle events, e.g. 
	/// call main page view model's save state method on app shutdown or deactivation
	/// and restore view model state on app launch
	/// </summary>
	public BingoNumbersWindow() : base() { }
	public BingoNumbersWindow(MainPage page) : base(page) 
	{

		_mainPage = page;

	} //BingoNumbersWindow


	/// <summary>
	/// called on app launch before main page visible
	/// </summary>
	protected override void OnCreated()
	{
		if (_mainPage is not null)
		{
			((MainViewModel)_mainPage.BindingContext).RestoreState();
		}

	} //OnCreated
	

	/// <summary>
	/// called when app loses focus, sent to background
	/// </summary>
	protected override void OnDeactivated()
	{

		if (_mainPage is not null)
		{
			((MainViewModel)_mainPage.BindingContext).SaveState();
		}

	} //OnDeactivated


	/// <summary>
	/// called when app is shutting down
	/// </summary>
	protected override void OnDestroying()
	{
		if (_mainPage is not null)
		{
			((MainViewModel)_mainPage.BindingContext).SaveState();
		}

	} //OnDestroying


} //BingoNumbersWindow