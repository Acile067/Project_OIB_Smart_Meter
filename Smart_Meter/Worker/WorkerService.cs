﻿using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worker
{
    public class WorkerService : IWorker
    {
        public void TestCommunicationWorker()
        {
            Console.WriteLine("Success connected to Worker");
        }
    }
}
