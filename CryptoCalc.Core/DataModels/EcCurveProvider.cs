namespace CryptoCalc.Core
{
    /// <summary>
    /// Represents all the available EC curve providers
    /// </summary>
    public enum EcCurveProvider
    {
        SEC = 0,
        NIST,
        TELETRUST,
        ANSSI,
        GOST3410,
        GM,
    }
}
