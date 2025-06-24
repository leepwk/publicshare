using TestConsoleApp.Helpers;
using TestConsoleApp.Interfaces;

namespace TestConsoleApp
{
    public class Roman : IRoman
    {
        public string Convert(string romans)
        {
            var result = 0;
            var year = string.Empty;
            var month = string.Empty;
            var day = string.Empty;

            var splitRomans = romans.ToUpper().Split(':');
            var dateType = DateTypeEnum.Year;

            if (splitRomans.Length > 2)
            {
                dateType = DateTypeEnum.Day;
            } 
            else if (splitRomans.Length > 1)
            {
                dateType = DateTypeEnum.Month;
            }

            for (int j = 0; j < splitRomans.Length; j++)
            {
                var datePart = splitRomans[j];

                if (!datePart.IsValidRoman())
                {
                    break;
                }

                for (int i = 0; i < datePart.Length; i++)
                {
                    if (i + 1 < datePart.Length)
                    {
                        if (StringHelper.RomanDictionary[datePart[i]] >= StringHelper.RomanDictionary[datePart[i + 1]])
                        {
                            result += StringHelper.RomanDictionary[datePart[i]];
                        }
                        else
                        {
                            result -= StringHelper.RomanDictionary[datePart[i]];
                        }
                    }
                    else
                    {
                        result += StringHelper.RomanDictionary[datePart[i]];
                    }
                }

                switch (dateType)
                {
                    case DateTypeEnum.Year:
                        year = result.IsValid(DateTypeEnum.Year) ? result.ToString() : "";
                        break;

                    case DateTypeEnum.Month:
                        month = result.IsValid(DateTypeEnum.Month) ? $"{result:D2}" : "";
                        dateType = DateTypeEnum.Year;
                        break;

                    case DateTypeEnum.Day:
                        day = result.IsValid(DateTypeEnum.Day) ? $"{result:D2}" : "";
                        dateType = DateTypeEnum.Month;
                        break;
                }

                result = 0;
            }

            return $"{day}{month}{year}";
        }



    }
    
}
