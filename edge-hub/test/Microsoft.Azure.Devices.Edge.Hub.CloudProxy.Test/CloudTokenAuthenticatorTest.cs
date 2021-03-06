// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.Azure.Devices.Edge.Hub.CloudProxy.Test
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.Devices.Edge.Hub.CloudProxy.Authenticators;
    using Microsoft.Azure.Devices.Edge.Hub.Core;
    using Microsoft.Azure.Devices.Edge.Hub.Core.Cloud;
    using Microsoft.Azure.Devices.Edge.Hub.Core.Device;
    using Microsoft.Azure.Devices.Edge.Hub.Core.Identity;
    using Microsoft.Azure.Devices.Edge.Util;
    using Microsoft.Azure.Devices.Edge.Util.Test.Common;
    using Moq;
    using Xunit;

    [Unit]
    public class CloudTokenAuthenticatorTest
    {
        [Fact]
        public async Task AuthenticateTest()
        {
            // Arrange
            var deviceIdentity = Mock.Of<IDeviceIdentity>(d => d.Id == "d1" && d.DeviceId == "d1");
            IClientCredentials credentials = new TokenCredentials(deviceIdentity, Guid.NewGuid().ToString(), string.Empty);
            var cloudProxy = Mock.Of<ICloudProxy>(c => c.OpenAsync() == Task.FromResult(true));
            var connectionManager = Mock.Of<IConnectionManager>(c => c.CreateCloudConnectionAsync(credentials) == Task.FromResult(Try.Success(cloudProxy)));
            IAuthenticator cloudAuthenticator = new CloudTokenAuthenticator(connectionManager);

            // Act
            bool isAuthenticated = await cloudAuthenticator.AuthenticateAsync(credentials);

            // Assert
            Assert.True(isAuthenticated);
        }

        [Fact]
        public async Task AuthenticateFailureTest()
        {
            // Arrange
            var deviceIdentity = Mock.Of<IDeviceIdentity>(d => d.Id == "d1" && d.DeviceId == "d1");
            IClientCredentials credentials = new TokenCredentials(deviceIdentity, Guid.NewGuid().ToString(), string.Empty);
            var cloudProxy = Mock.Of<ICloudProxy>();
            Mock.Get(cloudProxy).Setup(c => c.OpenAsync()).ThrowsAsync(new Exception("Unauthorized"));
            var connectionManager = Mock.Of<IConnectionManager>(c => c.CreateCloudConnectionAsync(credentials) == Task.FromResult(Try.Success(cloudProxy)));
            IAuthenticator cloudAuthenticator = new CloudTokenAuthenticator(connectionManager);

            // Act
            bool isAuthenticated = await cloudAuthenticator.AuthenticateAsync(credentials);

            // Assert
            Assert.False(isAuthenticated);
        }
    }
}
