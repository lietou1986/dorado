using System.Collections.Generic;

namespace Dorado.Platform.ServicesGenerator
{
    public class ServiceGenerator
    {
        public ServiceGenerator()
        {
            InitTaskList();
        }

        public List<Task> TaskList { get; set; }

        public void InitTaskList()
        {
        }

        public void Gen(Service service, string outputPath)
        {

        }
    }
}