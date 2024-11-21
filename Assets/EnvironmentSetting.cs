using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnvironmentSetting
{
    public static bool prod = true;
    public static string masterServerUrl = prod ? "https://master-royale.stage.inf.fh-dortmund.de" : "http://localhost";
    public static int masterServerPort = prod ? 443 : 3000;
    public static string gameServerUrl = prod ? "gsrv-royale.stage.inf.fh-dortmund.de" : "127.0.0.1";
}
