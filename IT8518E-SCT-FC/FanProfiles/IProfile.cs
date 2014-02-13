using System;
using System.Collections.Generic;
using System.Text;

namespace SCTFanControl
{
    interface IProfile
    {
        int intervalMs {get;}
        int safe { get; }
        int trip { get; }
        int speed { get; }
        string name { get;}
    }
}
