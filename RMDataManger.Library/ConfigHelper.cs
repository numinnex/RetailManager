﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMDataManger.Library
{

    public class ConfigHelper
    {
        public static decimal GetTaxRate()
        {

            string rateText = ConfigurationManager.AppSettings["taxRate"];

            bool isValidTaxRate = Decimal.TryParse(rateText, out decimal output);

            if (!isValidTaxRate)
                throw new ConfigurationErrorsException("The tax rate is not setup properly");

            return output;
        }
    }
}
