using System;
using System.Collections.Generic;

namespace storeme.ViewModels
{
    public class Latest30Days
    {
        public List<string> Dates { get; set; }

        public List<int> Dashboards { get; set; }

        public List<int> Files { get; set; }
    }
}