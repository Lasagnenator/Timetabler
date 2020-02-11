using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Timetablerv2
{

    //list of days that has a list of timeslots
    public class TimeTable
    {
        public List<TimeSlot> timeSlots;
        public int weeks; //number of weeks for cycling
        public List<int> CounterStart;
        public List<int> CounterEnd;
    }

    public class TimeSlot
    {
        public string name;
        public string room;
        public DateTime Start;
        public DateTime End;
    }

    public class Utils
    {

        public static string GetTimeSlotName(TimeSlot Timeslot)
        {
            return Timeslot.name;
        }

        public static string GetTimeSlotRoom(TimeSlot Timeslot)
        {
            return Timeslot.room;
        }

        public static DateTime GetTimeSlotStart(TimeSlot Timeslot)
        {
            return Timeslot.Start;
        }

        //Returns null if none are current
        public static TimeSlot GetCurrentTimeSlot(TimeTable Database)
        {
            List<TimeSlot> TimeSlots = Database.timeSlots;
            TimeSlot Current = null;
            DateTime Now = DateTime.Now;
            TimeSpan Delta = GetStartDateTimeSpan(Database);

            foreach (var item in TimeSlots)
            {
                DateTime Start = item.Start;
                DateTime End = item.End;
                
                Start += Delta;
                Start = Start.AddYears(Now.Year - 1);
                End += Delta;
                End = End.AddYears(Now.Year - 1);
                /*
                Start = Start.AddYears(Now.Year-1);
                Start = Start.AddMonths(Now.Month-1);
                Start = Start.AddDays(Now.Day-1);

                End = End.AddYears(Now.Year-1);
                End = End.AddMonths(Now.Month-1);
                End = End.AddDays(Now.Day-1);
                */

                if ((DateTime.Compare(Start, Now)<0) && (DateTime.Compare(Now, End)<0))
                {
                    Current = item;
                    break;
                }
            }

            return Current;
        }

        public static TimeSpan GetStartDateTimeSpan(TimeTable Database)
        {
            int ThisDay = DateTime.Now.DayOfYear;
            int term_start = 0;

            for (int i = 0; i < Database.CounterStart.Count; i++)
            {
                if ((ThisDay >= Database.CounterStart[i]) && (ThisDay <= Database.CounterEnd[i]))
                {
                    term_start = Database.CounterStart[i];
                    break;
                }
            }

            if (term_start == 0)
            {
                return new TimeSpan(0);
            }

            int DayIndex = (ThisDay - term_start) % (Database.weeks * 7); //which day within the week it is
            int Delta = DateTime.Now.DayOfYear - DayIndex; //What day of year is the preivous start of cycle
            return new TimeSpan(Delta-1, 0,0,0);
        }

        public static TimeSlot GetNextTimeSlot(TimeTable Database)
        {
            List<TimeSlot> TimeSlots = Database.timeSlots;
            TimeSlot Current = GetCurrentTimeSlot(Database);
            if (Current == null) { return null; }
            int NextIndex = TimeSlots.IndexOf(Current);
            if (NextIndex+1 >= TimeSlots.Count) { return null; }
            return TimeSlots[NextIndex+1];
        }

        public static int MinutesTillNext(TimeSlot Next, TimeSpan Delta)
        {
            DateTime temp = Next.Start;
            temp += Delta;
            temp = temp.AddYears(DateTime.Now.Year - 1);
            //temp = temp.AddDays(DateTime.Now.Day-1);
            //temp = temp.AddMonths(DateTime.Now.Month-1);
            TimeSpan diff = temp.Subtract(DateTime.Now);
            return Convert.ToInt32(diff.TotalMinutes);
        }

        public static double CurrentProgress(TimeSlot Current, TimeSpan Delta)
        {
            double Length = Current.End.Subtract(Current.Start).TotalSeconds;
            DateTime temp = Current.Start;
            temp += Delta;
            temp = temp.AddYears(DateTime.Now.Year-1);
            //temp = temp.AddMonths(DateTime.Now.Month-1);
            //temp = temp.AddDays(DateTime.Now.Day-1);
            double NowStartDiff = DateTime.Now.Subtract(temp).TotalSeconds;
            return NowStartDiff / Length;
        }
    }
    
}