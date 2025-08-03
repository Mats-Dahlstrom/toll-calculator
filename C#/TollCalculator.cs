using PublicHoliday;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using TollFeeCalculator;

public class TollCalculator
{
    /* The list of fee timespans
     * Order:Hour,Minute,Fee
     * 
    */
    private (int,int,int)[] FeeSpans =
    {
        (00, 00, 00),
        (06, 00, 08),
        (06, 30, 13),
        (07, 00, 18),
        (08, 00, 13),
        (08, 30, 08),
        (15, 00, 13),
        (15, 30, 18),
        (17, 00, 13),
        (18, 00, 08),
        (18, 30, 00),
    };

    /**
     * Calculate the total toll fee for the sent data
     *
     * @param vehicle - the vehicle
     * @param dates   - date and time of all passes for the veichle
     * @return - the total toll fee for that day
     */

    public int GetTollFee(Vehicle vehicle, DateTime[] dates)
    {
        var dayTimeStamps = dates;
        int totalDayFee = 0;
        var datesSeperatedByDay =
            from date in dates
            group date by date.Date into d
            select  d.ToArray() ;
        //TODO: Seperate by dates
        foreach (var currentDates in datesSeperatedByDay)
        {
            totalDayFee += GetTollFeeForDay(vehicle, currentDates);
        }
        return totalDayFee;
    }

    /**
     * Calculate the total toll fee for one day
     *
     * @param vehicle - the vehicle
     * @param dates   - date and time of all passes on one day
     * @return - the total toll fee for that day
     */

    public int GetTollFeeForDay(Vehicle vehicle, DateTime[] dayTimeStamps)
    {
        //Should check if dates are not of the same day
        DateTime intervalStart = dayTimeStamps[0];
        int totalFee = 0;
        int lastFee = 0;
        foreach (DateTime date in dayTimeStamps) {
            int nextFee = GetTollFee(date, vehicle);

            long diffInTicks = date.Ticks - intervalStart.Ticks;
            double minutes = TimeSpan.FromTicks(diffInTicks).TotalMinutes;
            if (minutes <= 60d) {
                if (nextFee >= lastFee) lastFee = nextFee;
            } else {
                //Intervalstart should get reset here
                totalFee += lastFee;
                lastFee = nextFee;
                intervalStart = date;
            }
        }
        totalFee += lastFee;
        if (totalFee > 60) totalFee = 60;
        return totalFee;
    }

    private bool IsTollFreeVehicle(Vehicle vehicle) {
        if (vehicle == null) return false;
        String vehicleType = vehicle.GetVehicleType();
        return vehicleType.Equals(TollFreeVehicles.Motorbike.ToString()) ||
               vehicleType.Equals(TollFreeVehicles.Tractor.ToString()) ||
               vehicleType.Equals(TollFreeVehicles.Emergency.ToString()) ||
               vehicleType.Equals(TollFreeVehicles.Diplomat.ToString()) ||
               vehicleType.Equals(TollFreeVehicles.Foreign.ToString()) ||
               vehicleType.Equals(TollFreeVehicles.Military.ToString());
    }

    public int GetTollFee(DateTime date, Vehicle vehicle) {
        if (IsTollFreeDate(date) || IsTollFreeVehicle(vehicle)) return 0;

        int timeStamp = (date.Hour * 100) + date.Minute;
        var fee =
            from span in FeeSpans
            where ((span.Item1 * 100) + span.Item2) <= timeStamp
            select span.Item3;
        return fee.Last();

    }
    /*
    * Fees will differ between 8 SEK and 18 SEK, depending on the time of day 
    * Rush-hour traffic will render the highest fee
    * The maximum fee for one day is 60 SEK
    * A vehicle should only be charged once an hour
      * In the case of multiple fees in the same hour period, the highest one applies.
    * Some vehicle types are fee-free
    * Weekends and holidays are fee-free
    */
    /* Fees Estimated
     * 06:00 - 06:29    08
     * 06:30 - 06:59    13
     * 07:00 - 07:59    18
     * 08:00 - 08:29    13
     * 08:30 - 14:59    08
     * 15:00 - 15:29    13
     * 15:30 - 16:59    18
     * 17:00 - 17:59    13
     * 18:00 - 18:29    08
     * 18:30 - 05:59    FREE
     */

    private Boolean IsTollFreeDate(DateTime date) {
        //Weekends are free
        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) return true;
        if (date.Month == 7) return true;
        var holidays = new SwedenPublicHoliday().PublicHolidays(date.Year);
        if (holidays.Contains(date.Date)) return true;

        return false;
    }

    //Använd enum ELLER string
    private enum TollFreeVehicles {
        Motorbike = 0,
        Tractor = 1,
        Emergency = 2,
        Diplomat = 3,
        Foreign = 4,
        Military = 5
    }
}