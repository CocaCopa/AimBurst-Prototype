using System;
using AimBurst.Core.Contracts;

namespace AimBurst.Input {
    internal interface IInputPublisher {
        event Action<IClickable> OnLeftClick;
    }
}
