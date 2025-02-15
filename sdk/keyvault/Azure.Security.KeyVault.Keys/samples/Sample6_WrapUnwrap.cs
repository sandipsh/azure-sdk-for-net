﻿using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Azure.Security.KeyVault.Keys.Samples
{
    /// <summary>
    /// Sample demonstrates how to wrap and unwrap a symmetric key with an RSA key using the synchronous methods of the CryptographyClient.
    /// </summary>
    [Category("Live")]
    public partial class Sample6_WrapUnwrap
    {
        [Test]
        public void WrapUnwrapSync()
        {
            // Environment variable with the Key Vault endpoint.
            string keyVaultUrl = Environment.GetEnvironmentVariable("AZURE_KEYVAULT_URL");

            // Instantiate a key client that will be used to create a key. Notice that the client is using default Azure
            // credentials. To make default credentials work, ensure that environment variables 'AZURE_CLIENT_ID',
            // 'AZURE_CLIENT_KEY' and 'AZURE_TENANT_ID' are set with the service principal credentials.
            var keyClient = new KeyClient(new Uri(keyVaultUrl), new DefaultAzureCredential());

            // First create a RSA key which will be used to wrap and unwrap another key
            string rsaKeyName = $"CloudRsaKey-{Guid.NewGuid()}";
            var rsaKey = new RsaKeyCreateOptions(rsaKeyName, hsm: false, keySize: 2048);

            Key cloudRsaKey = keyClient.CreateRsaKey(rsaKey);
            Debug.WriteLine($"Key is returned with name {cloudRsaKey.Name} and type {cloudRsaKey.KeyMaterial.KeyType}");

            // Let's create the CryptographyClient which can perform cryptographic operations with the key we just created. 
            // Again we are using the default Azure credential as above.
            var cryptoClient = new CryptographyClient(cloudRsaKey.Id, new DefaultAzureCredential());

            // Next we'll generate a symmetric key which we will wrap
            byte[] keyData = AesManaged.Create().Key;
            Debug.WriteLine($"Generated Key: {Convert.ToBase64String(keyData)}");

            // Wrap the key using RSAOAEP with the created key.
            WrapResult wrapResult = cryptoClient.WrapKey(KeyWrapAlgorithm.RSAOAEP, keyData);
            Debug.WriteLine($"Encrypted data using the algorithm {wrapResult.Algorithm}, with key {wrapResult.KeyId}. The resulting encrypted data is {Convert.ToBase64String(wrapResult.EncryptedKey)}");

            // Now unwrap the encrypted key. Note that the same algorithm must always be used for both wrap and unwrap
            UnwrapResult unwrapResult = cryptoClient.UnwrapKey(KeyWrapAlgorithm.RSAOAEP, wrapResult.EncryptedKey);
            Debug.WriteLine($"Decrypted data using the algorithm {unwrapResult.Algorithm}, with key {unwrapResult.KeyId}. The resulting decrypted data is {Encoding.UTF8.GetString(unwrapResult.Key)}");

            // The Cloud RSA Key is no longer needed, need to delete it from the Key Vault.
            keyClient.DeleteKey(rsaKeyName);

            // To ensure key is deleted on server side.
            Assert.IsTrue(WaitForDeletedKey(keyClient, rsaKeyName));

            // If the keyvault is soft-delete enabled, then for permanent deletion, deleted key needs to be purged.
            keyClient.PurgeDeletedKey(rsaKeyName);

        }

        private bool WaitForDeletedKey(KeyClient client, string keyName)
        {
            int maxIterations = 20;
            for (int i = 0; i < maxIterations; i++)
            {
                try
                {
                    client.GetDeletedKey(keyName);
                    return true;
                }
                catch
                {
                    Thread.Sleep(5000);
                }
            }
            return false;
        }
    }
}
