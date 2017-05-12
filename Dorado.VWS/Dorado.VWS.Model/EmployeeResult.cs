using System.Xml.Serialization;

namespace Dorado.VWS.Model
{
    public class EmployeeEntity
    {
        public string id { get; set; }

        public string email { get; set; }

        public string isLeader { get; set; }

        public string branchCompanyMngDeptId { get; set; }

        public string extension { get; set; }

        public string mngDeptId { get; set; }

        public string deptId { get; set; }

        public string salesLevel { get; set; }

        public string empRole { get; set; }

        public string rdRole { get; set; }

        public string userName { get; set; }
    }

    [XmlRoot("result")]
    public class EmployeeResult
    {
        public string status { get; set; }

        public EmployeeEntity employee { get; set; }
    }
}