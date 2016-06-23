using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace CommonCore
{
	public abstract class BaseAppSettings<T>
		where T : BaseAppSettings<T>, new()
	{
		#region Fields

		private Configuration _config;
		private KeyValueConfigurationCollection _appSettings;
		private Dictionary<string, object> _propertyCache;

		#endregion

		#region Constructors

		static BaseAppSettings()
		{
			Instance = new T();
		}

		protected BaseAppSettings()
		{
			_config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			_appSettings = _config.AppSettings.Settings;
			_propertyCache = new Dictionary<string, object>();
		}

		#endregion

		#region Properties

		public static T Instance { get; private set; }

		#endregion

		#region Methods

		protected TProperty GetSetting<TProperty>(TProperty defaultValue = default(TProperty), [CallerMemberName] string propertyName = "")
		{
			//var accessor = new PropertyAccessor<TProperty>(settingProperty);

			if (!_propertyCache.ContainsKey(propertyName))
			{
				if (_appSettings.AllKeys.Contains(propertyName))
				{
					var textValue = _appSettings[propertyName].Value;
					_propertyCache[propertyName] = ConvertEx.ChangeType<TProperty>(textValue);
				}
				else
				{
					SetSetting(defaultValue, propertyName);
				}
			}
			return (TProperty)_propertyCache[propertyName];
		}

		protected void SetSetting<TProperty>(TProperty value, [CallerMemberName] string propertyName = "")
		{
			//var accessor = new PropertyAccessor<TProperty>(settingProperty);
			var keyExists = _appSettings.AllKeys.Contains(propertyName);
			var isDirty = false;

			if (value == null)
			{
				if (keyExists)
				{
					_appSettings.Remove(propertyName);
					_propertyCache.Remove(propertyName);
					isDirty = true;
				}
			}
			else
			{
				if (keyExists)
				{
					var valueText = ConvertEx.ChangeType<string>(value);
					if (_appSettings[propertyName].Value != valueText)
					{
						_appSettings[propertyName].Value = valueText;
						_propertyCache[propertyName] = value;
						isDirty = true;
					}
				}
				else
				{
					_appSettings.Add(propertyName, value.ToString());
					_propertyCache.Add(propertyName, value);
					isDirty = true;
				}
			}

			if (isDirty)
			{
				_config.Save();
				ConfigurationManager.RefreshSection("appSettings");
			}
		}

		#endregion
	}
}
