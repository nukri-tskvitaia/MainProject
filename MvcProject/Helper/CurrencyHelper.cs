namespace MvcProject.Helper;

public static class CurrencyHelper
{
    public static char GetCurrencyAsSymbol(int value)
    {
        char symbol = '$';

        switch (value)
        {
            case 1:
                symbol = '€';
                break;
            case 3:
                symbol = '₾';
                break;
            default:
                break;
        }

        return symbol;
    }

    public static int GetCurrencyAsValue(string currency)
    {
        int value = 2;

        switch (currency)
        {
            case "EUR":
                value = 1;
                break;
            case "GEL":
                value = 3;
                break;
            default:
                break;
        }

        return value;
    }
}
