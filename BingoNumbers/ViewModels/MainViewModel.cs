using System.Diagnostics;
using System.Windows.Input;

namespace BingoNumbers.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private const int UPPER_BOUND_DEFAULT_VALUE = 25;
    private const int LOWER_BOUND_DEFAULT_VALUE = 1;
	private const string DRAWN_NUMBER_DEFAULT_VALUE = "";
	private const string DRAWN_NUMBER_HISTORY_DEFAULT_VALUE = "";

	private const string APP_SETTINGS_UBOUND_KEY = "UpperBound";
	private const string APP_SETTINGS_LBOUND_KEY = "LowerBound";
	private const string APP_SETTINGS_DRAWN_NUMBER_KEY = "DrawnNumber";
	private const string APP_SETTINGS_DRAWN_NUMBER_HISTORY_KEY = "DrawnNumberHistory";

	private readonly List<int> _numberList = [];
    private readonly Random _random = new();

    //command interface declarations
    public ICommand ChangeUpperBound{ private set; get; }
    public ICommand ChangeLowerBound { private set; get; }
    public ICommand Draw{ private set; get; } 
    public ICommand Reset { private set; get; }


	/// <summary>
	/// Integer value representing lower end of number range from which values will be inclusively drawn
	/// </summary>
	private int _lowerBound = LOWER_BOUND_DEFAULT_VALUE;
	public int LowerBound
    {
        get => _lowerBound;
        set 
        {
            if (_lowerBound != value)
            {
                SetProperty(ref _lowerBound, value);
                if (UpperBound <= LowerBound) UpperBound = LowerBound + 1;
                ResetNumberList();
            }
        }

	} //LowerBound


	/// <summary>
	/// Integer value representing upper end of number range from which values will be inclusively drawn
	/// </summary>
	private int _upperBound = UPPER_BOUND_DEFAULT_VALUE;
	public int UpperBound
    {
        get => _upperBound;
        set
        {
            if (_upperBound != value)
            {
                SetProperty(ref _upperBound, value);
                if (LowerBound >= _upperBound) LowerBound = UpperBound-1;
                ResetNumberList();
            }
        }
	} //UpperBound


	/// <summary>
	/// Double in range if 0.0 - 1.0 representing draw progress with 0.0 inidicating no numbers drawn 
	/// and 1.0 indicating all numbers drawn.
	/// </summary>
	private double _progress = 0;
	public double Progress
    {
        get => _progress;
        private set => SetProperty(ref _progress, Math.Min(1.0, Math.Max(0, value)));
    }


	/// <summary>
	/// String representing most recently drawn number.
	/// </summary>
	private string _drawnNumber = String.Empty;
    public string DrawnNumber
    {
        get => _drawnNumber;
        private  set => SetProperty(ref _drawnNumber, value);
    }


    /// <summary>
    /// Newline delimited string containing all previously drawn numbers, one per line.
    /// </summary>
    private string _drawnNumberHistory = string.Empty;
	public string DrawnNumberHistory
    {
        get => _drawnNumberHistory; 
        private set => SetProperty(ref _drawnNumberHistory, value);
    }


	private bool _canReset = false;
    public bool CanReset
    {
        get => _canReset;
        private set => SetProperty(ref _canReset, value);
    }

    private bool _canDrawNumber = true;
    public bool CanDrawNumber 
    {
        get => _canDrawNumber;
        private set => SetProperty(ref _canDrawNumber, value);
    }

	private bool _canChangeBounds = true;
	public bool CanChangeBounds
	{
		get => _canChangeBounds;
		private set => SetProperty(ref _canChangeBounds, value);
	}


	/// <summary>
	/// Initializes number list, progress and application state variables to prepare for
	/// new draw
	/// </summary>
	private void ResetNumberList()
    {
        _numberList.Clear();
        for (int i = _lowerBound; i <= _upperBound; i++)
        {
            _numberList.Add(i);
        }
        Progress = 0.0;
        DrawnNumber = "";
        DrawnNumberHistory = "";
        CanDrawNumber = true;
        CanReset = false;
        CanChangeBounds = true;
        RefreshCanExecutes();


	} //ResetNumberList


	/// <summary>
	/// Updates drawn number history with most recent value, randomly choses new value from number list, 
	/// updates draw progress and performs refresh state of command interfaces states
	/// to reflect state of remaining number pool
	/// </summary>
	private void DrawNumber()
    {
       if (CanDrawNumber)
       {
            //add previously drawn number to history
            DrawnNumberHistory = DrawnNumber + (String.IsNullOrEmpty(DrawnNumberHistory) ? "" : Environment.NewLine) + DrawnNumberHistory;

			//randomly select number from list
			int newlyDrawnNumber = _numberList[_random.Next(_numberList.Count)];

			//remove drawn number from list
			_numberList.Remove(newlyDrawnNumber);

            //add drawn number to history of drawn numbers
            DrawnNumber = newlyDrawnNumber.ToString();

            CanDrawNumber = _numberList.Count > 0;
            CanReset = true;
            CanChangeBounds = false;
			RefreshCanExecutes();                
        }

    } //DrawNumber


    /// <summary>
    /// Update command interfaces, called after numbers or drawn or number list is reset
    /// </summary>
    void RefreshCanExecutes()
    {
		//update draw progress
		Progress = (double)(UpperBound - LowerBound - _numberList.Count + 1) / (double)(UpperBound - LowerBound + 1);

		((Command)Draw)?.ChangeCanExecute();

        ((Command)Reset)?.ChangeCanExecute();

        ((Command)ChangeLowerBound)?.ChangeCanExecute();

        ((Command)ChangeUpperBound)?.ChangeCanExecute();

    } //RefreshCanExecutes


    /// <summary>
    /// Stores current lower and upper bound values in host application's Preferences store 
    /// as key/value pairs.
    /// </summary>
    public void SaveState()
    {

        Preferences.Set(APP_SETTINGS_LBOUND_KEY, LowerBound);
        Preferences.Set(APP_SETTINGS_UBOUND_KEY, UpperBound);
		Preferences.Set(APP_SETTINGS_DRAWN_NUMBER_KEY, DrawnNumber);
		Preferences.Set(APP_SETTINGS_DRAWN_NUMBER_HISTORY_KEY, DrawnNumberHistory);

		Debug.WriteLine("*** view model state saved ***");

	} //SaveState


	/// <summary>
	/// Initializes view model properties for lower bound, upper bound, drawn number history and most recently 
    /// drawn number with values from from host application's Preferences store and updates command state
    /// /// </summary>
	public void RestoreState()
    {
        LowerBound = Preferences.Get(APP_SETTINGS_LBOUND_KEY, LOWER_BOUND_DEFAULT_VALUE);
        UpperBound = Preferences.Get(APP_SETTINGS_UBOUND_KEY, UPPER_BOUND_DEFAULT_VALUE);
		ResetNumberList();
		DrawnNumber = Preferences.Get(APP_SETTINGS_DRAWN_NUMBER_KEY, DRAWN_NUMBER_DEFAULT_VALUE);
		DrawnNumberHistory = Preferences.Get(APP_SETTINGS_DRAWN_NUMBER_HISTORY_KEY, DRAWN_NUMBER_HISTORY_DEFAULT_VALUE);
        if (DrawnNumberHistory != DRAWN_NUMBER_HISTORY_DEFAULT_VALUE)
        {
            //parse saved number history string to enable removing corresponding values from number list
			foreach (var drawnNumberHistoryItem in DrawnNumberHistory.Split('\n'))
			{
				_numberList.Remove(Convert.ToInt16(drawnNumberHistoryItem));
			}
		}
		if (DrawnNumber != DRAWN_NUMBER_DEFAULT_VALUE)
        {

			//parse most recently drawn number string to enable removing corresponding value from number list
			_numberList.Remove(Convert.ToInt16(DrawnNumber));
            CanReset = true;
            CanChangeBounds = false;
		}

		RefreshCanExecutes();

		Debug.WriteLine("*** view model state restored ***");

	}//RestoreState


    /// <summary>
    /// Initializes new instance of view model object, object properties and 
    /// command interfaces
    /// </summary>
   public MainViewModel()
   {
		ResetNumberList();

		Draw = new Command(execute: () => DrawNumber(), canExecute: () => { return CanDrawNumber; });
        Reset = new Command( execute: () => ResetNumberList(), canExecute: () => { return CanReset; });

        ChangeLowerBound = new Command<string>(
            execute: (string sign) => 
            {
                LowerBound += sign == "+" ? 1 : -1;
            }, 
            canExecute: (string sign) => 
            {
                return CanChangeBounds;
            });
		ChangeUpperBound = new Command<string>(
			execute: (string sign) =>
            {
                UpperBound += sign == "+" ? 1 : -1;
            },
			canExecute: (string sign) =>
			{
                return CanChangeBounds;
            });


	} //MainViewModel

} //MainViewModel