using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using GameCore;

namespace ASCIIWorld
{
	public class AppSettings
	{
		private NameValueCollection _appSettings;

		public AppSettings(NameValueCollection appSettings)
		{
			_appSettings = appSettings;	
		}

		public AppSettings()
			: this(ConfigurationManager.AppSettings)
		{
		}

		public double UpdatesPerSecond
		{
			get
			{
				return GetSetting(() => UpdatesPerSecond);
			}
		}

		public double FramesPerSecond
		{
			get
			{
				return GetSetting(() => FramesPerSecond);
			}
		}

		public string ContentRootPath
		{
			get
			{
				return GetSetting(() => ContentRootPath);
			}
		}

		protected T GetSetting<T>(Expression<Func<T>> settingProperty, T defaultValue = default(T))
		{
			var accessor = new PropertyAccessor<T>(settingProperty);
			if (_appSettings.AllKeys.Contains(accessor.Name))
			{
				var textValue = _appSettings[accessor.Name];
				return (T)ConvertEx.ChangeType<T>(textValue);
			}
			else
			{
				return defaultValue;
			}
		}
	}
}
