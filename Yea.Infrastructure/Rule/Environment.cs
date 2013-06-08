using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yea.Infrastructure.Rule.ValueProviders;

namespace Yea.Infrastructure.Rule
{
    public static class Environment
    {
        public static IValueProviderFactory CurrentValueProviderFactory { get; set; }
    }
}
