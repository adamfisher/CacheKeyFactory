using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;

namespace AF.Extensions.Caching
{
    /// <summary>
    /// A factory for generating unique keys based on one or more input values.
    /// </summary>
    public class CacheKeyFactory
    {
        #region Fields & Properties

        private readonly Encoding _encoding;
        private readonly string _tokenSeparator;
        private readonly IDictionary<HashAlgorithmType, HashAlgorithm> _hashAlgorithms;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheKeyFactory"/> class.
        /// </summary>
        /// <param name="encoding">The encoding to use when computing values. Defaults to UTF-8 if one is not specified.</param>
        /// <param name="tokenSeparator">The token separator to use for separating inputs. Default is tilde symbol (~).</param>
        public CacheKeyFactory(Encoding encoding = null, string tokenSeparator = "~")
        {
            _encoding = encoding ?? Encoding.UTF8;
            _tokenSeparator = tokenSeparator;
            _hashAlgorithms = new Dictionary<HashAlgorithmType, HashAlgorithm>(6);
        }

        #endregion

        #region Methods

        #region String Keys

            /// <summary>
            /// Derives a unique cache key value based on the inputs.
            /// </summary>
            /// <typeparam name="T">A type that will be prefixed to the cache key.</typeparam>
            /// <param name="useFullyQualifiedTypeName">if set to <c>true</c> [use fully qualified type name]. Default is false.</param>
            /// <param name="inputs">The inputs used to compute the key.</param>
            /// <returns>The compute hash value.</returns>
            public string DeriveStringKey<T>(bool useFullyQualifiedTypeName = false, params object[] inputs)
            {
                var typeName = useFullyQualifiedTypeName ? typeof(T).FullName : typeof(T).Name;

                if (inputs == null)
                    return typeName;

                PrependArrayItem(ref inputs, typeName);
                return DeriveStringKey(inputs);
            }

            /// <summary>
            /// Derives a unique cache key value based on the inputs.
            /// </summary>
            /// <param name="inputs">The inputs used to compute the key.</param>
            /// <returns>The compute hash value.</returns>
            public string DeriveStringKey(params object[] inputs)
            {
                if (inputs == null)
                    return null;

                var convertedInputs = Array.ConvertAll(inputs, input => input?.ToString());
                var separator = inputs.Length > 1 ? (_tokenSeparator ?? String.Empty) : String.Empty;
                return String.Join(separator, convertedInputs);
            }

        #endregion

        #region Hash Keys

        /// <summary>
        /// Derives a unique cache key value based on the inputs.
        /// </summary>
        /// <typeparam name="T">A type that will be prefixed to the cache key.</typeparam>
        /// <param name="hashAlgorithmType">Type of the hash algorithm.</param>
        /// <param name="useFullyQualifiedTypeName">if set to <c>true</c> [use fully qualified type name]. Default is false.</param>
        /// <param name="inputs">The inputs used to compute the key.</param>
        /// <returns>
        /// The compute hash value.
        /// </returns>
        public string DeriveStringKeyHash<T>(HashAlgorithmType hashAlgorithmType = HashAlgorithmType.Sha1, 
            bool useFullyQualifiedTypeName = false, params object[] inputs)
        {
            var typeName = useFullyQualifiedTypeName ? typeof(T).FullName : typeof(T).Name;

            if (inputs != null)
            {
                PrependArrayItem(ref inputs, typeName);
            }
            else
            {
                inputs = new object[] { typeName };
            }
            
            return DeriveStringKeyHash(hashAlgorithmType, inputs);
        }

        /// <summary>
        /// Derives a unique cache key value based on the inputs.
        /// </summary>
        /// <param name="hashAlgorithmType">Type of the hash algorithm.</param>
        /// <param name="inputs">The inputs used to compute the key.</param>
        /// <returns>
        /// The compute hash value.
        /// </returns>
        public string DeriveStringKeyHash(HashAlgorithmType hashAlgorithmType = HashAlgorithmType.Sha1, params object[] inputs)
        {
            var key = DeriveStringKey(inputs);

            if (key == null)
                return null;

            HashAlgorithm hasher = null;

            if (!_hashAlgorithms.ContainsKey(hashAlgorithmType))
            {
                hasher = HashAlgorithm.Create(hashAlgorithmType.ToString());
                _hashAlgorithms.Add(hashAlgorithmType, hasher);
            }

            var hashedBytes = hasher.ComputeHash(_encoding.GetBytes(key));
            key = String.Join(String.Empty, Array.ConvertAll(hashedBytes, b => b.ToString("X2")));

            return key;
        }

        #endregion

        #region Byte Keys

        /// <summary>
        /// Derives a unique cache key value based on the inputs.
        /// </summary>
        /// <typeparam name="T">A type that will be prefixed to the cache key.</typeparam>
        /// <param name="useFullyQualifiedTypeName">if set to <c>true</c> [use fully qualified type name]. Default is false.</param>
        /// <param name="inputs">The inputs used to compute the key.</param>
        /// <returns>The compute hash value.</returns>
        public byte[] DeriveByteKey<T>(bool useFullyQualifiedTypeName = false, params object[] inputs) =>
            _encoding.GetBytes(DeriveStringKey<T>(useFullyQualifiedTypeName, inputs));

        /// <summary>
        /// Derives a unique cache key value based on the inputs.
        /// </summary>
        /// <param name="inputs">The inputs used to compute the key.</param>
        /// <returns>
        /// The compute hash value.
        /// </returns>
        public byte[] DeriveByteKey(params object[] inputs)
        {
            if (inputs == null)
                return null;

            return _encoding.GetBytes(DeriveStringKey(inputs));
        }

        /// <summary>
        /// Derives a unique cache key value based on the inputs.
        /// </summary>
        /// <typeparam name="T">A type that will be prefixed to the cache key.</typeparam>
        /// <param name="hashAlgorithmType">Type of the hash algorithm.</param>
        /// <param name="useFullyQualifiedTypeName">if set to <c>true</c> [use fully qualified type name]. Default is false.</param>
        /// <param name="inputs">The inputs used to compute the key.</param>
        /// <returns>
        /// The compute hash value.
        /// </returns>
        public byte[] DeriveByteKeyHash<T>(HashAlgorithmType hashAlgorithmType = HashAlgorithmType.Sha1,
            bool useFullyQualifiedTypeName = false, params object[] inputs)
            => _encoding.GetBytes(DeriveStringKeyHash<T>(hashAlgorithmType, useFullyQualifiedTypeName, inputs));

        /// <summary>
        /// Derives a unique cache key value based on the inputs.
        /// </summary>
        /// <param name="hashAlgorithmType">Type of the hash algorithm.</param>
        /// <param name="inputs">The inputs used to compute the key.</param>
        /// <returns>
        /// The compute hash value.
        /// </returns>
        public byte[] DeriveByteKeyHash(HashAlgorithmType hashAlgorithmType = HashAlgorithmType.Sha1, params object[] inputs)
        {
            if (inputs == null)
                return null;

            return _encoding.GetBytes(DeriveStringKeyHash(hashAlgorithmType, inputs));
        }

        #endregion

        /// <summary>
        /// Prepends an item to the beginning of an array.
        /// </summary>
        /// <param name="array">The array to prepend the item to.</param>
        /// <param name="obj">The object to be prepended.</param>
        private void PrependArrayItem(ref object[] array, object obj)
        {
            array = array ?? new object[]{};
            var argumentCount = array.Length;

            Array.Resize(ref array, array.Length + 1);
            Array.ConstrainedCopy(array, 0, array, 1, argumentCount);
            array[0] = obj;
        }

        #endregion
    }
}
