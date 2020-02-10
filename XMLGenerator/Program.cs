using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XMLGenerator
{
    class Program
    {
        public static int Main()
        {
            int weeks;
            int daysPerWeek;
            int total_days;
            List<TimeSlot> timeSlots = new List<TimeSlot>();
            TimeTable Output = new TimeTable();

        start:
        week_input:
            Console.WriteLine();
            Console.WriteLine("How many weeks before cycling (1-52):");
            weeks = Convert.ToInt32(Console.ReadLine());
            if (!(1<=weeks && weeks<=52))
            {
                Console.WriteLine("Weeks must be between 1 and 52.");
                goto week_input;
            }
            Output.weeks = weeks;

        counter_start:
            Console.WriteLine();
            Console.WriteLine("Comma separated list of day of year for starting counters.");
            Console.WriteLine("This means the first day of term.");
            Output.CounterStart = Console.ReadLine().Split(",".ToCharArray()).ToList().ConvertAll<int>(Convert.ToInt32);

        counter_end:
            Console.WriteLine();
            Console.WriteLine("Comma separated list of day of year for ending counters.");
            Console.WriteLine("This means the last day of term.");
            Output.CounterEnd = Console.ReadLine().Split(",".ToCharArray()).ToList().ConvertAll<int>(Convert.ToInt32);

        daysPerWeek_input:
            /*
            Console.WriteLine();
            Console.WriteLine("How many school days per week (1-7):");
            daysPerWeek = Convert.ToInt32(Console.ReadLine());
            if (!(1 <= daysPerWeek && daysPerWeek <= 7))
            {
                Console.WriteLine("Must be between 1 and 7 school days per week.");
                goto daysPerWeek_input;
            }
            */

            Console.WriteLine("Total slots.");
            int totalSlots = Convert.ToInt32(Console.ReadLine());

            for (int j = 0; j < totalSlots; j++)
            {
                TimeSlot newtimeSlot = new TimeSlot();
                Console.WriteLine();
                Console.WriteLine("Timeslot " + Convert.ToString(j + 1));
                Console.WriteLine("Name of this timeslot:");
                string temp2 = Console.ReadLine();
                newtimeSlot.name = temp2;
                Console.WriteLine("Location of this timeslot:");
                string temp3 = Console.ReadLine();
                newtimeSlot.room = temp3;
                Console.WriteLine("Start hour of this timeslot:");
                int temp4 = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Start minute of this timeslot:");
                int temp5 = Convert.ToInt32(Console.ReadLine());
                DateTime temp6 = new DateTime(1, 1, 1, temp4, temp5, 0);
                newtimeSlot.Start = temp6;

                Console.WriteLine("End hour of this timeslot:");
                int temp7 = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("End minute of this timeslot:");
                int temp8 = Convert.ToInt32(Console.ReadLine());
                DateTime temp9 = new DateTime(1, 1, 1, temp7, temp8, 0);
                newtimeSlot.End = temp9;

                timeSlots.Add(newtimeSlot);
            }

            Output.timeSlots = timeSlots;

            Console.WriteLine();
            Console.WriteLine("Current data is:");

            byte[] display = UserIO.Serialise<TimeTable>(Output);
            Console.WriteLine(Encoding.Default.GetString(display));

            Console.WriteLine("Restart?");
            if (Console.ReadKey(true).Key == ConsoleKey.Y)
            {
                goto start;
            }

            UserIO.Save<TimeTable>("Timetable", UserIO.Serialise<TimeTable>(Output));

            //write timetable to file now

            return 0;
        }
    }
}
