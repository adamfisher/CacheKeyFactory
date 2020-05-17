using System.Security.Authentication;
using System.Text;
using FluentAssertions;
using Xunit;

namespace AF.Extensions.Caching.Tests
{
    public class CacheKeyFactoryTests
    {
        private CacheKeyFactory CacheKeyFactory { get; set; }

        public CacheKeyFactoryTests()
        {
            CacheKeyFactory = new CacheKeyFactory();
        }

        [Theory]
        [InlineData("CacheKeyFactoryTests", false, null)]
        [InlineData("CacheKeyFactoryTests~", false, new object[] { null })]
        [InlineData("CacheKeyFactoryTests~~", false, new object[] { null, null })]

        [InlineData("AF.Extensions.Caching.Tests.CacheKeyFactoryTests", true, null)]
        [InlineData("AF.Extensions.Caching.Tests.CacheKeyFactoryTests~", true, new object[] { null })]
        [InlineData("AF.Extensions.Caching.Tests.CacheKeyFactoryTests~~", true, new object[] { null, null })]

        [InlineData("CacheKeyFactoryTests~False~1~test~23.6~78~False", false, new object[] { false, 1, "test", 23.6, 78f, false })]
        [InlineData("AF.Extensions.Caching.Tests.CacheKeyFactoryTests~False~1~test~23.6~78~False", true, new object[] { false, 1, "test", 23.6, 78f, false })]
        public void DeriveStringKeyGeneric(string expectedKey, bool useFullyQualifiedName, object[] inputs)
            => CacheKeyFactory.DeriveStringKey<CacheKeyFactoryTests>(useFullyQualifiedName, inputs).Should().Be(expectedKey);

        [Theory]
        [InlineData(null, null)]
        [InlineData("", new object[] { null })]
        [InlineData("~", new object[] { null, null })]
        [InlineData("False~1~test~23.6~78~True", new object[] { false, 1, "test", 23.6, 78f, true })]
        public void DeriveStringKey(string expectedKey, object[] inputs)
            => CacheKeyFactory.DeriveStringKey(inputs).Should().Be(expectedKey);

        [Theory]
        [InlineData("F40DAF942DFDE5F22F1FF542AFCE7F1C", HashAlgorithmType.Md5, false, null)]
        [InlineData("4A69E5905C22AD33DD30C6BD4AC9181E", HashAlgorithmType.Md5, true, null)]
        [InlineData("5A7205AD280D26F2CE365A0AE77E9A0737936793", HashAlgorithmType.Sha1, false, null)]
        [InlineData("C2B7858F3D09FEA8C41483A3C22D2D0F244FF5A3", HashAlgorithmType.Sha1, true, null)]
        [InlineData("5C41FB0E4E1A7C15B59EBC0257629804D6731F7456D77B1F2FE5A00C0C1D64B0", HashAlgorithmType.Sha256, false, null)]
        [InlineData("593EDE22C4B9EB26CA3819C42E66895ED10C0B823512FB24586E781977B108C6", HashAlgorithmType.Sha256, true, null)]

        [InlineData("56ED2814A05BB873063B3387D7221C7F36C45403", HashAlgorithmType.Sha1, false, new object[] { null })]
        [InlineData("01A96D249F443BC5BC4FA06B42F3F13369DB579C", HashAlgorithmType.Sha1, false, new object[] { null, null })]

        [InlineData("09E8116E652530C42C740FB59055DFCD48A2A5C9", HashAlgorithmType.Sha1, false, new object[] { false, 1, "test", 23.6, 78f, false })]
        public void DeriveStringKeyHashGeneric(string expectedKey, HashAlgorithmType hashAlgorithmType, bool useFullyQualifiedTypeName, object[] inputs)
            => CacheKeyFactory.DeriveStringKeyHash<CacheKeyFactoryTests>(hashAlgorithmType, useFullyQualifiedTypeName, inputs).Should().Be(expectedKey);

        [Theory]
        [InlineData(null, HashAlgorithmType.Sha1, null)]
        [InlineData("DA39A3EE5E6B4B0D3255BFEF95601890AFD80709", HashAlgorithmType.Sha1, new object[] { null })]
        [InlineData("FB3C6E4DE85BD9EAE26FDC63E75F10A7F39E850E", HashAlgorithmType.Sha1, new object[] { null, null })]
        [InlineData("5835EA367FF381B5083841231B94BA9E1B640879", HashAlgorithmType.Sha1, new object[] { false, 1, "test", 23.6, 78f, true })]
        public void DeriveStringKeyHash(string expectedKey, HashAlgorithmType hashAlgorithmType, object[] inputs)
            => CacheKeyFactory.DeriveStringKeyHash(hashAlgorithmType, inputs).Should().Be(expectedKey);

        [Theory]
        [InlineData("CacheKeyFactoryTests", false, null)]
        [InlineData("AF.Extensions.Caching.Tests.CacheKeyFactoryTests", true, null)]
        [InlineData("AF.Extensions.Caching.Tests.CacheKeyFactoryTests~False~1~test~23.6~78~False", true, new object[] { false, 1, "test", 23.6, 78f, false })]
        public void DeriveByteKeyGeneric(string expectedKey, bool useFullyQualifiedName, object[] inputs) {
            var bytes = CacheKeyFactory.DeriveByteKey<CacheKeyFactoryTests>(useFullyQualifiedName, inputs);
            var result = Encoding.UTF8.GetString(bytes);

            result.Should().Be(expectedKey);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", new object[] { null })]
        [InlineData("~", new object[] { null, null })]
        [InlineData("False~1~test~23.6~78~True", new object[] { false, 1, "test", 23.6, 78f, true })]
        public void DeriveByteKey(string expectedKey, object[] inputs) {
            var bytes = CacheKeyFactory.DeriveByteKey(inputs);
            var result = bytes == null ? null : Encoding.UTF8.GetString(bytes);

            result.Should().Be(expectedKey);
        }

        [Theory]
        [InlineData("5A7205AD280D26F2CE365A0AE77E9A0737936793", HashAlgorithmType.Sha1, false, null)]
        [InlineData("56ED2814A05BB873063B3387D7221C7F36C45403", HashAlgorithmType.Sha1, false, new object[] {null})]
        [InlineData("01A96D249F443BC5BC4FA06B42F3F13369DB579C", HashAlgorithmType.Sha1, false, new object[] {null, null})]
        [InlineData("BA46CFF08EF4EA580290957D228791BD1B55AA51", HashAlgorithmType.Sha1, true, new object[] {false, 1, "test", 23.6, 78f, true})]
        public void DeriveByteKeyHashGeneric(string expectedKey, HashAlgorithmType hashAlgorithmType, bool useFullyQualifiedTypeName, object[] inputs)
        {
            var bytes = CacheKeyFactory.DeriveByteKeyHash<CacheKeyFactoryTests>(hashAlgorithmType, useFullyQualifiedTypeName, inputs);
            var result = bytes == null ? null : Encoding.UTF8.GetString(bytes);

            result.Should().Be(expectedKey);
        }

        [Theory]
        [InlineData(null, HashAlgorithmType.Sha1, null)]
        [InlineData("DA39A3EE5E6B4B0D3255BFEF95601890AFD80709", HashAlgorithmType.Sha1, new object[] {null})]
        [InlineData("FB3C6E4DE85BD9EAE26FDC63E75F10A7F39E850E", HashAlgorithmType.Sha1, new object[] {null, null})]
        [InlineData("5835EA367FF381B5083841231B94BA9E1B640879", HashAlgorithmType.Sha1,
            new object[] {false, 1, "test", 23.6, 78f, true})]
        public void DeriveByteKeyHash(string expectedKey, HashAlgorithmType hashAlgorithmType, object[] inputs)
        {
            var bytes = CacheKeyFactory.DeriveByteKeyHash(hashAlgorithmType, inputs);
            var result = bytes == null ? null : Encoding.UTF8.GetString(bytes);

            result.Should().Be(expectedKey);
        }
    }
}
