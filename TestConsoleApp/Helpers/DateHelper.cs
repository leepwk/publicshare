using System;

namespace TestConsoleApp.Helpers
{
    public static class DateHelper
    {
        public static bool IsValid(this int value, DateTypeEnum dateType)
        {
            bool result;
            try
            {
                DateTime dt;
                switch (dateType)
                {
                    case DateTypeEnum.Year:
                        dt = new DateTime(value, 1, 1);
                        break;

                    case DateTypeEnum.Month:
                        dt = new DateTime(DateTime.Today.Year, value, 1);
                        break;

                    case DateTypeEnum.Day:
                        dt = new DateTime(DateTime.Today.Year, DateTime.Today.Month, value);
                        break;

                    default:
                        throw new ArgumentException();
                }
                
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }

    }
}
