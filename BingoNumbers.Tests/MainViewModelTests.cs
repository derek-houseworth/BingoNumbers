using BingoNumbers.ViewModels;

using System.Reflection;

namespace BingoNumbers.Tests;


[TestFixture]
internal class MainViewModelTests
{

    private const string PREFERENCES_UBOUND_KEY = "UpperBound";
    private const string PREFERENCES_LBOUND_KEY = "LowerBound";
    private const string PREFERENCES_DRAWN_NUMBER_KEY = "DrawnNumber";
    private const string PREFERENCES_DRAWN_NUMBER_HISTORY_KEY = "DrawnNumberHistory";

    [SetUp]
	public void Setup()
	{
	}


	[Test]
	public void TestInitialState()
	{
		TestHelper.DebugWriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod()}:");

		var viewModel = new MainViewModel(new MockPreferencesService());
        using (Assert.EnterMultipleScope())
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
			Assert.That(viewModel.Progress, Is.Zero);
		}

	} //TestViewModelInitialState


	[Test]
	public void TestChangeLowerBound()
	{
		TestHelper.DebugWriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod()}:");

        var viewModel = new MainViewModel(new MockPreferencesService());
        using (Assert.EnterMultipleScope())
        {
			Assert.That(viewModel, Is.Not.Null);
			int initialLowerBound = viewModel.LowerBound;
			viewModel.ChangeLowerBound.Execute("-");
			Assert.That(viewModel.LowerBound, Is.LessThan(initialLowerBound));
			viewModel.ChangeLowerBound.Execute("+");
			Assert.That(viewModel.LowerBound, Is.EqualTo(initialLowerBound));
		}

	} //TestChangeLowerBound


	[Test]
	public void TestChangeUpperBound()
	{
		TestHelper.DebugWriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod()}:");

        var viewModel = new MainViewModel(new MockPreferencesService());
        using (Assert.EnterMultipleScope())
        {
			Assert.That(viewModel, Is.Not.Null);
			int initialUpperBound = viewModel.UpperBound;
			viewModel.ChangeUpperBound.Execute("+");
			Assert.That(viewModel.UpperBound, Is.GreaterThan(initialUpperBound));
			viewModel.ChangeUpperBound.Execute("-");
			Assert.That(viewModel.UpperBound, Is.EqualTo(initialUpperBound));
		}

	} //TestChangeUpperBound


	[Test]
	public void TestUpperBoundAutoIncrement()
	{
		TestHelper.DebugWriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod()}:");

        var viewModel = new MainViewModel(new MockPreferencesService());
		using (Assert.EnterMultipleScope())
		{
			Assert.That(viewModel, Is.Not.Null);

			viewModel.LowerBound = viewModel.UpperBound;
			Assert.That(viewModel.UpperBound, Is.EqualTo(viewModel.LowerBound + 1));
		}

	} //TestUpperBoundAutoIncrement


	[Test]
	public void TestLowerBoundAutoDecrement()
	{
		TestHelper.DebugWriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod()}:");

        var viewModel = new MainViewModel(new MockPreferencesService());
        Assert.That(viewModel, Is.Not.Null);

		viewModel.UpperBound = viewModel.LowerBound;
		Assert.That(viewModel.LowerBound, Is.EqualTo(viewModel.UpperBound - 1));

	} //TestLowerBoundAutoDecrement


	[Test]
	public void TestDrawSingleNumber()
	{
		TestHelper.DebugWriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod()}:");

        var viewModel = new MainViewModel(new MockPreferencesService());
        using (Assert.EnterMultipleScope())
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

		}

	} //TestDrawSingleNumber()


	[Test]
	public void TestDrawTwoNumbers()
	{
		TestHelper.DebugWriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod()}:");

        var viewModel = new MainViewModel(new MockPreferencesService());

        using (Assert.EnterMultipleScope())
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

		}

	} //TestDrawTwoNumbers


	[Test]
	public void TestDrawAllNumbers()
	{
		TestHelper.DebugWriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod()}:");

        var viewModel = new MainViewModel(new MockPreferencesService());

        using (Assert.EnterMultipleScope())
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
		}

	} //TestDrawAllNumbers

	[Test]
	public void TestRestoreState()
	{
        TestHelper.DebugWriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod()}:");
        var mockPrefsService = new MockPreferencesService();

		int lowerBound = 1;
        int upperBound = 10;
        string drawnNumber = "5";
        string drawnNumberHistory = "9\r\n3";
        mockPrefsService.Set(PREFERENCES_UBOUND_KEY, upperBound);
        mockPrefsService.Set(PREFERENCES_LBOUND_KEY, lowerBound);
        mockPrefsService.Set(PREFERENCES_DRAWN_NUMBER_KEY, drawnNumber);
        mockPrefsService.Set(PREFERENCES_DRAWN_NUMBER_HISTORY_KEY, drawnNumberHistory);

        var viewModel = new MainViewModel(mockPrefsService);
		using (Assert.EnterMultipleScope())
		{
			Assert.That(viewModel, Is.Not.Null);
			viewModel.RestoreState();
            Assert.That(viewModel.LowerBound, Is.EqualTo(lowerBound));
			Assert.That(viewModel.UpperBound, Is.EqualTo(upperBound));
			Assert.That(viewModel.DrawnNumber, Is.EqualTo(drawnNumber));
            Assert.That(viewModel.DrawnNumberHistory, Is.EqualTo(drawnNumberHistory));
        }

    } //TestRestoreState


    [Test]
	public void TestSaveState()
	{
        TestHelper.DebugWriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod()}:");

		var mockPrefsService = new MockPreferencesService();

        Random random = new(Seed: DateTime.Now.Microsecond);

		int lowerBound = random.Next(-100, 100);
		int upperBound = random.Next(lowerBound+1, 101);
        var viewModel = new MainViewModel(mockPrefsService)
		{
			LowerBound = lowerBound,
			UpperBound = upperBound
        };

		using (Assert.EnterMultipleScope())
		{
			Assert.That(viewModel, Is.Not.Null);

            //draw 2 numbers & save state
            viewModel.Draw.Execute(null);
            viewModel.Draw.Execute(null);
            viewModel.SaveState();

            //verify expected values were saved to preferences

			//lower bound
            Assert.That(mockPrefsService.ContainsKey(PREFERENCES_LBOUND_KEY), Is.True);
            Assert.That(mockPrefsService.Get(PREFERENCES_LBOUND_KEY, 0), Is.EqualTo(lowerBound));

			//upper bound
            Assert.That(mockPrefsService.ContainsKey(PREFERENCES_UBOUND_KEY), Is.True);
            Assert.That(mockPrefsService.Get(PREFERENCES_UBOUND_KEY, 0), Is.EqualTo(upperBound));

			//drawn number
            Assert.That(mockPrefsService.ContainsKey(PREFERENCES_DRAWN_NUMBER_KEY), Is.True);
            Assert.That(mockPrefsService.Get(PREFERENCES_DRAWN_NUMBER_KEY, string.Empty), Is.EqualTo(viewModel.DrawnNumber));

            //drawn number history
            Assert.That(mockPrefsService.ContainsKey(PREFERENCES_DRAWN_NUMBER_HISTORY_KEY), Is.True);
            Assert.That(mockPrefsService.Get(PREFERENCES_DRAWN_NUMBER_HISTORY_KEY, string.Empty), Is.EqualTo(viewModel.DrawnNumberHistory));
        }

    } //TestSaveState


} //MainViewModelTests