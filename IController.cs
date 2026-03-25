interface IController
{
    bool IsUpPressed { get; }
    bool IsDownPressed { get; }
    bool IsLeftPressed { get; }
    bool IsRightPressed { get; }
    bool IsEscPressed { get; }
    bool IsPickupPressed { get; }

    ControllerInput ReadInput();
}
