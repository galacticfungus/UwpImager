using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleInjector;

namespace RefOrganiser.Services
{
    using System;
    using System.Collections.Generic;
    using SimpleInjector;

    public static class InjectorService
    {
        static InjectorService()
        {
            Default = new Container();
        }

        public static Container Default { get; }

    }
}
