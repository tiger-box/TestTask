using System.Globalization;
using System.Net;

namespace Test
{
	public class Application
	{
		private string fileLog;
		private string fileOutput;
		private List<IPInfo> addresses = new List<IPInfo>();

		private DateTime? timeStart = null;
		private DateTime? timeEnd = null;

		private IPAddress? addressStart = null;
		private IPAddress? addressMask = null;

		public void Run(string[] args)
		{
			try
			{
				ValidateArguments(args);
			}
			catch (ArgumentException e)
			{
				Console.WriteLine(e.Message);
				return;
			}
			catch
			{
				Console.WriteLine("Неверно указаны аргументы");
				return;
			}

			ReadFile();
			WriteFile();
			Console.WriteLine("Успешно");
		}

		private void WriteFile()
		{
			try
			{
				using (StreamWriter streamWriter = new StreamWriter(fileOutput))
				{
					foreach (var ipInfo in addresses)
					{
						streamWriter.WriteLine($"{ipInfo.ipAddress};{ipInfo.count}");
					}

					streamWriter.Close();
				}
			}
			catch (DirectoryNotFoundException e)
			{
				Console.WriteLine($"Указанная директория {e.Message} не найдена");
			}
			catch (IOException e)
			{
				Console.WriteLine($"Ошибка чтения файла {fileOutput}");
			}
			catch
			{
				Console.WriteLine($"Ошибка записи в файл");
			}
		}

		private void ReadFile()
		{
			try
			{
				using (StreamReader streamReader = new StreamReader(fileLog))
				{
					string? line;
					while ((line = streamReader.ReadLine()) != null)
					{
						int delimeterPos = line.IndexOf(':');

						IPAddress ip = IPAddress.Parse(line.Substring(0, delimeterPos));
						DateTime date = DateTime.Parse(line.Substring(delimeterPos + 1));

						if (timeStart != null && date < timeStart || timeEnd != null && date > timeEnd)
							continue;

						if (addressMask != null && !IPAddressExtensions.IsInRange(ip, addressStart, addressMask))
							continue;
						else if (addressStart != null && !IPAddressExtensions.IsInRange(ip, addressStart))
							continue;

						IPInfo ipInfo;
						if ((ipInfo = addresses.Find(ipInfo => ipInfo.ipAddress.Equals(ip))) != null)
							ipInfo.IncrementCount();
						else
							addresses.Add(new IPInfo() { ipAddress = ip });
					}
					streamReader.Close();
				}
			}
			catch (FileNotFoundException e)
			{
				Console.WriteLine($"Файл {fileLog} не найден");
			}
			catch (DirectoryNotFoundException e)
			{
				Console.WriteLine($"Файл {fileLog} не найден");
			}
			catch (IOException e)
			{
				Console.WriteLine($"Ошибка чтения файла {fileLog}");
			}
			catch (ArgumentOutOfRangeException e)
			{
				Console.WriteLine("Неккоректные данные");
			}
		}

		private void ValidateArguments(string[] args)
		{
			if (args.Length < 4) throw new ArgumentException("Недостаточно аргументов");

			string? addressMaskArg = null;
			string? timeStartArg = null;
			string? timeEndArg = null;

			for (int i = 0; i < args.Length; i++)
			{
				var arg = args[i];
				switch (arg)
				{
					case "--file-log":
						{
							if (fileLog != null) throw new ArgumentException($"Аргумент {arg} указан несколько раз");
							fileLog = args[i + 1];
							break;
						}
					case "--file-output":
						{
							if (fileOutput != null) throw new ArgumentException($"Аргумент {arg} указан несколько раз");
							fileOutput = args[i + 1];
							break;
						}
					case "--address-start":
						{
							if (addressStart != null) throw new ArgumentException($"Аргумент {arg} указан несколько раз");
							addressStart = IPAddress.Parse(args[i + 1]);
							break;
						}
					case "--address-mask":
						{
							if (addressMaskArg != null) throw new ArgumentException($"Аргумент {arg} указан несколько раз");
							addressMaskArg = args[i + 1];
							break;
						}
					case "--time-start":
						{
							if (timeStartArg != null) throw new ArgumentException($"Аргумент {arg} указан несколько раз");
							timeStartArg = args[i + 1];
							break;
						}
					case "--time-end":
						{
							if (timeEndArg != null) throw new ArgumentException($"Аргумент {arg} указан несколько раз");
							timeEndArg = args[i + 1];
							break;
						}
				}
			}

			if (addressMaskArg != null && addressStart == null)
			{
				throw new ArgumentException("Аргумент --adress-mask не может быть указан без --addres-start");
			}
			else if (addressMaskArg != null)
			{
				addressMask = IPAddressExtensions.GetUpperBound(addressStart, Convert.ToInt32(addressMaskArg));
			}

			try
			{
				if (timeStartArg != null) timeStart = DateTime.ParseExact(timeStartArg, "dd.MM.yyyy", CultureInfo.InvariantCulture);
				if (timeEndArg != null) timeEnd = DateTime.ParseExact(timeEndArg, "dd.MM.yyyy", CultureInfo.InvariantCulture);
			}
			catch
			{
				throw new ArgumentException("Указан неверный формат времени");
			}

			if (timeStart != null && timeEnd != null && timeEnd < timeStart)
			{
				throw new ArgumentException("Нижняя граница времени не может быть выше верхней");
			}
		}
	}
}
