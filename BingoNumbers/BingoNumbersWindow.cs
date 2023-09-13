using BingoNumbers.ViewModels;
using BingoNumbers.Views;

namespace BingoNumbers
{
	public class BingoNumbersWindow : Window
	{
		private MainPage _mainPage;
		public BingoNumbersWindow() : base() 
		{		
		}

		public BingoNumbersWindow(MainPage page) : base(page) 
		{
			_mainPage = page;
		}

		protected override void OnCreated()
		{
			((MainViewModel)_mainPage.BindingContext).RestoreState();
		}
		protected override void OnDeactivated()
		{
			((MainViewModel)_mainPage.BindingContext).SaveState();
		}
		protected override void OnDestroying()
		{
			((MainViewModel)_mainPage.BindingContext).SaveState();
		}

	}

}
