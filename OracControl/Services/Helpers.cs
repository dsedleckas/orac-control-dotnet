using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OracControl.Services
{
    public static class Helpers
    {
        public static void OnKeyPress(UIKeyboardEventArgs args)
        {
            Console.WriteLine(args.Key);
        }
    }
}
