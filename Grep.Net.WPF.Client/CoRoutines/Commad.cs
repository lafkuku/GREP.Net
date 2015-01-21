using System;
using System.Linq;
using System.Windows.Input;

namespace Grep.Net.WPF.Client.CoRoutines
{
    public class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public Func<Object, bool> CanExecuteAction { get; set; }

        public Action<Object> ExecuteAction { get; set; }

        public Command(Func<Object, bool> canExecute, Action<Object> execute)
        {
            CanExecuteAction = canExecute;
            ExecuteAction = execute; 
        }

        public bool CanExecute(object parameter)
        {
            if (CanExecuteAction != null)
            {
                return CanExecuteAction(parameter); 
            }
            return false;
        }
     
        public void Execute(object parameter)
        {
            if (ExecuteAction != null)
            {
                ExecuteAction(parameter);
            }
            else
            {
                throw new NotImplementedException("Action execute method is missing.. "); 
            }
        }
    }
}