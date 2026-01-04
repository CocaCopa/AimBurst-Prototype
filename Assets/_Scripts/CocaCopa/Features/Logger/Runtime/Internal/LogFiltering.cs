using System;

namespace CocaCopa.Logger.Runtime.Internal {
    [Flags]
    internal enum LogFiltering {
        None = 0,
        Messages = 1,
        Warnings = 2,
        Errors = 4
    }
}
