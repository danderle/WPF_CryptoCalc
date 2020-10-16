using Org.BouncyCastle.Asn1.Anssi;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.GM;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.TeleTrust;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;

namespace CryptoCalc.Core
{
    /// <summary>
    /// An interface for algortihims that use EC curves
    /// </summary>
    public interface IECAlgorithims : IAsymmetricCipher
    {
        /// <summary>
        /// A flag for knowing if the algorithim uses elliptical curves
        /// </summary>
        public bool UsesCurves => true;

        /// <summary>
        /// Gets the available ec curve providers
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<string> GetEcProviders();

        /// <summary>
        /// Gets a list of all available ec curves
        /// </summary>
        /// <returns>the list of all ec curves</returns>
        public ObservableCollection<string> GetEcCurves(EcCurveProvider provider);

        /// <summary>
        /// Create a key pair by using a given ec curve
        /// </summary>
        /// <param name="curveName">the curve to use for key creation</param>
        public void CreateKeyPair(string curveName);

         /// <summary>
        /// Creates a a dictionary for all the available msdn ec curves
        /// </summary>
        public static Dictionary<string, ECCurve> GetAllAvailableMsdnEcCurves()
        {
            Dictionary<string, ECCurve> ecCurves = new Dictionary<string, ECCurve>
            {
                { nameof(ECCurve.NamedCurves.brainpoolP160r1), ECCurve.NamedCurves.brainpoolP160r1 },
                { nameof(ECCurve.NamedCurves.brainpoolP160t1), ECCurve.NamedCurves.brainpoolP160t1 },
                { nameof(ECCurve.NamedCurves.brainpoolP192r1), ECCurve.NamedCurves.brainpoolP192r1 },
                { nameof(ECCurve.NamedCurves.brainpoolP192t1), ECCurve.NamedCurves.brainpoolP192t1 },
                { nameof(ECCurve.NamedCurves.brainpoolP224r1), ECCurve.NamedCurves.brainpoolP224r1 },
                { nameof(ECCurve.NamedCurves.brainpoolP224t1), ECCurve.NamedCurves.brainpoolP224t1 },
                { nameof(ECCurve.NamedCurves.brainpoolP256r1), ECCurve.NamedCurves.brainpoolP256r1 },
                { nameof(ECCurve.NamedCurves.brainpoolP256t1), ECCurve.NamedCurves.brainpoolP256t1 },
                { nameof(ECCurve.NamedCurves.brainpoolP320r1), ECCurve.NamedCurves.brainpoolP320r1 },
                { nameof(ECCurve.NamedCurves.brainpoolP320t1), ECCurve.NamedCurves.brainpoolP320t1 },
                { nameof(ECCurve.NamedCurves.brainpoolP384r1), ECCurve.NamedCurves.brainpoolP384r1 },
                { nameof(ECCurve.NamedCurves.brainpoolP384t1), ECCurve.NamedCurves.brainpoolP384t1 },
                { nameof(ECCurve.NamedCurves.brainpoolP512r1), ECCurve.NamedCurves.brainpoolP512r1 },
                { nameof(ECCurve.NamedCurves.brainpoolP512t1), ECCurve.NamedCurves.brainpoolP512t1 },
                { nameof(ECCurve.NamedCurves.nistP256), ECCurve.NamedCurves.nistP256 },
                { nameof(ECCurve.NamedCurves.nistP384), ECCurve.NamedCurves.nistP384 },
                { nameof(ECCurve.NamedCurves.nistP521), ECCurve.NamedCurves.nistP521 }
            };
            return ecCurves;
        }

        /// <summary>
        /// Gets a list of available ec curves
        /// </summary>
        /// <returns>the list of all ec curves</returns>
        public static ObservableCollection<string> GetBouncyEcCurves(EcCurveProvider provider)
        {
            IEnumerator curves = null;
            switch (provider)
            {
                case EcCurveProvider.SEC:
                    curves = SecNamedCurves.Names.GetEnumerator();
                    break;
                case EcCurveProvider.NIST:
                    curves = NistNamedCurves.Names.GetEnumerator();
                    break;
                case EcCurveProvider.TELETRUST:
                    curves = TeleTrusTNamedCurves.Names.GetEnumerator();
                    break;
                case EcCurveProvider.ANSSI:
                    curves = AnssiNamedCurves.Names.GetEnumerator();
                    break;
                case EcCurveProvider.GOST3410:
                    curves = ECGost3410NamedCurves.Names.GetEnumerator();
                    break;
                case EcCurveProvider.GM:
                    curves = GMNamedCurves.Names.GetEnumerator();
                    break;
            }
            var list = new ObservableCollection<string>();
            while (curves.MoveNext())
            {
                list.Add((string)curves.Current);
            }

            //sort list
            list = new ObservableCollection<string>(list.OrderBy(x => x));
            return list;
        }
    }
}
