using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommonBasicStandardLibraries.MVVMHelpers;
using BasicControlsAndWindows.BasicWindows.Misc;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.ContainerClasses;
namespace BasicControlsAndWindows.BasicWindows.Windows
{
    public abstract class BasicWindow<VM> : Window, IUISetting, IFocusOnFirst where VM : BaseViewModel //can no longer create new since dependency injection is required for other things now.
    {

        
        protected VM ThisMod;

		protected IPersonalSetting ThisPersonal = new BlankPersonal(); //was going to force it to be done but chose not to.

        protected virtual void StartUp() { }

        protected abstract void RegisterInterfaces();

        protected abstract void AfterViewModel();

        protected abstract void AfterInitialBuild();

        protected virtual Size DefaultWindowSize()
        {
            return new Size(1200, 750);
        }

        protected abstract Brush ButtonBackColor();

        protected abstract Brush ButtonForeColor();

        protected abstract Brush LabelForeColor();

        protected abstract Brush WindowBackgroundColor();

        protected virtual int GetFontSize()
        {
            return 35; //this can change too
        }

        public Binding GetVisibleBinding(string VisiblePath)
        {
            return WindowHelper.GetVisibleBinding(VisiblePath);

        }

        private void FirstRegister()
        {
            OurContainer.RegisterSingleton<ISimpleUI>(this);
            OurContainer.RegisterSingleton<IFocusOnFirst>(this); //looks like can't automatically register the parts for the contact management.
            OurContainer.RegisterType<VM>(true); //this is to register the view model being used.
        }

        protected ContainerMain OurContainer;

        protected void BuildXAML()
        {
            OurContainer = new ContainerMain(); //i think you need the container even before then in case you want to register something unusual
            StartUp();
            OS = EnumOS.WindowsDT;
            cons = OurContainer;
            ShowCurrentUser();
            FirstRegister();
            RegisterInterfaces();
            ThisMod = cons.Resolve<VM>(); //i think
			AfterViewModel();
            WindowHelper.CurrentWindow = this;
            WindowHelper.SetDefaultLocation();
            WindowHelper.SetTitleBindings(ThisMod);
            var TempSize = DefaultWindowSize();
            WindowHelper.SetSize(TempSize.Width, TempSize.Height);
            var ThisColor = WindowBackgroundColor();
            if (ThisColor != null)
                Background = ThisColor;
            AfterInitialBuild();
        }


        protected void ShowCurrentUser()
        {
            ThisPersonal.ShowCurrentUser(); //instead of overriding, just create a new behavior.
        }

        public void CloseProgram()
        {
            Cursor = Cursors.Arrow;
            Application.Current.Shutdown(); //hopefully this works as well.
        }

        public abstract void FocusOnFirstControl();


        public virtual void ShowError(string Message)
        {
            MessageBox.Show(Message);
            CloseProgram();
        }

        

        public async  Task ShowMessageBox(string Message)
        {
            await BasicDataFunctions.WaitBlank();
            MessageBox.Show(Message);
        }

        protected virtual bool HasBorderWhiteBrush() { return false; }


        public Button GetDefaultButton(string Text, string CommandPath, object CommandParameter = null)
        {
            Button ThisBut = new Button
            {
                FontSize = GetFontSize()
            };
            var ThisColor = ButtonBackColor();
            if (ThisColor != null)
                ThisBut.Background = ThisColor;
            ThisColor = ButtonForeColor();
            if (ThisColor != null)
                ThisBut.Foreground = ThisColor;
            ThisBut.SetBinding(Button.CommandProperty, new Binding(CommandPath));
            if (CommandParameter != null)
                ThisBut.CommandParameter = CommandParameter;
            ThisBut.Content = Text;
            if (HasBorderWhiteBrush() == true)
            {
                ThisBut.BorderBrush = Brushes.White;
                ThisBut.BorderThickness = new Thickness(1, 1, 1, 1);
            }
            ThisBut.HorizontalAlignment = HorizontalAlignment.Left;
            ThisBut.VerticalAlignment = VerticalAlignment.Top;
            ThisBut.Margin = new Thickness(2, 2, 2, 2);
            return ThisBut;

        }

        public virtual TextBlock GetDefaultLabel()
        {
            TextBlock ThisLabel = new TextBlock
            {
                Foreground = LabelForeColor()
            };
            return ThisLabel;
        }
    }
}
