using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plazma.Models
{
    // -1 по умолчанию необработанное исключение.
    // 0 без ошибки
    // 1
    // 2
    // 3
    // 4
    // 5
    // 6
    // 7 ненайден CNC-файл для отображения

    public class Error
    {
        public int number;
        public string text;
    }
}