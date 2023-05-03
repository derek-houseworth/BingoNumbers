using BingoNumbers.ViewModels;
using BingoNumbers.Views;

namespace BingoNumbers
{
	public class BNWindow : Window
	{
		private MainPage _mainPage;
		public BNWindow() : base() 
		{		
		}

		public BNWindow(MainPage page) : base(page) 
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
