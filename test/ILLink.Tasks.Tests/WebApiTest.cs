using System;
using System.IO;
using System.Xml.Linq;
using Xunit;
using Xunit.Abstractions;

namespace ILLink.Tests
{
	public class WebApiTest : IntegrationTestBase
	{
		private string csproj;

		public WebApiTest(ITestOutputHelper output) : base(output) {
			csproj = SetupProject();
		}

		public string SetupProject()
		{
			string projectRoot = "webapi";
			string csproj = Path.Combine(projectRoot, $"{projectRoot}.csproj");

			if (File.Exists(csproj)) {
				output.WriteLine($"using existing project {csproj}");
				return csproj;
			}

			if (Directory.Exists(projectRoot)) {
				Directory.Delete(projectRoot, true);
			}

			Directory.CreateDirectory(projectRoot);
			int ret = Dotnet("new webapi", projectRoot);
			if (ret != 0) {
				output.WriteLine("dotnet new failed");
				Assert.True(false);
			}

			PreventPublishFiltering(csproj);

			AddLinkerReference(csproj);

			return csproj;
		}

		// TODO: Remove this once we figure out what to do about apps
		// that have the publish output filtered by a manifest
		// file. It looks like aspnet has made this the default. See
		// the bug at https://github.com/dotnet/sdk/issues/1160.
		private void PreventPublishFiltering(string csproj) {
			var xdoc = XDocument.Load(csproj);
			var ns = xdoc.Root.GetDefaultNamespace();

			var propertygroup = xdoc.Root.Element(ns + "PropertyGroup");

			output.WriteLine("setting PublishWithAspNetCoreTargetManifest=false");
			propertygroup.Add(new XElement(ns + "PublishWithAspNetCoreTargetManifest",
										   "false"));

			using (var fs = new FileStream(csproj, FileMode.Create)) {
				xdoc.Save(fs);
			}
		}

		[Fact]
		public void RunWebApiStandalone()
		{
			string executablePath = BuildAndLink(csproj, selfContained: true);
			CheckOutput(executablePath, selfContained: true);
		}

		[Fact]
		public void RunWebApiPortable()
		{
			string target = BuildAndLink(csproj, selfContained: false);
			CheckOutput(target, selfContained: false);
		}

		void CheckOutput(string target, bool selfContained = false)
		{
			string terminatingOutput = "Now listening on: http://localhost:5000";
			int ret = RunApp(target, out string commandOutput, 60000, terminatingOutput, selfContained: selfContained);
			Assert.True(commandOutput.Contains("Application started. Press Ctrl+C to shut down."));
			Assert.True(commandOutput.Contains(terminatingOutput));
		}
	}
}
