using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using BasicControlsAndWindows.Controls.Interfaces;

namespace BasicControlsAndWindows.Controls.BasicControls
{
    public class CustomTextbox : UserControl, IDataEntry
    {

        private readonly TextBox ThisText;
        public EnumDataEntryCategory Category => EnumDataEntryCategory.SingleLineTextbox;

        public string BindingPath { get; set; }

        public event EventHandler HasEntered;

        public bool HasValidationError()
        {
            BindingExpression ThisBind = GetBindingExpression(TextBox.TextProperty);
            return ThisBind.HasError;
        }

        //void IBasicEntry.Focus()
        //{
        //    Focus();
        //}


        public new void Focus()
        {
            //throw new Exception("Hidden Focus");
            //Focus(); //this seems to work
            CustomFocus();
        }

        public void CustomFocus()
        {
            ThisText.Focus();
        }

        public CustomTextbox()
        {
            ThisText = new TextBox();
            {
                FontSize = 12;
            }
            Validation.SetErrorTemplate(ThisText, null);
            Content = ThisText;
            ThisText.GotFocus += ThisText_GotFocus;
            ThisText.GotKeyboardFocus += SelectAll;
            ThisText.MouseDoubleClick += SelectAll;
            ThisText.PreviewMouseLeftButtonDown += MouseButtonIgnor;
            ThisText.KeyDown += ThisText_KeyDown;
        }

        private void ThisText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
                return; //even if we don't have enter command sent in, ignore

            if (EnterCommand == null)
            {
                HasEntered?.Invoke(this, new EventArgs()); //i think if there is a command, then needs to focus on command instead.
                AttemptedToSubmitForm = true; //i think
                return;
            }
            BindingExpression ThisBind = ThisText.GetBindingExpression(TextBox.TextProperty);
            ThisBind.UpdateSource();
            if (EnterCommand.CanExecute(null) == true)
            {
                EnterCommand.Execute(null);
                return;
            }
            HasEntered?.Invoke(this, new EventArgs()); //still needs to raise the hasentered (so you can do additional processing)
            AttemptedToSubmitForm = true; //tried but failed
        }

        //will try to do without the arrow keys.  if i really need it, rethink.
        //attempt to do no textchange event. if that is needed, rethink.


        private void MouseButtonIgnor(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is TextBox == false)
                return;
            TextBox ThisText = (TextBox)sender;
            if (ThisText.IsKeyboardFocusWithin == false)
                return;
            e.Handled = true;
            ThisText.Focus();
        }

        private void SelectAll(object sender, RoutedEventArgs e) //i think
        {
            if (sender is TextBox == false)
                return;
            TextBox ThisText = (TextBox)sender;
            ThisText.SelectAll();
        }

        private void ThisText_GotFocus(object sender, RoutedEventArgs e)
        {
            Focus();
        }

        public void SendTextBinding(BindingBase ThisBind)
        {
            ThisText.SetBinding(TextBox.TextProperty, ThisBind);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name == nameof(FontSize))
                ThisText.FontSize =  (double) e.NewValue;
            base.OnPropertyChanged(e);
        }

        private static void OnTextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var ThisItem = (CustomTextbox)sender;
            ThisItem.ThisText.Text =  e.NewValue.ToString();
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(CustomTextbox), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnTextPropertyChanged)));

        public string Text
        {
            get { return ThisText.Text; }
            set { SetValue(TextProperty, value); }
        }


        public static readonly DependencyProperty EnterCommandProperty = DependencyProperty.Register("EnterCommand", typeof(ICommand), typeof(CustomTextbox), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(EnterCommandPropertyChanged)));
        public ICommand EnterCommand
        {
            get
            {
                return (ICommand)GetValue(EnterCommandProperty);
            }
            set
            {
                SetValue(EnterCommandProperty, value);
            }
        }


        private static void EnterCommandPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {

        }

        public static DependencyProperty AttemptedToSubmitFormProperty = DependencyProperty.Register("AttemptedToSubmitForm", typeof(bool), typeof(CustomTextbox), new PropertyMetadata(true));
        public bool AttemptedToSubmitForm
        {
            get
            {
                return (bool) GetValue(AttemptedToSubmitFormProperty);
            }
            set
            {
                SetValue(AttemptedToSubmitFormProperty, value);
            }
        }



    }
}
