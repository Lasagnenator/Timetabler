using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Timetabler
{
    class UserIO
    {
		private const string Extension = ".xml";
		public static string root;

		private static string GetHandle(string name)
		{
			
			return Path.Combine(root, name + Extension);
		}

		private static string GetBackupHandle(string name)
		{
			return Path.Combine(root, name + Extension);
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
				else
				{
					throw new System.IO.FileNotFoundException("File not found");
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

		public static TimeTable LoadStorage(string name)
		{
			return UserIO.Load<TimeTable>(name);
		}
	}
}