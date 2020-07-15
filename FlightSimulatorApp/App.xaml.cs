using FlightSimulatorApp.Model;
using FlightSimulatorApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FlightSimulatorApp
{ 
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public MapViewModel Mapvm { get; internal set; }
        public CommunicationViewModel Commvm { get; internal set; }
        public DashBoardViewModel Dashvm { get; internal set; }
        public JoystickAndSlidersViewModel Joyvm { get; internal set; }
        public SuperModel Model { get; internal set; }
        public MessageViewModel Msgvm { get; internal set; }

        /// <summary>Handles the Startup event of the Application control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="StartupEventArgs" /> instance containing the event data.</param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Model = new SuperModel(new Client());
            Mapvm = new MapViewModel(Model);
            Commvm = new CommunicationViewModel(Model);
            Dashvm = new DashBoardViewModel(Model);
            Joyvm = new JoystickAndSlidersViewModel(Model);
            Msgvm = new MessageViewModel(Model);
        }
    }
}
