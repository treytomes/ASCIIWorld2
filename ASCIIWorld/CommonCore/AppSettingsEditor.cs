using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CommonCore
{
	public class AppSettingsEditor<TSettings>
		where TSettings : BaseAppSettings<TSettings>, new()
	{
		#region Constants

		private const string PROMPT = "> ";

		#endregion

		#region Fields

		private TSettings _settings;
		private PropertyInfo[] _properties;

		#endregion

		#region Constructors

		public AppSettingsEditor(TSettings settings)
		{
			_settings = settings;
			_properties = typeof(TSettings).GetProperties(BindingFlags.Instance | BindingFlags.Public);
		}

		#endregion

		#region Methods

		public void Run()
		{
			Task.Run(() => RunAsync());
		}

		private void RunAsync()
		{
			Console.WriteLine("App Settings Editor");
			Console.WriteLine("Syntax: PropertyName=<property value>");
			Console.WriteLine("? - List properties.");

			while (true)
			{
				try
				{
					Console.Write(PROMPT);
					var input = Console.ReadLine().Trim();

					if (input == "?")
					{
						ListProperties();
					}
					else if (input.Contains("="))
					{
						var midPoint = input.IndexOf('=');
						var propertyName = input.Substring(0, midPoint).Trim();
						var propertyValue = input.Substring(midPoint + 1).Trim();

						AssignProperty(propertyName, propertyValue);
					}
					else
					{
						Console.WriteLine("Syntax error!");
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
		}

		private void ListProperties()
		{
			Console.WriteLine("Properties:");
			foreach (var property in _properties)
			{
				var value = property.GetValue(_settings);

				if (property.PropertyType == typeof(char))
				{
					Console.WriteLine($"\t{property.Name} ({property.PropertyType}) = '{value}'");
				}
				else if (property.PropertyType == typeof(string))
				{
					Console.WriteLine($"\t{property.Name} ({property.PropertyType}) = \"{value}\"");
				}
				else
				{
					Console.WriteLine($"\t{property.Name} ({property.PropertyType}) = {value}");
				}
			}
		}

		private void AssignProperty(string propertyName, string propertyValue)
		{
			var selectedProperties = _properties.Where(x => x.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
			if (selectedProperties.Count() <= 0)
			{
				Console.WriteLine($"'{propertyName}' is not a valid property name.");
			}
			else if (selectedProperties.Count() > 1)
			{
				Console.WriteLine($"Invalid settings file.  There are too many properties named {propertyName}.");
			}
			else
			{
				Console.WriteLine($"Assigning {propertyName}={propertyValue}");

				var property = selectedProperties.Single();
				property.SetValue(_settings, ConvertEx.ChangeType(propertyValue, property.PropertyType));
			}
		}

		#endregion
	}
}