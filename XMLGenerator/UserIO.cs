using System;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

namespace XMLGenerator
{
	class UserIO
	{
		private const string SavePath = "Saves";
		private const string BackupPath = "Backups";
		private const string Extension = ".xml";
		private static string GetHandle(string name)
		{
			return Path.Combine(SavePath, name + Extension);
		}

		private static string GetBackupHandle(string name)
		{
			return Path.Combine(BackupPath, name + Extension);
		}
		public static bool Save<T>(string path, byte[] data) where T : class
		{
			string handle = UserIO.GetHandle(path);
			bool flag = false;
			try
			{
				string backupHandle = UserIO.GetBackupHandle(path);
				DirectoryInfo directory = new FileInfo(handle).Directory;
				if (!directory.Exists)
				{
					directory.Create();
				}
				directory = new FileInfo(backupHandle).Directory;
				if (!directory.Exists)
				{
					directory.Create();
				}
				using (FileStream fileStream = File.Open(backupHandle, FileMode.Create, FileAccess.Write))
				{
					fileStream.Write(data, 0, data.Length);
				}

				//Try loading the save file to test if save was good.
				if (UserIO.Load<T>(path, true) != null)
				{
					File.Copy(backupHandle, handle, true);
					flag = true;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("ERROR: " + ex.ToString());
			}
			if (!flag)
			{
				Console.WriteLine("Save Failed");
			}
			return flag;
		}

		public static T Load<T>(string path, bool backup = false) where T : class
		{
			string path2 = (!backup) ? UserIO.GetHandle(path) : UserIO.GetBackupHandle(path);
			T result = default(T);
			try
			{
				if (File.Exists(path2))
				{
					using (FileStream fileStream = File.OpenRead(path2))
					{
						result = UserIO.Deserialise<T>(fileStream);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("ERROR: " + ex.ToString());
			}
			return result;
		}

		private static T Deserialise<T>(Stream stream) where T : class
		{
			T t = default(T);
			try
			{
				t = (T)((object)new XmlSerializer(typeof(T)).Deserialize(stream));
			}
			catch (Exception)
			{
			}
			if (t == null)
			{
				stream.Position = 0L;
				t = (T)((object)new BinaryFormatter().Deserialize(stream));
			}
			return t;
		}

		public static bool Exists(string path)
		{
			return File.Exists(UserIO.GetHandle(path));
		}

		public static bool Delete(string path)
		{
			string handle = UserIO.GetHandle(path);
			if (File.Exists(handle))
			{
				File.Delete(handle);
				return true;
			}
			return false;
		}

		public static byte[] Serialise<T>(T instance)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				new XmlSerializer(typeof(T)).Serialize(memoryStream, instance);
				result = memoryStream.ToArray();
			}
			return result;
		}

		public static void SaveTimeTable(TimeTable Table)
		{
			UserIO.Save<TimeTable>("TimeTable.xml", UserIO.Serialise<TimeTable>(Table));
		}
	}

	public class TimeTable
	{
		public List<Day> days; //length = daysPerWeek*weeks
		public int weeks; //number of weeks for cycling
		public int daysPerWeek;
		public List<int> CounterStart;
		public List<int> CounterEnd;
	}
	public class Day
	{
		public List<TimeSlot> TimeSlots;
		public string name; //name of the day. i.e. Monday
	}

	public class TimeSlot
	{
		public string name;
		public string room;
		public DateTime Start;
		public DateTime End;
	}
}