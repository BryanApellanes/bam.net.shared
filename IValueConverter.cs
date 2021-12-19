using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public interface IValueConverter<TOutput> : IValueConverter
    {
        TOutput ConvertBytesToObject(byte[] bytes);

        byte[] ConvertObjectToBytes(TOutput value);
    }

    public interface IValueConverter
    {
        string ConvertBytesToString(byte[] bytes);

        byte[] ConvertStringToBytes(string value);
    }
}
