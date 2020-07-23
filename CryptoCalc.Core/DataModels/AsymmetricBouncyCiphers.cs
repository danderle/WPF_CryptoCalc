namespace CryptoCalc.Core
{
    /// <summary>
    /// Represents all the available asymmetric cipher algorithims
    /// </summary>
    public enum AsymmetricBouncyCiphers
    {
        DSA = 0,
        RSA,
        ECDsa,
        Gost3410_94,
        ECGost3410,
        ECNR,
        ED25519,
        ECDifiieHellman,
    }
}
