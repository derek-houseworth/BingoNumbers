using System.Windows.Input;

namespace BingoNumbers.ViewModels
{
    public class BingoNumbersViewModel : ViewModelBase
    {
        private const int UPPER_BOUND_DEFAULT_VALUE = 25;
        private const int LOWER_BOUND_DEFAULT_VALUE = 1;

        const string APP_SETTINGS_UBOUND_KEY = "UpperBound";
        const string APP_SETTINGS_LBOUND_KEY = "LowerBound";

        private List<int> _numberList = new List<int>();
        private readonly Random _random = new();

        //command interface declarations
        public ICommand UpperBoundIncrCommand { private set; get; }
        public ICommand UpperBoundDecrCommand { private set; get; }
        public ICommand LowerBoundIncrCommand { private set; get; }
        public ICommand LowerBoundDecrCommand { private set; get; }        
        public ICommand DrawNumberCommand { private set; get; }
        public ICommand ResetNumberListCommand { private set; get; }

        private int _lowerBound;

        /// <summary>
        /// Integer representing lower end of number range from which values will be inclusively drawn.
        /// </summary>
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
        }

    
        private int _upperBound;

        /// <summary>
        /// Integer representing upper end of number range from which values will be inclusively drawn.
        /// </summary>
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
        }

        private double _progress;

        /// <summary>
        /// Double in range if 0.0 - 1.0 representing draw progress with 0.0 as no numbers drawn 
        /// and 1.0 representing all numbers drawn.
        /// </summary>
        public double Progress
        {
            get => _progress;
            private set => SetProperty(ref _progress, Math.Min(1.0, Math.Max(0, value)));
        }


        private string _drawnNumber;

        /// <summary>
        /// String representing most recently drawn number.
        /// </summary>
        public string DrawnNumber
        {
            get => _drawnNumber;
            private  set => SetProperty(ref _drawnNumber, value);
        }

        private string _drawnNumberHistory;
        /// <summary>
        /// Newline delimited string containing all previously drawn numbers, one per line.
        /// </summary>
        public string DrawnNumberHistory
        {
            get => _drawnNumberHistory; 
            private set => SetProperty(ref _drawnNumberHistory, value);
        }


        /// <summary>
        /// Boolean indicates state of application where true means no numbers have been drawn 
        /// and false means one or more numbers have been drawn.
        /// </summary>
        private bool _numberListFull;
        public bool NumberListFull
        {
            get => _numberListFull; 
            private set => SetProperty(ref _numberListFull, value);
        }
       

        /// <summary>
        /// Initializes number list, progress and application state variables to prepare for
        /// new draw
        /// </summary>
        public void ResetNumberList()
        {
            _numberList.Clear();
            for (int i = _lowerBound; i <= _upperBound; i++)
            {
                _numberList.Add(i);
            }
            Progress = 0.0;
            DrawnNumber = "";
            DrawnNumberHistory = "";
            NumberListFull = true;
            RefreshCanExecutes();

        } //ResetNumberList


        /// <summary>
        /// Updates drawn number history with most recent value, randomly choses new value from number list, 
        /// updates draw progress and performs refresh state of command interfaces states
        /// to reflect state of remaining number pool
        /// </summary>
        public void DrawNumber()
        {
           if (_numberList.Count > 0)
           {
                //add previously dranw number to history
                DrawnNumberHistory = DrawnNumber + (DrawnNumberHistory.Length > 0 ? "\n" : "") + DrawnNumberHistory;

                //randomly select number from list
                int newlyDrawnNumber = _numberList[_random.Next(_numberList.Count)];

                //remove drawn number drom list
                _numberList.Remove(newlyDrawnNumber);

                //add drawn number to history of drawn numbers
                DrawnNumber = newlyDrawnNumber.ToString();
            
                //update draw progress
                Progress = (double)(UpperBound - LowerBound - _numberList.Count + 1) / (double)(UpperBound - LowerBound + 1);
                NumberListFull = false;

                RefreshCanExecutes();                
            }

        } //DrawNumber


        /// <summary>
        /// Update command interfaces, called after numbers or drawn or number list is reset
        /// </summary>
        void RefreshCanExecutes()
        {
            ((Command)DrawNumberCommand)?.ChangeCanExecute();

            ((Command)ResetNumberListCommand)?.ChangeCanExecute();

            ((Command)UpperBoundIncrCommand)?.ChangeCanExecute();

            ((Command)UpperBoundDecrCommand)?.ChangeCanExecute();

            ((Command)LowerBoundIncrCommand)?.ChangeCanExecute();
            
            ((Command)LowerBoundDecrCommand)?.ChangeCanExecute();


        } //RefreshCanExecutes


        /// <summary>
        /// Stores current lower and upper bound values in host application's Preferences store 
        /// as key/value pairs.
        /// </summary>
        public void SaveState()
        {

            Preferences.Set(APP_SETTINGS_LBOUND_KEY, LowerBound);
            Preferences.Set(APP_SETTINGS_UBOUND_KEY, UpperBound);


        } //SaveState

        /// <summary>
        /// Retrieves previously saved lower and upper bound values from host application's Preferences store 
        /// and sets either retrieved or default values if not found to appropriate view model properties.
        /// </summary>
        public void RestoreState()
        {
            LowerBound = Preferences.Get(APP_SETTINGS_LBOUND_KEY, LOWER_BOUND_DEFAULT_VALUE);
            UpperBound = Preferences.Get(APP_SETTINGS_UBOUND_KEY, UPPER_BOUND_DEFAULT_VALUE);

        }//RestoreState


        /// <summary>
        /// Initializes new instance of BingoNumbersViewModel object, object properties and 
        /// command interfaces.
        /// </summary>
        public BingoNumbersViewModel()
       {
            RestoreState();

            DrawNumberCommand = new Command(DrawNumber, canExecute: () => { return _numberList.Count > 0; });
            ResetNumberListCommand = new Command(ResetNumberList, canExecute: () => { return !NumberListFull; });
            UpperBoundIncrCommand = new Command(
                execute: () => 
                { 
                    UpperBound++;
                    SaveState();
                }, 
                canExecute: () => 
                { 
                    return NumberListFull; 
                });
            UpperBoundDecrCommand = new Command(
                execute: () => 
                { 
                    UpperBound--;
                    SaveState();
                }, canExecute: () => 
                { 
                    return NumberListFull; 
                });
            LowerBoundIncrCommand = new Command(
                execute: () => 
                { 
                    LowerBound++;
                    SaveState();
                }, 
                canExecute: () => 
                { 
                    return NumberListFull; 
                });
            LowerBoundDecrCommand = new Command(execute: () => 
            { 
                LowerBound--;
                SaveState();
            }, 
            canExecute: () => 
            { 
                return NumberListFull; 
            });

        } //BingoNumbersViewModel

    } //BingoNumbersViewModel

} //BingoNumbers.ViewModels