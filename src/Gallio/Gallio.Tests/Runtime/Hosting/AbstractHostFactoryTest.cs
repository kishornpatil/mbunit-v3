// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections;
using System.IO;
using System.Runtime.Remoting;
using Gallio.Framework.Utilities;
using Gallio.Reflection;
using Gallio.Runtime.Hosting;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.Hosting
{
    public abstract class AbstractHostFactoryTest : BaseUnitTest
    {
        public abstract IHostFactory Factory { get; }

        [Test, ExpectedArgumentNullException]
        public void CreateHostThrowsIfHostSetupIsNull()
        {
            Factory.CreateHost(null, new LogStreamLogger());
        }

        [Test, ExpectedArgumentNullException]
        public void CreateHostThrowsIfLoggerIsNull()
        {
            Factory.CreateHost(new HostSetup(), null);
        }

        [Test]
        public void PingSucceedsUntilHostIsDisposed()
        {
            IHost host;
            using (host = Factory.CreateHost(new HostSetup(), new LogStreamLogger()))
            {
                // Should work fine.
                host.Ping();
            }

            // Should fail.
            InterimAssert.Throws<Exception>(delegate { host.Ping(); });
        }

        [Test]
        public void CreateInstanceCreatesAValidObjectHandle()
        {
            using (IHost host = Factory.CreateHost(new HostSetup(), new LogStreamLogger()))
            {
                Type remoteType = typeof(ArrayList);
                ObjectHandle handle = host.CreateInstance(remoteType.Assembly.FullName, remoteType.FullName);

                Assert.IsInstanceOfType(remoteType, handle.Unwrap());
            }
        }

        [Test]
        public void CreateInstanceFromCreatesAValidObjectHandle()
        {
            using (IHost host = Factory.CreateHost(new HostSetup(), new LogStreamLogger()))
            {
                Type serviceType = typeof(RemoteHostFactoryTest.TestService);
                RemoteHostFactoryTest.TestService serviceProxy = (RemoteHostFactoryTest.TestService)host.CreateInstanceFrom(
                    AssemblyUtils.GetAssemblyLocalPath(serviceType.Assembly), serviceType.FullName).Unwrap();

                Assert.AreEqual(42, serviceProxy.Add(23, 19));
            }
        }

        [Test]
        public void HostRunsInRequestedWorkingDirectory()
        {
            string oldWorkingDirectory = Environment.CurrentDirectory;

            HostSetup hostSetup = new HostSetup();
            hostSetup.WorkingDirectory = Path.GetTempPath();

            using (IHost host = Factory.CreateHost(hostSetup, new LogStreamLogger()))
            {
                HostAssemblyResolverHook.Install(host);
                host.DoCallback(HostRunsInRequestedWorkingDirectoryCallback);
            }

            Assert.AreEqual(oldWorkingDirectory, Environment.CurrentDirectory,
                "Current working directory of the calling process should be unchanged or at least restored once the host is disposed.");
        }

        protected static void HostRunsInRequestedWorkingDirectoryCallback()
        {
            AssertArePathsEqualIgnoringFinalBackslash(Path.GetTempPath(), Environment.CurrentDirectory);
        }

        protected static void AssertArePathsEqualIgnoringFinalBackslash(string expected, string actual)
        {
            Assert.AreEqual(AddFinalBackslashIfAbsent(expected), AddFinalBackslashIfAbsent(actual));
        }

        private static string AddFinalBackslashIfAbsent(string value)
        {
            return value.EndsWith("\\") ? value : value + '\\';
        }
    }
}