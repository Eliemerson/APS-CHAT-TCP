using System;
using System.Windows.Input;

namespace APS_2022_1.MVVM.Core
{
    public class RelyCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        { 
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelyCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }
        public bool CanExecute(object parameter)
        {
            //Valida se os elemento contem valor para liberar o comando a ser executado
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            //DIspara o evento para FUNC
            this.execute(parameter);
        }
    }
}
