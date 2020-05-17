# CacheKeyFactory
Create unique keys for use with any caching system. 🔑🔑🔑

## Usage

Let's say you have a .NET core app and you want to create a unique cache entry in your `IDistributedCache` based on several different values.

### Initialization

```csharp
// Create a cache key factory instance
var cacheKeyFactory = new CacheKeyFactory();

// You can also change the encoding and token seperator used when creating keys by passing 
// them into the constructor. UTF8 encoding and the tilde symbol (~) are used by default.
var cacheKeyFactory = new CacheKeyFactory(Encoding.UTF8, "~");
```

Then use the derive methods to generate a key as a string, hexidecimal hash, or a byte array:

```csharp
string stringKey = cacheKeyFactory.DeriveStringKey("Hello", 123); // "Hello~123"
string hashKey = cacheKeyFactory.DeriveStringKeyHash(HashAlgorithmType.Sha1, "Hello"); // "F7FF9E8B7BB2E09B70935A5D785E0CC5D9D0ABF0"
byte[] byteKey = cacheKeyFactory.DeriveByteKey("Hello", 123);
```

### Generics

You also have the ability to pass a generic parameter which is prepended to the key. This can be useful for creating context or groupings around a set of keys. The first parameter is `bool useFullyQualifiedTypeName = false` and determines if the fully qualified type name is used or just the simplified name of the type. The remaining parameters are any number of inputs as normal to create the key:

```csharp
var key = cacheKeyFactory.DeriveStringKey<CacheKeyFactory>(); // "CacheKeyFactory"
var key = cacheKeyFactory.DeriveStringKey<CacheKeyFactory>(false, 123, "Hello"); // "CacheKeyFactory~123~Hello"
var key = cacheKeyFactory.DeriveStringKey<CacheKeyFactory>(true, "World"); // "AF.Extensions.Caching.CacheKeyFactory~World"
```