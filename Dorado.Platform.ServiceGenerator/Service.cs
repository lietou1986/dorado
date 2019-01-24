using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorado.Platform.ServicesGenerator
{
    public class Service
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public string StartType { get; set; }

        public List<Task> TaskList { get; set; }
    }
}
