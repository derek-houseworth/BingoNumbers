using BingoNumbers.ViewModels;
using System.Reflection;

namespace BingoNumbers.Tests
{
	public class MainViewModelTests
	{
		[SetUp]
		public void Setup()
		{
		}


		[Test]
		public void TestInitialState()
		{
			TestHelper.DebugWriteLine($"{this.GetType().Name}.{MethodBase.GetCurrentMethod()}:");

			var viewModel = new MainViewModel();
			Assert.Multiple(() =>
			{
				Assert.That(viewModel, Is.Not.Null);

				//reset
				Assert.That(viewModel.CanReset, Is.False);
				Assert.That(viewModel.Reset.CanExecute(null), Is.False);

				//draw
				Assert.That(viewModel.CanDrawNumber, Is.True);
				Assert.That(viewModel.Draw.CanExecute(null), Is.True);

				//bounds
				Assert.That(viewModel.CanChangeBounds, Is.True);
				Assert.That(viewModel.LowerBound, Is.LessThan(viewModel.UpperBound));
				Assert.That(viewModel.ChangeLowerBound.CanExecute(null), Is.True);
				Assert.That(viewModel.ChangeUpperBound.CanExecute(null), Is.True);

				//progress
				Assert.That(viewModel.Progress, Is.EqualTo(0.0d));
			});

		} //TestViewModelInitialState

		[Test]
		public void TestChangeLowerBound()
		{
			TestHelper.DebugWriteLine($"{this.GetType().Name}.{MethodBase.GetCurrentMethod()}:");

			var viewModel = new MainViewModel();
			Assert.Multiple(() =>
			{
				Assert.That(viewModel, Is.Not.Null);
				int initialLowerBound = viewModel.LowerBound;
				viewModel.ChangeLowerBound.Execute("-");
				Assert.That(viewModel.LowerBound, Is.LessThan(initialLowerBound));
				viewModel.ChangeLowerBound.Execute("+");
				Assert.That(viewModel.LowerBound, Is.EqualTo(initialLowerBound));
			});

		} //TestChangeLowerBound


		[Test]
		public void TestChangeUpperBound()
		{
			TestHelper.DebugWriteLine($"{this.GetType().Name}.{MethodBase.GetCurrentMethod()}:");

			var viewModel = new MainViewModel();
			Assert.Multiple(() =>
			{
				Assert.That(viewModel, Is.Not.Null);
				int initialUpperBound = viewModel.UpperBound;
				viewModel.ChangeUpperBound.Execute("+");
				Assert.That(viewModel.UpperBound, Is.GreaterThan(initialUpperBound));
				viewModel.ChangeUpperBound.Execute("-");
				Assert.That(viewModel.UpperBound, Is.EqualTo(initialUpperBound));
			});

		} //TestChangeUpperBound


		[Test]
		public void TestUpperBoundAutoIncrement()
		{
			TestHelper.DebugWriteLine($"{this.GetType().Name}.{MethodBase.GetCurrentMethod()}:");

			var viewModel = new MainViewModel();
			Assert.That(viewModel, Is.Not.Null);
			
			viewModel.LowerBound = viewModel.UpperBound;
			Assert.That(viewModel.UpperBound, Is.EqualTo(viewModel.LowerBound+1));

		} //TestUpperBoundAutoIncrement


		[Test]
		public void TestLowerBoundAutoDecrement()
		{
			TestHelper.DebugWriteLine($"{this.GetType().Name}.{MethodBase.GetCurrentMethod()}:");

			var viewModel = new MainViewModel();
			Assert.That(viewModel, Is.Not.Null);

			viewModel.UpperBound = viewModel.LowerBound;
			Assert.That(viewModel.LowerBound, Is.EqualTo(viewModel.UpperBound - 1));

		} //TestLowerBoundAutoDecrement


		[Test]
		public void TestDrawSingleNumber()
		{
			TestHelper.DebugWriteLine($"{this.GetType().Name}.{MethodBase.GetCurrentMethod()}:");

			var viewModel = new MainViewModel();
			Assert.Multiple(() =>
			{
				Assert.That(viewModel, Is.Not.Null);
				
				viewModel.Draw.Execute(null);

				Assert.That(int.TryParse(viewModel.DrawnNumber, out _), Is.True);
				Assert.That(string.IsNullOrEmpty(viewModel.DrawnNumberHistory), Is.True);

				//reset
				Assert.That(viewModel.CanReset, Is.True);
				Assert.That(viewModel.Reset.CanExecute(null), Is.True);


				//bounds
				Assert.That(viewModel.CanChangeBounds, Is.False);
				Assert.That(viewModel.ChangeLowerBound.CanExecute(null), Is.False);
				Assert.That(viewModel.ChangeUpperBound.CanExecute(null), Is.False);

				//progress
				Assert.That(viewModel.Progress, Is.GreaterThan(0.0d));

			});

		} //TestDrawSingleNumber()


		[Test]
		public void TestDrawTwoNumbers()
		{
			TestHelper.DebugWriteLine($"{this.GetType().Name}.{MethodBase.GetCurrentMethod()}:");

			var viewModel = new MainViewModel();
			Assert.Multiple(() =>
			{
				Assert.That(viewModel, Is.Not.Null);
				viewModel.UpperBound = viewModel.LowerBound + 1;
				viewModel.Draw.Execute(null);
				string firstNumberDrawn = viewModel.DrawnNumber;
				viewModel.Draw.Execute(null);

				//DrawnNumber property value should be 2nd number drawn, 
				//DrawnNumberHistory property value should contain first number drawn
				Assert.That(viewModel.DrawnNumberHistory.Trim().Split(Environment.NewLine).Count, Is.EqualTo(1));
				Assert.That(viewModel.DrawnNumberHistory.Trim(), Is.EqualTo(firstNumberDrawn));

				Assert.That(viewModel.Progress, Is.EqualTo(1.0d));

			});

		} //TestDrawTwoNumbers


		[Test]
		public void TestDrawAllNumbers()
		{
			TestHelper.DebugWriteLine($"{this.GetType().Name}.{MethodBase.GetCurrentMethod()}:");		

			var viewModel = new MainViewModel();

			Assert.Multiple(() =>
			{
				Assert.That(viewModel, Is.Not.Null);
				for (int i = viewModel.LowerBound; i <= viewModel.UpperBound; i++)
				{
					viewModel.Draw.Execute(null);
					TestHelper.DebugWriteLine($"DrawnNumber = {viewModel.DrawnNumber}\tCanDrawNumber = {viewModel.CanDrawNumber}\tCanReset = {viewModel.CanReset}");
				}

				Assert.That(viewModel.CanDrawNumber, Is.False);
				Assert.That(viewModel.Draw.CanExecute(null), Is.False);
				Assert.That(viewModel.Progress, Is.EqualTo(1.0d));
			});

		} //TestDrawAllNumbers


	} //MainViewModelTests

} //BingoNumbers.Tests