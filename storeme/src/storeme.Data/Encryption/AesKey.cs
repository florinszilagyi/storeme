using System;

namespace storeme.Data.Encryption
{
    public class AesKey
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AesKey"/> class.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="iv">
        /// The iv.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// key
        ///     or
        ///     iv
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// key
        ///     or
        ///     iv
        /// </exception>
        public AesKey(byte[] key, byte[] iv)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (iv == null)
            {
                throw new ArgumentNullException(nameof(iv));
            }

            if (key.Length != ExpectedKeyLength)
            {
                throw new ArgumentOutOfRangeException(nameof(key));
            }

            if (iv.Length != ExpectedInitializationVectorLength)
            {
                throw new ArgumentOutOfRangeException(nameof(iv));
            }

            this.InitializationVector = iv;
            this.Key = key;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the expected length of the iv.
        /// </summary>
        /// <value>
        ///     The expected length of the iv.
        /// </value>
        private static int ExpectedInitializationVectorLength => 16;

        /// <summary>
        ///     Gets the expected length of the key.
        /// </summary>
        /// <value>
        ///     The expected length of the key.
        /// </value>
        private static int ExpectedKeyLength => 32;

        /// <summary>
        /// Gets or sets the initialization vector.
        /// </summary>
        private byte[] InitializationVector { get; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        private byte[] Key { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The from byte array.
        /// </summary>
        /// <param name="array">
        /// The array.
        /// </param>
        /// <returns>
        /// The <see cref="AesKey"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Byte array must be of appropiate size
        /// </exception>
        public static AesKey FromByteArray(byte[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (array.Length != ExpectedInitializationVectorLength + ExpectedKeyLength)
            {
                throw new ArgumentOutOfRangeException(nameof(array));
            }

            var key = new byte[ExpectedKeyLength];
            var iv = new byte[ExpectedInitializationVectorLength];
            Array.Copy(array, 0, key, 0, ExpectedKeyLength);
            Array.Copy(array, ExpectedKeyLength, iv, 0, ExpectedInitializationVectorLength);

            return new AesKey(key, iv);
        }

        /// <summary>
        /// Gets the initialization vector.
        /// </summary>
        /// <returns>
        /// The <see>
        ///         <cref>byte[]</cref>
        ///     </see>
        ///     .
        /// </returns>
        public byte[] GetInitializationVector()
        {
            return this.InitializationVector;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public byte[] GetKey()
        {
            return this.Key;
        }

        /// <summary>
        ///     The to byte array.
        /// </summary>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>byte[]</cref>
        ///     </see>
        /// </returns>
        public byte[] ToByteArray()
        {
            var result = new byte[this.Key.Length + this.InitializationVector.Length];
            Array.Copy(this.Key, 0, result, 0, this.Key.Length);
            Array.Copy(this.InitializationVector, 0, result, this.Key.Length, this.InitializationVector.Length);

            return result;
        }

        #endregion
    }

}
