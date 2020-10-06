using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefOrganiser
{
    public static class Pages
    {
        public static Type MainPage => typeof(Views.MainPage);
        public static Type ShowImagePage => typeof(Views.ReferencePage);
    }
}
