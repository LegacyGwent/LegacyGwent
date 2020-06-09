using System;

public static class NumberConverter
{
    private static string keys = "+-*/&!@#$%abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";//编码,可加一些字符也可以实现72,96等任意进制转换，但是有符号数据不直观，会影响阅读。
    private static int exponent = keys.Length;//幂数

    public static string To64(this int value)
    {
        string result = string.Empty;
        do
        {
            int index = value % exponent;
            result = keys[(int)index] + result;
            value = (value - index) / exponent;
        }
        while (value > 0);

        return result;
    }

    public static int To10(this string value)
    {
        int result = 0;
        for (int i = 0; i < value.Length; i++)
        {
            int x = value.Length - i - 1;
            result += keys.IndexOf(value[i]) * Pow(exponent, x);
        }
        return result;
    }

    private static int Pow(int baseNo, int x)
    {
        int value = 1;////1 will be the result for any number's power 0.任何数的0次方，结果都等于1
        while (x > 0)
        {
            value = value * baseNo;
            x--;
        }
        return value;
    }
}