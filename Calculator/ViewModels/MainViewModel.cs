using Calculator.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Calculator.Core.Interfaces;
using Calculator.Services.Interfaces;

namespace Calculator.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Vars
        private readonly IExpressionEvaluator _expressionEvaluator;
        private readonly INotifyService _notifyService;
        private readonly IDialogService _dialogService;
        #endregion Vars
        #region Constructors
        public MainViewModel(IExpressionEvaluator expressionEvaluator, INotifyService notifyService, IDialogService dialogService)
        {
            EvaluateCommand = new RelayCommand(OnEvaluateExpression);
            ClearCommand = new RelayCommand(OnClear, _ => !IsBusy);
            DigitCommand = new RelayCommand(OnDigit, _ => !IsBusy);
            OperationCommand = new RelayCommand(OnOperation, _ => !IsBusy);
            BackSpaceCommand = new RelayCommand(OnBackSpace, _ => !IsBusy);
            SelectInputFileCommand = new RelayCommand(OnSelectInputFile, _ => !IsBusy);
            SelectOutputDirectoryCommand = new RelayCommand(OnSelectOutputDirectory, _ => !IsBusy);
            EvaluateFromFileCommand = new AsyncRelayCommand(OnEvaluateFromFileAsync, () => !IsBusy);
            _input = String.Empty;
            _inputFileName = "No input file specified.";
            _outputDirectory = "No output directory specified.";
            _expressionEvaluator =  expressionEvaluator;//todo osetrit null
            _notifyService = notifyService;
            _dialogService = dialogService;
        }
        #endregion Constructors
        #region Properties

        private string _input;
        public string Input
        {
            get => _input;
            set => SetProperty(ref _input, value);
        }

        private string _inputFileName;
        public string InputFilePath
        {
            get => _inputFileName;
            set => SetProperty(ref _inputFileName, value);
        }
        
        private string _outputDirectory;
        public string OutputDirectory
        {
            get => _outputDirectory;
            set => SetProperty(ref _outputDirectory, value);
        }
        private string _currentExpression;
        private string CurrentExpression
        {
            get => _currentExpression;
            set => SetProperty(ref _currentExpression, value);
        }
        #endregion Properties
        #region Commands
        public RelayCommand EvaluateCommand { get; }
        public RelayCommand ClearCommand { get; }
        public RelayCommand DigitCommand { get; }
        public RelayCommand OperationCommand { get; }
        public RelayCommand BackSpaceCommand { get; }
        public RelayCommand SelectInputFileCommand { get; }
        public RelayCommand SelectOutputDirectoryCommand { get; }
        public AsyncRelayCommand EvaluateFromFileCommand { get; }
        #endregion Commands
        #region CommandHandlers
        private void OnEvaluateExpression(object parameter)
        {
            // Implementation for calculation
            
            var result = _expressionEvaluator.Evaluate(_input);
            if(result.Success)
            {
                Input = result.Value.ToString();
            }
            else
            {
                _notifyService.ShowError(result.ErrorMessage);
                Input = "Error";
            }
        }
        private void OnClear(object parameter)
        {
            Input = String.Empty;    
        }
        private void OnDigit(object parameter)
        {
            if (parameter is null || parameter is not string digitString)
               return;//handle this error for the programmer 
            Input += digitString;
        }
        private void OnOperation(object parameter)
        {
            if (parameter is null || parameter is not string operationString)
                return;//handle this error for the programmer 
            Input += operationString;
        }
        private void OnBackSpace(object parameter)
        {
            Input = Input.Length > 0 ? Input[..^1] : String.Empty;
        }
        //files
        private void OnSelectInputFile(object parameter)
        {
            
        }
        private void OnSelectOutputDirectory(object parameter)
        {
            // Implementation for selecting output directory
        }
        private async Task OnEvaluateFromFileAsync()
        {
            // Implementation for calculating from file
        }
        #endregion CommandHandlers
        #region Methods
        
        private void EvaluateExpression()
        {
            // Implementation for evaluating the current expression
        }
        #endregion Methods
    }
}
