﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOVACHSERVERNETFRAMEWORK
{
    internal interface IBoxCollider
    {
         BoxCollider GetBoxCollider();
        void OnUpdatedBoxCollider();

    }
}
