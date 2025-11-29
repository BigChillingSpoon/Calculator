using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Calculator.Core.Interfaces;
using Calculator.Services.Interfaces;
using Calculator.AppLayer.Services.Interfaces;
using Calculator.Core.Models.Enums;
using Calculator.AppLayer.Models.Enums;
using Calculator.Extensions.Commands;

namespace Calculator.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Vars
        private readonly IEvaluationProcessingService _evaluationProcessingService;
        private readonly INotifyService _notifyService;
        private readonly IDialogService _dialogService;
        private readonly IFileProcessingService _fileProcessingService;
        #endregion Vars

        #region Constructors
        public MainViewModel(IEvaluationProcessingService evaluationProcessingService, INotifyService notifyService, IDialogService dialogService, IFileProcessingService fileProcessingService)
        {
            // Initialize commands
            EvaluateCommand = new RelayCommand(OnEvaluateExpression, _ => !IsBusy && !string.IsNullOrWhiteSpace(Input));
            ClearCommand = new RelayCommand(OnClear, _ => !IsBusy);
            DigitCommand = new RelayCommand(OnDigit, _ => !IsBusy);
            OperationCommand = new RelayCommand(OnOperation, _ => !IsBusy);
            BackSpaceCommand = new RelayCommand(OnBackSpace, _ => !IsBusy && !string.IsNullOrEmpty(Input));
            SelectInputFileCommand = new AsyncRelayCommand(OnSelectInputFileAsync, () => !IsBusy);
            SelectOutputDirectoryCommand = new AsyncRelayCommand(OnSelectOutputDirectory, () => !IsBusy);
            EvaluateFromFileCommand = new AsyncRelayCommand(OnEvaluateFromFileAsync, CanEvaluateFromFile);

            // Initialize properties
            _input = string.Empty;
            _evaluationProcessingService = evaluationProcessingService;
            _notifyService = notifyService;
            _dialogService = dialogService;
            _fileProcessingService = fileProcessingService;
        }
        #endregion Constructors

        #region Properties
        private string _input;
        public string Input
        {
            get => _input;
            set
            {
                if (SetProperty(ref _input, value))
                {
                    // Update command states when input changes
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private string _inputFilePath;
        public string InputFilePath
        {
            get => _inputFilePath;
            set
            {
                if (SetProperty(ref _inputFilePath, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private string _outputDirectory;
        public string OutputDirectory
        {
            get => _outputDirectory;
            set
            {
                if (SetProperty(ref _outputDirectory, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private string _outputFileName;
        public string OutputFileName
        {
            get => _outputFileName;
            set => SetProperty(ref _outputFileName, value);
        }

        #endregion Properties

        #region Commands
        public RelayCommand EvaluateCommand { get; }
        public RelayCommand ClearCommand { get; }
        public RelayCommand DigitCommand { get; }
        public RelayCommand OperationCommand { get; }
        public RelayCommand BackSpaceCommand { get; }
        public AsyncRelayCommand SelectInputFileCommand { get; }
        public AsyncRelayCommand SelectOutputDirectoryCommand { get; }
        public AsyncRelayCommand EvaluateFromFileCommand { get; }
        public RelayCommand ToggleMenuCommand { get; }
        #endregion Commands

        #region Command Handlers
        private void OnEvaluateExpression(object parameter)
        {
            if (string.IsNullOrWhiteSpace(_input))
                return;

            IsBusy = true;

            try
            {
                var result = _evaluationProcessingService.ProcessEvaluation(_input);

                if (result.Success)
                
                    Input = result.Value.ToString();
                else
                    HandleEvaluationError(result);
            }
            catch (Exception ex)
            {
                Input = "Error";
                _notifyService.ShowError($"Unexpected error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnClear(object parameter)
        {
            Input = string.Empty;
        }

        private void OnDigit(object parameter)
        {
            if (parameter is string digitString)
            {
                Input += digitString;
            }
        }

        private void OnOperation(object parameter)
        {
            if (parameter is string operationString)
            {
                Input += operationString;
            }
        }

        private void OnBackSpace(object parameter)
        {
            if (!string.IsNullOrEmpty(Input))
            {
                Input = Input[..^1];
            }
        }

        // File operations
        private async Task OnSelectInputFileAsync()
        {
            IsBusy = true;

            try
            {
                if (_dialogService.ShowOpenFileDialog(out string inputFilePath))
                {
                    await Task.Yield();
                    InputFilePath = inputFilePath;
                }
                else
                {
                    _notifyService.ShowWarning("No file selected.");
                }
            }
            catch (Exception ex)
            {
                _notifyService.ShowError($"Error selecting file: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OnSelectOutputDirectory()
        {
            IsBusy = true;

            try
            {
                if (_dialogService.ShowOpenDirectoryDialog(out string outputDirectory))
                {
                    await Task.Yield();
                    OutputDirectory = outputDirectory;
                }
                else
                {
                    _notifyService.ShowWarning("No directory selected.");
                }
            }
            catch (Exception ex)
            {
                _notifyService.ShowError($"Error selecting directory: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OnEvaluateFromFileAsync()
        {
            if (!CanEvaluateFromFile())
            {
                _notifyService.ShowWarning("Please select both input file and output directory.");
                return;
            }

            IsBusy = true;

            try
            {
                var processResult = await _fileProcessingService.ProcessEvaluationFromFileAsync(InputFilePath, OutputDirectory, OutputFileName);

                if (processResult.Success)
                    _notifyService.ShowInfo("File processed successfully. Check output directory for results.");
                else
                    HandleProcessingError(processResult);
            }
            catch (Exception ex)
            {
                _notifyService.ShowError($"Error processing file: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanEvaluateFromFile()
        {
            return !IsBusy
                && !string.IsNullOrWhiteSpace(InputFilePath)
                && !string.IsNullOrWhiteSpace(OutputDirectory);
        }
        #endregion Command Handlers

        #region Helper Methods
        private void HandleEvaluationError(dynamic result)
        {
            if (result.ErrorType == ErrorType.Error)
            {
                Input = "Error";
                _notifyService.ShowError(result.ErrorMessage);
            }
            else if (result.ErrorType == ErrorType.Warning)
            {
                _notifyService.ShowWarning(result.ErrorMessage);
            }
        }

        private void HandleProcessingError(dynamic processResult)
        {
            if (processResult.ErrorType == ErrorType.Error)
            {
                _notifyService.ShowError(processResult.ErrorMessage);
            }
            else if (processResult.ErrorType == ErrorType.Warning)
            {
                _notifyService.ShowWarning(processResult.ErrorMessage);
            }
        }
        #endregion Helper Methods
    }
}