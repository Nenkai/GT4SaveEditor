using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;

using PDTools.Structures;
using PDTools.Structures.PS2;
using GT4SaveEditor.Database;

namespace GT4SaveEditor
{
    public partial class CarGarageEditorWindow : Window, INotifyPropertyChanged
    {
        public CarGarage Car { get; set; }
        private GT4Database _db;

        private Sheet _currentSheet;
        public Sheet CurrentSheet { get => _currentSheet; set { _currentSheet = value; OnPropertyChanged(nameof(CurrentSheet)); } }

        public CarGarageEditorWindow(CarGarage car, GT4Database db)
        {
            Car = car;
            CurrentSheet = new Sheet(car.Sheets[0]);

            _db = db;

            InitializeComponent();

            lb_CarName.Content = db.GetCarNameByCode(car.Sheets[0].CarCode.Code);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl tabControl)
            {
                CurrentSheet = new(Car.Sheets[tabControl.SelectedIndex]);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        
        public class Sheet
        {
            public CarEquipments _current;

            public Sheet(CarEquipments sheet)
            {
                _current = sheet;
            }

            public int Brake { get => _current.Brake.Code; set { _current.Brake.Code = value; }}
            public int BrakeController { get => _current.BrakeController.Code; set { _current.BrakeController.Code = value;} }
            public int CarCode { get => _current.CarCode.Code; set { _current.CarCode.Code = value; } }
            public int TunedCarCode { get => _current.TunedCarCode.Code; set { _current.TunedCarCode.Code = value; } }
            public int Variation { get => _current.Variation.Code; set { _current.Variation.Code = value; } }
            public byte VariationOrder { get => _current.VariationOrder; set { _current.VariationOrder = value; } }
            public int Chassis { get => _current.Chassis.Code; set { _current.Chassis.Code = value;} }
            public int Engine { get => _current.Engine.Code; set { _current.Engine.Code = value; } }
            public int Drivetrain { get => _current.Drivetrain.Code; set { _current.Drivetrain.Code = value; } }
            public int Gear { get => _current.Gear.Code; set { _current.Gear.Code = value; } }
            public int Suspension { get => _current.Suspension.Code; set { _current.Suspension.Code = value; } }
            public int LSD { get => _current.LSD.Code; set { _current.LSD.Code = value; } }
            public int FrontTire { get => _current.FrontTire.Code; set { _current.FrontTire.Code = value;  } }
            public int RearTire { get => _current.RearTire.Code; set { _current.RearTire.Code = value;  } }
            public int Steer { get => _current.Steer.Code; set { _current.Steer.Code = value;} }
            public int Lightweight { get => _current.Lightweight.Code; set { _current.Lightweight.Code = value; } }
            public int RacingModify { get => _current.RacingModify.Code; set { _current.RacingModify.Code = value;  } }
            public int Portpolish { get => _current.Portpolish.Code; set { _current.Portpolish.Code = value; } }
            public int EngineBalance { get => _current.EngineBalance.Code; set { _current.EngineBalance.Code = value;} }
            public int Displacement { get => _current.Displacement.Code; set { _current.Displacement.Code = value; } }
            public int Computer { get => _current.Computer.Code; set { _current.Computer.Code = value;  } }
            public int Natune { get => _current.Natune.Code; set { _current.Natune.Code = value; } }
            public int TurbineKit { get => _current.TurbineKit.Code; set { _current.TurbineKit.Code = value;} }
            public int Flywheel { get => _current.Flywheel.Code; set { _current.Flywheel.Code = value;} }
            public int Clutch { get => _current.Clutch.Code; set { _current.Clutch.Code = value; } }
            public int PropellerShaft { get => _current.PropellerShaft.Code; set { _current.PropellerShaft.Code = value; } }
            public int Muffler { get => _current.Muffler.Code; set { _current.Muffler.Code = value; } }
            public int Intercooler { get => _current.Intercooler.Code; set { _current.Intercooler.Code = value; } }
            public int ASCC { get => _current.ASCC.Code; set { _current.ASCC.Code = value; } }
            public int TCSC { get => _current.TCSC.Code; set { _current.TCSC.Code = value; } }
            public int Wheel { get => _current.Wheel.Code; set { _current.Wheel.Code = value;} }
            public int NOS { get => _current.NOS.Code; set { _current.NOS.Code = value; } }
            public int Wing { get => _current.Wing.Code; set { _current.Wing.Code = value; } }
            public int SuperCharger { get => _current.SuperCharger.Code; set { _current.SuperCharger.Code = value;} }
        }
    }
}
