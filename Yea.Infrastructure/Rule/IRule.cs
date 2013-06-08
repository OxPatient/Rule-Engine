using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yea.Infrastructure.Rule
{
    public interface IRule
    {
        bool Evaluate(object instance);
    }
}
