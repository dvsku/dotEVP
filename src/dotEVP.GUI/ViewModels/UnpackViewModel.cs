using System;
using System.Collections.Generic;
using System.Media;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using dotEVP.Core;
using dotEVP.GUI.Helpers;
using dvsku.Wpf.Utilities.Commands;
using dvsku.Wpf.Utilities.ViewModels;

namespace dotEVP.GUI.ViewModels {
    public class UnpackViewModel : PageViewModel {
        private string _input;
        private string _output;
        private int _progressBarValue = 0;
        private string _progressBarText = "0%";
        private DispatcherTimer _progressUpdateTimer;
        private Queue<double> _progressQueue;
        private Unpacker _unpacker;
        private bool _canExecute = true;

        public string Input {
            get => _input;
            set {
                _input = value;
                RaisePropertyChanged(nameof(Input));
            }
        }
        public string Output {
            get => _output;
            set {
                _output = value;
                RaisePropertyChanged(nameof(Output));
            }
        }
        public int ProgressBarValue {
            get => _progressBarValue;
            set {
                _progressBarValue = value;
                RaisePropertyChanged(nameof(ProgressBarValue));
            }
        }
        public string ProgressBarText {
            get => _progressBarText;
            set {
                _progressBarText = value;
                RaisePropertyChanged(nameof(ProgressBarText));
            }
        }
        public bool CanExecute {
            get => _canExecute;
            set {
                _canExecute = value;
                RaisePropertyChanged(nameof(CanExecute));
            }
        }

        public ICommand SelectInput { get; private set; }
        public ICommand SelectOutput { get; private set; }
        public ICommand Unpack { get; private set; }

        public UnpackViewModel() : base() {
            _progressQueue = new Queue<double>();
            _progressUpdateTimer = new DispatcherTimer(DispatcherPriority.Render, Application.Current.Dispatcher);
            _progressUpdateTimer.Tick += new EventHandler(OnProgressTimerTick);
            _progressUpdateTimer.Interval = new TimeSpan(1);

            _unpacker = new Unpacker();
            _unpacker.UnpackingStarted += OnUnpackingStarted;
            _unpacker.UnpackingStopped += OnUnpackingStopped;
            _unpacker.UnpackingFinished += OnUnpackingFinished;
            _unpacker.ProgressUpdate += OnProgressUpdate;
            _unpacker.ErrorOccurred += OnErrorOccured;
        }

        protected override void InitCommands() {
            base.InitCommands();
            SelectInput = new BasicCommand(ExecuteSelectInput);
            SelectOutput = new BasicCommand(ExecuteSelectOutput);
            Unpack = new BasicCommand(ExecuteUnpack);
        }

        private void ExecuteSelectInput() {
            Input = SelectHelpers.SelectFile();
        }

        private void ExecuteSelectOutput() {
            Output = SelectHelpers.SelectFolder();
        }

        private void ExecuteUnpack() {
            Thread thread = new Thread(() => { _unpacker.Unpack(Input, Output); });
            thread.Start();
        }

        private void OnProgressTimerTick(object sender, EventArgs e) {
            if (_progressQueue.Count == 0) return;

            double progress = _progressQueue.Dequeue();
            ProgressBarValue = (int)(progress);
            ProgressBarText = ProgressBarValue + "%";
            if (progress == 100)
                _progressUpdateTimer.Stop();
        }

        private void OnErrorOccured(object sender, Core.Events.ErrorOccurredArgs e) {
            MessageBox.Show(e.Text, e.Operation, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void OnProgressUpdate(object sender, double e) {
            _progressQueue.Enqueue(e);
        }

        private void OnUnpackingFinished(object sender, EventArgs e) {
            CanExecute = true;
            _progressQueue.Enqueue(100);
            SystemSounds.Exclamation.Play();
        }

        private void OnUnpackingStopped(object sender, EventArgs e) {
            _progressQueue.Enqueue(0);
        }

        private void OnUnpackingStarted(object sender, EventArgs e) {
            CanExecute = false;
            _progressUpdateTimer.Start();
            _progressQueue.Enqueue(0);
        }
    }
}