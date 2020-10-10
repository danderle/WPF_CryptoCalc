namespace CryptoCalc.Core
{
    /// <summary>
    /// Represents all the available EC curve providers
    /// </summary>
    public enum EcCurveProvider
    {
        TELETRUST = 0,
        GOST3410 ,
        SEC,
        NIST,
        ANSSI,
        GM,
        ED25519,
        ED448
    }
}
