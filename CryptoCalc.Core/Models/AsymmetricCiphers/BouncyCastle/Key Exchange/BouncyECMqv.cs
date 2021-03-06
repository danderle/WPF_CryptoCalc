﻿using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace CryptoCalc.Core
{
    /// <summary>
    /// Class for EC MQV key exchange
    /// </summary>
    public class BouncyECMqv : BaseBouncyAsymmetric, IAsymmetricKeyExchange, IECAlgorithims
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BouncyECMqv()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the available ec curve providers
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<string> GetEcProviders()
        {
            var list = Enum.GetValues(typeof(EcCurveProvider)).Cast<EcCurveProvider>().Select(t => t.ToString()).ToList();
            return new ObservableCollection<string>(list);
        }

        /// <summary>
        /// Gets a list of all available ec curves
        /// </summary>
        /// <returns>the list of all ec curves</returns>
        public ObservableCollection<string> GetEcCurves(EcCurveProvider provider)
        {
            return GetCurves(provider);
        }

        /// <summary>
        /// Create a key pair by using a given ec curve
        /// </summary>
        /// <param name="curveName">the curve to use for key creation</param>
        public void CreateKeyPair(string curveName)
        {
            var oid = ECNamedCurveTable.GetOid(curveName);
            int xLength;
            int yLength;
            var keyGenerationParameters = new ECKeyGenerationParameters(oid, new SecureRandom());
            var keyGenerator = new ECKeyPairGenerator();

            do
            {
                keyGenerator.Init(keyGenerationParameters);
                keyPair = keyGenerator.GenerateKeyPair();
                xLength = ((ECPublicKeyParameters)keyPair.Public).Q.AffineXCoord.ToBigInteger().ToByteArrayUnsigned().Length;
                yLength = ((ECPublicKeyParameters)keyPair.Public).Q.AffineYCoord.ToBigInteger().ToByteArrayUnsigned().Length;
            }
            while (xLength != yLength);
        }

        /// <summary>
        /// Returns the private key
        /// </summary>
        /// <returns>private key in bytes</returns>
        public byte[] GetPrivateKey()
        {
            return GetPrivateKeyInfo();
        }

        /// <summary>
        /// Returns the public key info
        /// </summary>
        /// <returns>the public key in bytes</returns>
        public byte[] GetPublicKey()
        {
            return GetPublicKeyInfo();
        }

        /// <summary>
        /// Derives a shared secret key from a private key and another persons public key
        /// </summary>
        /// <param name="myPrivateKey">the private key which is used</param>
        /// <param name="otherPartyPublicKey">the public key of the other person</param>
        /// <returns></returns>
        public byte[] DeriveKey(byte[] myPrivateKey, byte[] otherPartyPublicKey)
        {
            ECPrivateKeyParameters privKey = null;
            try
            {
                privKey = (ECPrivateKeyParameters)CreateAsymmetricKeyParameterFromPrivateKeyInfo(myPrivateKey);
            }
            catch (InvalidCastException exception)
            {
                string message = "Private Key Import Failed!\n" +
                    $"{exception.Message}.\n" +
                    "The contents of the source do not represent a valid EC private key parameter\n" +
                    "Verify that the public key is not corrupted.\n" +
                    "- or - Verify that the correct key is selected.";
                throw new CryptoException(message, exception);
            }

            var mqvParameters = new MqvPrivateParameters(privKey, privKey);
            var a1 = new ECMqvBasicAgreement();
            a1.Init(mqvParameters);

            ECPublicKeyParameters pubKey = null; 
            try
            {
                pubKey = (ECPublicKeyParameters)CreateAsymmetricKeyParameterFromPublicKeyInfo(otherPartyPublicKey);
            }
            catch (InvalidCastException exception)
            {
                string message = "Public Key Import Failed!\n" +
                    $"{exception.Message}.\n" +
                    "The contents of the source do not represent a valid EC public key parameter\n" +
                    "Verify that the public key is not corrupted.\n" +
                    "- or - Verify that the correct key is selected.";
                throw new CryptoException(message, exception);
            }

            var mqvPubParameters = new MqvPublicParameters(pubKey, pubKey);

            BigInteger k = null;
            try
            {
                k = a1.CalculateAgreement(mqvPubParameters);
            }
            catch(InvalidOperationException exception)
            {
                string message = "Key Deriviation Failed!\n" +
                    $"{exception.Message}.\n" +
                    "The public key does not use the same domain parameters as the private key.\n" +
                    "Verify that the correct public key is selected.";
                throw new CryptoException(message, exception);
            }

            return k.ToByteArrayUnsigned();
        }

        #endregion
    }
}
