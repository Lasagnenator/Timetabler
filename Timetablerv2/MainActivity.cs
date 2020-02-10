using System;
using System.Threading.Tasks;
using System.Timers;
using Android;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace Timetablerv2
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        public TimeTable Database;
        public ProgressBar CurrentEventProgressBar;
        public TextView CurrentLocationTextView;
        public TextView CurrentEventNameTextView;

        public TextView NextLocationTextView;
        public TextView NextEventNameTextView;
        public TextView NextEventCounterTextview;

        public TextView DayNameTextView;
        public TextView WeekTextView;

        public Button RefreshButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            RequestPermissions(new string[] { Manifest.Permission.ReadExternalStorage }, 1);
            UserIO.root = ApplicationContext.GetExternalFilesDir(null).CanonicalPath;

            //Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            //SetSupportActionBar(toolbar);

            //FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            //fab.Click += FabOnClick;

            CurrentEventProgressBar = FindViewById<ProgressBar>(Resource.Id.CurrentEventProgressBar);
            CurrentLocationTextView = FindViewById<TextView>(Resource.Id.CurrentLocationTextView);
            CurrentEventNameTextView = FindViewById<TextView>(Resource.Id.CurrentEventNameTextView);
            
            NextLocationTextView = FindViewById<TextView>(Resource.Id.NextLocationTextView);
            NextEventNameTextView = FindViewById<TextView>(Resource.Id.NextEventNameTextView);
            NextEventCounterTextview = FindViewById<TextView>(Resource.Id.NextEventCounterTextview);

            RefreshButton = FindViewById<Button>(Resource.Id.RefreshButton);

            RefreshButton.Click += RefreshTimeTable;

            RefreshTimeTable(null, null);

            Timer t = new Timer(60000)
            {
                AutoReset = true
            };
            t.Elapsed += UpdateAll;

            //This syncs the timer to run when the minute changes
            Timer t_sync = new Timer((60 - DateTime.Now.Second)*1000) { AutoReset=false};
            t_sync.Elapsed += (sender, args) => { t.Start(); Toast.MakeText(ApplicationContext, "Updater synced.", ToastLength.Long).Show(); };
            t_sync.Start();

            //t.Start();

            //Toast.MakeText(ApplicationContext, "Here", ToastLength.Long).Show();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if ((requestCode == 1) && (grantResults.Length == 1) && (grantResults[0]==Android.Content.PM.Permission.Granted))
            {
                //permission granted
                Toast.MakeText(this.ApplicationContext, "Permission granted", ToastLength.Long).Show();
                return;
            }

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void RefreshTimeTable(object sender, EventArgs Args)
        {

            try
            {
                Database = UserIO.LoadStorage("Timetable");
                if (Database == null)
                {
                    throw new Exception();
                }
                Toast.MakeText(this.ApplicationContext, "Loaded Timetable.xml", ToastLength.Long).Show();
                UpdateAll(null, null);
            }
            catch (Exception)
            {
                Toast.MakeText(this.ApplicationContext, "Unable to load Timetable.xml", ToastLength.Long).Show();
            }
        }

        public void UpdateAll(object sender, ElapsedEventArgs Args)
        {
            if (Database == null) { return; }
            Toast.MakeText(this.ApplicationContext, "Updating all views", ToastLength.Long).Show();

            TimeSlot Current = null;
            TimeSlot Next = null;

            string MinutesTillNext = "0";
            double CurrentProgress = 0;
            string CurrentName = "None";
            string CurrentRoom = "None";

            string NextName = "None";
            string NextRoom = "None";

            Current = Utils.GetCurrentTimeSlot(Database.timeSlots);
            Next = Utils.GetNextTimeSlot(Database.timeSlots);
            
            if (Next != null)
            {
                MinutesTillNext = Utils.MinutesTillNext(Next).ToString() + " mins";
                NextName = Next.name;
                NextRoom = Next.room;
            }
            if (Current != null) 
            {
                CurrentProgress = Utils.CurrentProgress(Current) *100.0;
                CurrentName = Current.name;
                CurrentRoom = Current.room;
            }

            //Values acquired, Update viewed ones

            CurrentEventProgressBar.Progress = (int)CurrentProgress;
            CurrentEventNameTextView.Text = CurrentName;
            CurrentLocationTextView.Text = CurrentRoom;

            NextEventNameTextView.Text = NextName;
            NextLocationTextView.Text = NextRoom;
            NextEventCounterTextview.Text = MinutesTillNext;
            
        }
	}
}

