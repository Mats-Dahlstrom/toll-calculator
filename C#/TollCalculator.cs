using System;

namespace Test_C__app.Afry_toll_calculator_fork.C_
{
    public interface TollCalculator
    {
        int GetTollFee(VehicleType vehicle, DateTime[] dates);
        int GetTollFeeForDay(VehicleType vehicle, DateTime[] dayTimeStamps);
        int GetTollFee(VehicleType vehicle, DateTime date);
        VehicleType[] TollFreeVehicleTypes { get; }
    }
    public class FeeSpan
    {
        public TimeSpan StartTime{ get => _startTime; }
        public int Fee { get => _fee; }
        public FeeSpan(TimeSpan startTime, int fee)
        {
            _fee = fee; 
            _startTime = startTime;
        }
        private int _fee;
        private TimeSpan _startTime;
    }
    public enum VehicleType
    {
        Car = 0,
        Motorbike = 1,
        Tractor = 2,
        Emergency = 3,
        Diplomat = 4,
        Foreign = 5,
        Military = 6
    }
}
