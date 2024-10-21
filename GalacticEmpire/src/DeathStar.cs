namespace GalacticEmpire;

public class DeathStar
{
    public string Fire(int reactorIgnitionCount = 10)
    {
        if (reactorIgnitionCount <= 0 || reactorIgnitionCount > 10)
        {
            throw new ArgumentOutOfRangeException($"{nameof(reactorIgnitionCount)}");
        }

        return string.Join(" ", Enumerable.Repeat("pew", reactorIgnitionCount));
    }

    public int SurfaceTurboLaserCount {get; private set;} = 1000;
    public DeathStarStatus Status { get; private set; } = DeathStarStatus.FullyArmedAndOperational;

    public bool IsTheEmperorAboard { get; set; } = false;
    public bool IsMoon => false;

    public string CurrentSystem => "Yavin IV";

    public string CanteenFoodTemperature => "Hot enough to need a tray";

    public void PhotonTorpedoStrike(DeathStarSurfaceParts location)
    {
        switch(location)
        {
            case DeathStarSurfaceParts.TurboLasers:
                if (SurfaceTurboLaserCount > 0)
                {
                    SurfaceTurboLaserCount -= 1;
                }
                break;
            case DeathStarSurfaceParts.Towers:
                // Do nothing
                break;
            case DeathStarSurfaceParts.ThermalExhaustPort:
                // Like bullseye-ing womp rats in your T-16 back home
                Status = DeathStarStatus.Destroyed;
                break;
        }
    }
}