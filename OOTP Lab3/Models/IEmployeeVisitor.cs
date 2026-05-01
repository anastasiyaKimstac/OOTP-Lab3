using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOTP_Lab3.Models
{
    /// <summary>
    /// Visitor pattern interface - eliminates need for reflection and conditional statements
    /// </summary>
    public interface IEmployeeVisitor
    {
        string Visit(Manager manager);
        string Visit(Developer developer);
        string Visit(Designer designer);
        string Visit(Tester tester);
        string Visit(Intern intern);
    }
}
