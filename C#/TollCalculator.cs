using PublicHoliday;
using System;
using System.Globalization;
using TollFeeCalculator;

public class TollCalculator {

    /**
     * Calculate the total toll fee for one day
     *
     * @param vehicle - the vehicle
     * @param dates   - date and time of all passes on one day
     * @return - the total toll fee for that day
     */

    public int GetTollFee(Vehicle vehicle, DateTime[] dates)
    {
        var dayTimeStamps = dates;
        int totalDayFee = 0;
        //TODO: Seperate by dates
        totalDayFee += GetTollFeeForDay(vehicle, dayTimeStamps);
        return totalDayFee;
    }
   
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

        int hour = date.Hour;
        int minute = date.Minute;

        if (hour == 6 && minute >= 0 && minute <= 29) return 8;
        else if (hour == 6 && minute >= 30 && minute <= 59) return 13;
        else if (hour == 7 && minute >= 0 && minute <= 59) return 18;
        else if (hour == 8 && minute >= 0 && minute <= 29) return 13;
        //First half hour is missed 8 - 14 
        else if (hour >= 8 && hour <= 14 && minute >= 30 && minute <= 59) return 8;
        else if (hour == 15 && minute >= 0 && minute <= 29) return 13;
        //What? first should be ">= 30"? TECHNICALLY WORKS, prev if statement
        else if (hour == 15 && minute >= 0 || hour == 16 && minute <= 59) return 18;
        else if (hour == 17 && minute >= 0 && minute <= 59) return 13;
        else if (hour == 18 && minute >= 0 && minute <= 29) return 8;
        else return 0;
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


    private Boolean IsTollFreeDate(DateTime date) {
        //Weekends are free
        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) return true;
        if (date.Month == 7) return true;
        var holidays = new SwedenPublicHoliday().PublicHolidays(date.Year);
        if (holidays.Contains(date.Date)) return true;
/*        if (year == 2013) {
            if (month == 1 && day == 1 || // Nyårs dagen
                month == 3 && (day == 28 || day == 29) || //Skärtorsdag Å långfredag
                month == 4 && (day == 1 || day == 30) || // Annan dag Påsk Å Kungensfödelsedag
                month == 5 && (day == 1 || day == 8 || day == 9) || // Första maj, dagen före + Kristihimmelsfärd
                month == 6 && (day == 5 || day == 6 || day == 21) || // dagen före + Nationaldagen, Sommarsolståndet
                month == 7 || // Hela Juli
                month == 11 && day == 1 || // Allhelgonadagen
                month == 12 && (day == 24 || day == 25 || day == 26 || day == 31)) // Julafton, Juldagen, Annandag jul, Nyårsafton
            {
                return true;
            }
        }*/
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