using System.Globalization;
using System.Windows.Data;

namespace Lab1
{
    public class CultureResources
    {
        public Strings GetStringsInstance()
        {
            return new Strings();
        }

        private static ObjectDataProvider _provider;

        public static ObjectDataProvider ResourceProvider
        {
            get
            {
                if (_provider == null)
                {
                    _provider = (ObjectDataProvider)System.Windows.Application
                        .Current
                        .FindResource("Strings");
                }

                return _provider;

            }
        }
        public static void ChangeCulture(CultureInfo culture)
        {
            ResourceProvider.Refresh();
        }
    }
}

