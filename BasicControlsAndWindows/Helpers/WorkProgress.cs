using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
namespace BasicControlsAndWindows.Helpers
{
    public class WorkProgress : DependencyObject
    {
        TextBlock ThisLabel;
        public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.Register("IsVisible", typeof(bool), typeof(WorkProgress), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnIsVisiblePropertyChanged)));
        public bool Visible
        {
            get
            {
                return (bool) GetValue(IsVisibleProperty);
            }
            set
            {
                SetValue(IsVisibleProperty, value);
            }
        }

        private static void OnIsVisiblePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var ThisItem = (WorkProgress)sender;
            if ((bool) e.NewValue == true)
                ThisItem.ThisLabel.Visibility = Visibility.Visible;
            else
                ThisItem.ThisLabel.Visibility = Visibility.Hidden;
        }

        private int _Total = 0;
        private int _Progress = 0;
        private readonly string _Process = "Processing"; //i think
        public event CompletedEventHandler Completed;

        public delegate void CompletedEventHandler();

        public event NoneToBeginWithEventHandler NoneToBeginWith;

        public delegate void NoneToBeginWithEventHandler();

        public event ContinueOnEventHandler ContinueOn;

        public delegate void ContinueOnEventHandler();

        public event BeginningOfProcessEventHandler BeginningOfProcess;

        public delegate void BeginningOfProcessEventHandler();

        private bool WasBeginning;

        public string GetProcess()
        {
            return _Process;
        }

        public int Index()
        {
            return _Progress-1;
        }

        public void BeginProcess<T>(CustomBasicList<T> ThisList)
        {
            _Total = ThisList.Count;
            _Progress = 0;
            DoResize();
            if (_Total == 0)
            {
                NoneToBeginWith();
                return;
            }
            WasBeginning = true;
            NextOne();
        }

        public void SkipSeveral(int HowMany)
        {
            if (_Progress + HowMany>_Total)
            {
                Completed();
                return;
            }
            DoResize();
            ContinueOn();
        }
        public void NextOne()
        {
            if (_Progress == _Total)
            {
                Completed();
                return;
            }
            _Progress++;
            DoResize();
            if (WasBeginning == true)
            {
                WasBeginning = false;
                BeginningOfProcess();
                return;
            }
            ContinueOn();
        }

        public void PreviousOne()
        {
            if (_Progress < 2)
                throw new BasicBlankException("Cannot goto the previous one because its at the beginning");
            _Progress--; //maybe this one reduces by one.
            DoResize();
            ContinueOn();
        }

        private void DoResize()
        {
            if (_Total < _Progress)
                throw new BasicBlankException("The total cannot be less than the progress made");
            ThisLabel.Visibility = Visibility.Visible;
            ThisLabel.Text = $"{_Process} #: {_Progress}     Total:  {_Total}";
        }
        
        public WorkProgress(TextBlock _Label)
        {
            ThisLabel = _Label;
        }

    }
}
