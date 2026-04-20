﻿using System.Collections.Generic;
using Plazma.Controllers;

namespace Plazma.Models.Services
{
    public class OrderDataService
    {
        private readonly PartsClass _parts;

        public OrderDataService(PartsClass parts)
        {
            _parts = parts;
        }

        public void ReadOrders(int timeInDays = 180)
        {
            _parts.ReadOrders(timeInDays);
        }

        public List<string> CheckOrdersDone(int cncId)
        {
            return _parts.checkOrdersDone(cncId);
        }
    }
}
