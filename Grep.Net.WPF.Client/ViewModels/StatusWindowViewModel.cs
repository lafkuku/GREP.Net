using System;
using System.Linq;
using Caliburn.Micro;
using Grep.Net.WPF.Client.Controls;

namespace Grep.Net.WPF.Client.ViewModels
{
    public class StatusWindowViewModel : Screen
    {
        public IResult Command { get; set; }

        public CircularProgressBar WheelOfUncertainty { get; set; }

        public String Message { get; set; }

        public StatusWindowViewModel(IResult command, string message = "Please Wait...")
        {
            Command = command;
            Command.Completed += new EventHandler<ResultCompletionEventArgs>(Command_Completed);
            WheelOfUncertainty = new CircularProgressBar();
            Message = message;

            NotifyOfPropertyChange(() => WheelOfUncertainty);

            WheelOfUncertainty.Start(); 
        }

        private void Command_Completed(object sender, ResultCompletionEventArgs e)
        {
            Execute.OnUIThread(() =>
            {
                WheelOfUncertainty.Stop(); 
                TryClose();
            });
        }
    }
}