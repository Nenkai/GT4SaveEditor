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
using System.Windows.Navigation;
using System.Diagnostics;
using System.IO;

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

            string dir = System.IO.Path.GetFullPath(_db.FileName);
            dir = System.IO.Path.GetDirectoryName(dir);

            HyperLink_DatabaseFile.NavigateUri = new Uri($"file://{dir}");
            lb_CarName.Content = db.GetCarNameByCode(car.Sheets[0].CarCode.Code);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl tabControl)
            {
                CurrentSheet = new(Car.Sheets[tabControl.SelectedIndex]);
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = e.Uri.ToString(), UseShellExecute = true });
        }

        private void Hyperlink_DatabaseFileRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = e.Uri.ToString(), UseShellExecute = true });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public class Sheet
        {
            public CarEquipments _current;

            public Sheet(CarEquipments sheet)
            {
                _current = sheet;
            }

            public int Brake { get => _current.Brake.Code; set { _current.Brake.Code = value; } }
            public int BrakeController { get => _current.BrakeController.Code; set { _current.BrakeController.Code = value; } }
            public int CarCode { get => _current.CarCode.Code; set { _current.CarCode.Code = value; } }
            public int TunedCarCode { get => _current.TunedCarCode.Code; set { _current.TunedCarCode.Code = value; } }
            public int Variation { get => _current.Variation.Code; set { _current.Variation.Code = value; } }
            public byte VariationOrder { get => _current.VariationOrder; set { _current.VariationOrder = value; } }
            public int Chassis { get => _current.Chassis.Code; set { _current.Chassis.Code = value; } }
            public int Engine { get => _current.Engine.Code; set { _current.Engine.Code = value; } }
            public int Drivetrain { get => _current.Drivetrain.Code; set { _current.Drivetrain.Code = value; } }
            public int Gear { get => _current.Gear.Code; set { _current.Gear.Code = value; } }
            public int Suspension { get => _current.Suspension.Code; set { _current.Suspension.Code = value; } }
            public int LSD { get => _current.LSD.Code; set { _current.LSD.Code = value; } }
            public int FrontTire { get => _current.FrontTire.Code; set { _current.FrontTire.Code = value; } }
            public int RearTire { get => _current.RearTire.Code; set { _current.RearTire.Code = value; } }
            public int Steer { get => _current.Steer.Code; set { _current.Steer.Code = value; } }
            public int Lightweight { get => _current.Lightweight.Code; set { _current.Lightweight.Code = value; } }
            public int RacingModify { get => _current.RacingModify.Code; set { _current.RacingModify.Code = value; } }
            public int Portpolish { get => _current.Portpolish.Code; set { _current.Portpolish.Code = value; } }
            public int EngineBalance { get => _current.EngineBalance.Code; set { _current.EngineBalance.Code = value; } }
            public int Displacement { get => _current.Displacement.Code; set { _current.Displacement.Code = value; } }
            public int Computer { get => _current.Computer.Code; set { _current.Computer.Code = value; } }
            public int Natune { get => _current.Natune.Code; set { _current.Natune.Code = value; } }
            public int TurbineKit { get => _current.TurbineKit.Code; set { _current.TurbineKit.Code = value; } }
            public int Flywheel { get => _current.Flywheel.Code; set { _current.Flywheel.Code = value; } }
            public int Clutch { get => _current.Clutch.Code; set { _current.Clutch.Code = value; } }
            public int PropellerShaft { get => _current.PropellerShaft.Code; set { _current.PropellerShaft.Code = value; } }
            public int Muffler { get => _current.Muffler.Code; set { _current.Muffler.Code = value; } }
            public int Intercooler { get => _current.Intercooler.Code; set { _current.Intercooler.Code = value; } }
            public int ASCC { get => _current.ASCC.Code; set { _current.ASCC.Code = value; } }
            public int TCSC { get => _current.TCSC.Code; set { _current.TCSC.Code = value; } }
            public int Wheel { get => _current.Wheel.Code; set { _current.Wheel.Code = value; } }
            public int NOS { get => _current.NOS.Code; set { _current.NOS.Code = value; } }
            public int Wing { get => _current.Wing.Code; set { _current.Wing.Code = value; } }
            public int SuperCharger { get => _current.SuperCharger.Code; set { _current.SuperCharger.Code = value; } }

            public ushort GearRatio1 { get => _current.GearRatio1; set { _current.GearRatio1 = value; } }
            public ushort GearRatio2 { get => _current.GearRatio2; set { _current.GearRatio2 = value; } }
            public ushort GearRatio3 { get => _current.GearRatio3; set { _current.GearRatio3 = value; } }
            public ushort GearRatio4 { get => _current.GearRatio4; set { _current.GearRatio4 = value; } }
            public ushort GearRatio5 { get => _current.GearRatio5; set { _current.GearRatio5 = value; } }
            public ushort GearRatio6 { get => _current.GearRatio6; set { _current.GearRatio6 = value; } }
            public ushort GearRatio7 { get => _current.GearRatio7; set { _current.GearRatio7 = value; } }
            public ushort GearRatio8 { get => _current.GearRatio8; set { _current.GearRatio8 = value; } }
            public ushort GearRatio9 { get => _current.GearRatio9; set { _current.GearRatio9 = value; } }
            public ushort GearRatio10 { get => _current.GearRatio10; set { _current.GearRatio10 = value; } }
            public ushort GearRatio11 { get => _current.GearRatio11; set { _current.GearRatio11 = value; } }

            public ushort GearReverse { get => _current.GearReverse; set { _current.GearReverse = value; } }
            public ushort FinalGearRatio { get => _current.FinalGearRatio; set { _current.FinalGearRatio = value; } }
            public ushort LastFinalGearRatio { get => _current.LastFinalGearRatio; set { _current.LastFinalGearRatio = value; } }
            public ushort MaxSpeed { get => _current.MaxSpeed; set { _current.MaxSpeed = value; } }

            public byte FrontCamber { get => _current.Susp_FrontCamber; set => _current.Susp_FrontCamber = value; }
            public byte RearCamber { get => _current.Susp_RearCamber; set => _current.Susp_RearCamber = value; }
            public short RideHeightF { get => _current.Susp_RideHeightF; set => _current.Susp_RideHeightF = value; }
            public short RideHeightR { get => _current.Susp_RideHeightR; set => _current.Susp_RideHeightR = value; }
            public byte FrontToe { get => _current.Susp_FrontToe; set => _current.Susp_FrontToe = value; }
            public byte RearToe { get => _current.Susp_RearToe; set => _current.Susp_RearToe = value; }
            public byte FrontSpringRate { get => _current.Susp_FrontSpringRate; set => _current.Susp_FrontSpringRate = value; }
            public byte RearSpringRate { get => _current.Susp_RearSpringRate; set => _current.Susp_RearSpringRate = value; }
            public byte LeverRatioF { get => _current.leverRatioF; set => _current.leverRatioF = value; }
            public byte LeverRatioR { get => _current.leverRatioR; set => _current.leverRatioR = value; }
            public byte FrontDamperF1B { get => _current.Susp_FrontDamperF1B; set => _current.Susp_FrontDamperF1B = value; }
            public byte FrontDamperF2B { get => _current.Susp_FrontDamperF2B; set => _current.Susp_FrontDamperF2B = value; }
            public byte FrontDamperF1R { get => _current.Susp_FrontDamperF1R; set => _current.Susp_FrontDamperF1R = value; }
            public byte FrontDamperF2R { get => _current.Susp_FrontDamperF2R; set => _current.Susp_FrontDamperF2R = value; }
            public byte RearDamperF1B { get => _current.Susp_RearDamperF1B; set => _current.Susp_RearDamperF1B = value; }
            public byte RearDamperF2B { get => _current.Susp_RearDamperF2B; set => _current.Susp_RearDamperF2B = value; }
            public byte RearDamperF1R { get => _current.Susp_RearDamperF1R; set => _current.Susp_RearDamperF1R = value; }
            public byte RearDamperF2R { get => _current.Susp_RearDamperF2R; set => _current.Susp_RearDamperF2R = value; }
            public byte FrontStabilizer { get => _current.Susp_FrontStabilizer; set => _current.Susp_FrontStabilizer = value; }
            public byte RearStabilizer { get => _current.Susp_RearStabilizer; set => _current.Susp_RearStabilizer = value; }
            public byte FrontPreLoadLevel { get => _current.Susp_FrontPreLoadLevel; set => _current.Susp_FrontPreLoadLevel = value; }
            public byte RearPreLoadLevel { get => _current.Susp_RearPreLoadLevel; set => _current.Susp_RearPreLoadLevel = value; }
            public byte FrontSpringRateLevel { get => _current.Susp_FrontSpringRateLevel; set => _current.Susp_FrontSpringRateLevel = value; }
            public byte RearSpringRateLevel { get => _current.Susp_RearSpringRateLevel; set => _current.Susp_RearSpringRateLevel = value; }

            public byte LSDParam_FrontTrq { get => _current.LSDParam_FrontTorque; set => _current.LSDParam_FrontTorque = value; }
            public byte LSDParam_RearTrq { get => _current.LSDParam_RearTorque; set => _current.LSDParam_RearTorque = value; }
            public byte LSDParam_FrontAccel { get => _current.LSDParam_FrontAccel; set => _current.LSDParam_FrontAccel = value; }
            public byte LSDParam_RearAccel { get => _current.LSDParam_RearAccel; set => _current.LSDParam_RearAccel = value; }
            public byte LSDParam_FrontDecel { get => _current.LSDParam_FrontDecel; set => _current.LSDParam_FrontDecel = value; }
            public byte LSDParam_RearDecel { get => _current.LSDParam_RearDecel; set => _current.LSDParam_RearDecel = value; }

            public byte TurboBoost1 { get => _current.Turbo_Boost1; set => _current.Turbo_Boost1 = value; }
            public byte TurboPeakRPM1 { get => _current.Turbo_PeakRPM1; set => _current.Turbo_PeakRPM1 = value; }
            public byte TurboResponse1 { get => _current.Turbo_Response1; set => _current.Turbo_Response1 = value; }
            public byte TurboBoost2 { get => _current.Turbo_Boost2; set => _current.Turbo_Boost2 = value; }
            public byte TurboPeakRPM2 { get => _current.Turbo_PeakRPM2; set => _current.Turbo_PeakRPM2 = value; }
            public byte TurboResponse2 { get => _current.Turbo_Response2; set => _current.Turbo_Response2 = value; }

            public byte PowerMultiplier { get => _current.PowerMultiplier; set { _current.PowerMultiplier = value; } }
            public byte GripMultiplier { get => _current.WeightMultiplier; set { _current.WeightMultiplier = value; } }

        }
    }
}
